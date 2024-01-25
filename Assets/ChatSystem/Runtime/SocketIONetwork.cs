#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Klem.SocketChat.ChatSystem.ChannelContainers;
using Klem.SocketChat.ChatSystem.Const;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.Infra;
using Klem.SocketChat.ChatSystem.SettingsScript;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using SocketIOClient.Transport;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Klem.SocketChat.ChatSystem
{
   
    
    public static partial class SocketIONetwork
    {
        
        public static bool IsConnected => Socket is { Connected: true };
        public static bool InRoom => CurrentRoom != null;
        
        public static SocketIOUser User { get; private set; } = new SocketIOUser();
        
        private static Room _currentRoom;
        public static Room CurrentRoom
        {
            get => _currentRoom;
            set
            {
                _currentRoom = value;
                User.RoomId = value?.Name;
            }
        }
        

        
        public static TimeSpan TimeSpanSinceLastPing { get; private set; }

        public static string Uri => ChatSettings.Instance.ServerUri + ":" + ChatSettings.Instance.ServerPort;
        
    }

    public static partial class SocketIONetwork
    {
        #region Socket Events Callback Channels
        private static readonly ConnectionChannelsContainer ConnectionChannels = new ConnectionChannelsContainer();
        private static readonly ErrorChannelsContainer ErrorChannels = new ErrorChannelsContainer();
        private static readonly RoomChannelsContainer RoomChannels = new RoomChannelsContainer();
        
        private static readonly MessageChannel<SocketIOUser> NewUserConnectedToMasterMessageChannel = new MessageChannel<SocketIOUser>();

        private static readonly MessageChannel<ChatMessage> ChatMessageMessageChannel = new MessageChannel<ChatMessage>();
        private static readonly MessageChannel<SocketIOUser> GetUserMessageChannel = new MessageChannel<SocketIOUser>();
        private static readonly MessageChannel<ChatInvite> InviteRequestMessageChannel = new MessageChannel<ChatInvite>();


        /// <summary>
        ///     Add a MonoBehaviourSocketCallBacks to the callback channels.<br />
        /// </summary>
        /// <param name="callbacks">
        ///     The MonoBehaviourSocketCallBacks to add to the callback channels.<br />
        /// </param>
        public static void AddCallbackTarget(MonoBehaviourSocketCallBacks callbacks)
        {

            AddCallbackTarget(ConnectionChannels, callbacks);
            AddCallbackTarget(ErrorChannels, callbacks);
            AddCallbackTarget(RoomChannels, callbacks);

            NewUserConnectedToMasterMessageChannel.Subscribe(callbacks.OnNewUserConnectedToMaster);

            ChatMessageMessageChannel.Subscribe(callbacks.OnChatMessage);
            GetUserMessageChannel.Subscribe(callbacks.OnGetUser);
            InviteRequestMessageChannel.Subscribe(callbacks.OnChatInviteReceived);

        }


        /// <summary>
        ///     Remove a MonoBehaviourSocketCallBacks from the callback channels.<br />
        /// </summary>
        /// <param name="monoBehaviourSocketCallBacks">
        ///     The MonoBehaviourSocketCallBacks to remove from the callback channels.<br />
        /// </param>
        public static void RemoveCallbackTarget(MonoBehaviourSocketCallBacks monoBehaviourSocketCallBacks)
        {
            RemoveCallbackTarget(ConnectionChannels, monoBehaviourSocketCallBacks);
            RemoveCallbackTarget(ErrorChannels, monoBehaviourSocketCallBacks);
            RemoveCallbackTarget(RoomChannels, monoBehaviourSocketCallBacks);
            
            NewUserConnectedToMasterMessageChannel.Unsubscribe(monoBehaviourSocketCallBacks.OnNewUserConnectedToMaster);

            ChatMessageMessageChannel.Unsubscribe(monoBehaviourSocketCallBacks.OnChatMessage);
            GetUserMessageChannel.Unsubscribe(monoBehaviourSocketCallBacks.OnGetUser);
            InviteRequestMessageChannel.Unsubscribe(monoBehaviourSocketCallBacks.OnChatInviteReceived);

        }
        #endregion


    }

    public static partial class SocketIONetwork
    {
        private static SocketIOUnity Socket { get; set; }


        private const string LogPrefix = "<color=\"blue\"> [SocketManager] </color> ";


        #region Socket Methods
        /// <summary>
        ///     Connect to the socket server using the settings provided in the ChatSettings ScriptableObject<br />
        ///     If the settings are not set, it will throw an exception. <br />
        ///     If the connection is successful, it will set the Socket property to the connected socket and setup the callbacks
        ///     channels.
        /// </summary>
        public static async Task Connect()
        {
            string uri;
            string port;
            try
            {

                uri = ChatSettings.Instance.ServerUri;
                port = ChatSettings.Instance.ServerPort;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }


#if UNITY_EDITOR
            EditorApplication.quitting += async () => { await Disconnect(); };
#else
            Application.quitting += async () => { await Disconnect(); };
#endif

            Socket = new SocketIOUnity(uri + ":" + port, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    { "token", ChatSettings.Instance.ServerToken },
                    { "username", User.Username}
                },
                Transport = TransportProtocol.WebSocket,
            });

            Socket.JsonSerializer = new NewtonsoftJsonSerializer();

            SetupCallbacks();
            
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"Attempting to connect to {uri}:{port} with token {ChatSettings.Instance.ServerToken}");
            }

            await Socket.ConnectAsync();

        }

        private static void SetupCallbacks()
        {
            // Setup core SocketIOUnity callbacks
            Socket.OnConnected += OnConnected;
            Socket.OnDisconnected += OnDisconnected;
            Socket.OnReconnectAttempt += OnReconnectAttempt;

            Socket.OnError += SocketOnError;

            Socket.OnPing += OnPing;
            Socket.OnPong += OnPong;
            Socket.OnReconnectFailed += OnReconnectFailed;
            Socket.OnReconnected += OnReconnected;
            Socket.OnReconnectError += OnReconnectError;

            Socket.OnUnityThread(SocketReservedEvents.ERROR, OnError);
            Socket.OnUnityThread(SocketReservedEvents.CONNECT_ERROR, response => { Debug.LogError($"{LogPrefix} connect_error {response.GetValue<string>()}"); });

            Socket.OnUnityThread("on-user-updated", response => { User = response.GetValue<SocketIOUser>(); });

            Socket.OnUnityThread(SocketConnectionEvents.CONNECTION, OnConnectedToChat);
            Socket.OnUnityThread(SocketConnectionEvents.ON_NEW_USER_CONNECTED_TO_MASTER, OnNewUserConnectedToMaster);
            Socket.OnUnityThread(SocketConnectionEvents.DISCONNECTING, OnPlayerDisconnecting);

            Socket.OnUnityThread("chat-message", OnChatMessage);

            #region Room Events
            Socket.OnUnityThread(SocketRoomEventsIn.CREATE_ROOM_FAILED, OnCreateRoomFailed);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_CREATED, OnNewRoomCreated);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_LIST_UPDATED, OnRoomListUpdated);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_JOINED_BY_CURRENT_USER, OnRoomJoined);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_NEW_USER_JOINED, OnUserJoinedRoom);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_LEFT_BY_CURRENT_USER, OnRoomLeft);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_LEFT_BY_OTHER_USER, OnRoomLeftByOtherUser);
            #endregion

            Socket.OnUnityThread("list-ids", OnListIds);
            Socket.OnUnityThread("invite-request", OnInviteRequest);

        }

        /// <summary>
        ///     Disconnect from the socket server.<br />
        /// </summary>
        public static async Task Disconnect()
        {
            if (IsConnected)
            {
                await Socket.DisconnectAsync();
                if (ChatSettings.Instance.EnableLogging)
                {
                    Debug.Log($"{LogPrefix} disconnected.");
                }
            }
            else
            {
                Socket.Dispose();
            }

        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Send a chat message to the server.<br />
        /// </summary>
        /// <param name="message">
        ///     The message to send.<br />
        /// </param>
        public static async Task SendChatMessage(string message)
        {

            ChatMessage chatMessage = new ChatMessage
            {
                Message = message,
                Sender = User,
            };


            await Socket.EmitAsync("chat-message", response => { chatMessage = response.GetValue<ChatMessage>(); }, chatMessage);

        }

        public static void ListIds()
        {
            Socket.EmitAsync("list-ids");
        }

        public static async Task<SocketIOUser> GetChatUser(string chatId)
        {
            SocketIOUser user = null;


            await Socket.EmitAsync("get-user", response => { user = response.GetValue<SocketIOUser>(); }, chatId);

            return user;
        }

        #region Room Methods
        /// <summary>
        ///     Create a room on the server.<br />
        /// </summary>
        /// <param name="room">
        ///     The room to create.<br />
        /// </param>
        public static void CreateRoom(Room room)
        {
            Socket.Emit(SocketRoomEventsOut.ROOM_CREATE, room);
        }
        
        /// <summary>
        ///   Leave the current room.<br />
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task LeaveRoom()
        {
            if (InRoom)
            {
                await Socket.EmitAsync(SocketRoomEventsOut.ROOM_LEAVE, CurrentRoom);
            }
        }
        
        /// <summary>
        ///     GetAllRooms from the server.<br />
        /// </summary>
        /// <returns>
        ///     The list of rooms.<br />
        /// </returns>
        public static async Task<Room[]> GetAllRooms()
        {
            Room[] rooms = null;

            await Socket.EmitAsync(SocketRoomEventsOut.ROOM_GET_ALL, response => { rooms = response.GetValue<Room[]>(); });

            return rooms;
        }
        
        private static void OnRoomJoined(SocketIOResponse obj)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} on-room-creation-success response {obj.GetValue<Room>()}");
            }
            
            CurrentRoom = obj.GetValue<Room>();
            
            RoomChannels.RoomCreatedMessageChannel.Publish(CurrentRoom);
            
        }
        
        #endregion

        


        public static async Task UpdateUser(SocketIOUser user)
        {
            await Socket.EmitAsync("update-user", user);
        }
        #endregion


        #region Socket Connection Callbacks
        private static void OnConnected(object sender, EventArgs e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} Connected to socket server: {Socket.Connected} {(sender as SocketIO).Id} sender {sender} e {e}");
            }
        }

        private static void OnPing(object sender, EventArgs e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} OnPing {e}");
            }

            ConnectionChannels.PingMessageChannel.Publish(e);
        }

        private static void OnPong(object sender, TimeSpan e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} OnPong sender {(sender as SocketIO).Id} TimeSpan {e.Milliseconds}");
            }
            TimeSpanSinceLastPing = e;
            ConnectionChannels.PongMessageChannel.Publish(e.Milliseconds);
        }

        private static void OnDisconnected(object sender, string e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} OnDisconnected {e}");
            }

            ConnectionChannels.DisconnectMessageChannel.Publish(e);
        }


        private static void OnReconnectError(object sender, Exception e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} OnReconnectError sender {sender} e {e}");
            }

            ConnectionChannels.ReconnectErrorMessageChannel.Publish(e);
        }

        private static void OnReconnected(object sender, int e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} OnReconnected sender {sender} e {e}");
            }

            ConnectionChannels.ReconnectedMessageChannel.Publish(e);
        }

        private static void OnReconnectFailed(object sender, EventArgs e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} OnReconnectFailed sender {sender} e {e}");
            }

            ConnectionChannels.ReconnectFailedMessageChannel.Publish(e);
        }

        /// <summary>
        ///     Called when the socket server throws an error.<br />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SocketOnError(object sender, string e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.LogError($"{LogPrefix} Socket Error: sender {sender} e {e}");
            }
            ErrorChannels.ErrorMessageChannel.Publish(e);
        }

        private static void OnReconnectAttempt(object sender, int e)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} OnReconnectAttempt sender {sender} attempts {e}");
            }
            ConnectionChannels.ReconnectAttemptMessageChannel.Publish(e);
        }

        private static async void OnConnectedToChat(SocketIOResponse data)
        {

            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} on-connected-to-chat response {data.GetValue<SocketServerConnection>()}");
            }

            SocketServerConnection connection = data.GetValue<SocketServerConnection>();
            User.ChatId = connection.ChatId;
            Color color = ChatSettings.Instance.UserColors[Random.Range(0, ChatSettings.Instance.UserColors.Count)];
            User.Color = ColorUtility.ToHtmlStringRGB(color);
            await Socket.EmitAsync("update-user", User);
            ConnectionChannels.ConnectionMessageChannel.Publish(connection);
        }

        private static void OnNewUserConnectedToMaster(SocketIOResponse data)
        {
            NewUserConnectedToMasterMessageChannel.Publish(data.GetValue<SocketIOUser>());
        }
        #endregion

        #region Private Callbacks
        /// <summary>
        ///     Called when the socket server emits an "error" event.<br />
        /// </summary>
        /// <param name="data"></param>
        private static void OnError(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.LogError($"{LogPrefix} error {data.GetValue<string>()}");
            }
            ErrorChannels.ErrorMessageChannel.Publish(data.GetValue<string>());
        }

        private static void OnPlayerDisconnecting(SocketIOResponse data)
        {
            ConnectionChannels.DisconnectingMessageChannel.Publish(data.GetValue<string>());
        }

        private static void OnChatMessage(SocketIOResponse data)
        {
            ChatMessageMessageChannel.Publish(data.GetValue<ChatMessage>());
        }

        private static void OnListIds(SocketIOResponse data)
        {
            Debug.Log($"{LogPrefix} list-ids response {data}");
        }

        private static void OnInviteRequest(SocketIOResponse data)
        {
            ChatInvite invite = data.GetValue<ChatInvite>();

            if (invite.To.ChatId == User.ChatId)
            {
                InviteRequestMessageChannel.Publish(invite);
            }
        }

        private static void OnRoomListUpdated(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} on-room-list-updated response {data.GetValue<Room[]>().Length}");
            }

            RoomChannels.RoomListUpdateMessageChannel.Publish(data.GetValue<Room[]>());
        }


        private static void OnNewRoomCreated(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} on-room-created response {data.GetValue<Room>()}");
            }

            RoomChannels.RoomCreatedMessageChannel.Publish(data.GetValue<Room>());
        }

        private static void OnCreateRoomFailed(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} create-room-failed response {data.GetValue<string>()}");
            }

            RoomChannels.RoomCreationFailedMessageChannel.Publish(data.GetValue<string>());

        }

        private static void OnUserJoinedRoom(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} on-user-joined-room response {data.GetValue<Room>()}");
            }

            RoomChannels.RoomUserJoinedMessageChannel.Publish(data.GetValue<RoomAndUser>());
        }
        
        
        private static void OnRoomLeft(SocketIOResponse obj)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} on-room-left response {obj.GetValue<Room>()}");
            _currentRoom = null;
            RoomChannels.RoomLeftMessageChannel.Publish(obj.GetValue<Room>());
        }
        
        private static void OnRoomLeftByOtherUser(SocketIOResponse obj)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} on-room-left-by-other-user response {obj.GetValue<RoomAndUser>()}");
            RoomChannels.RoomLeftByOtherUserMessageChannel.Publish(obj.GetValue<RoomAndUser>());
        }
        
        #endregion


        public static void SendInvite(SocketIOUser toUser)
        {
            Socket.Emit("invite-request", new ChatInvite
            {
                From = User,
                To = toUser,
            });
        }

        public static void JoinRoom(Room room)
        {
            Socket.Emit(SocketRoomEventsOut.ROOM_JOIN, room);
        }


        private static void AddCallbackTarget<T>(List<T> callbackContainer, MonoBehaviourSocketCallBacks monoBehaviourSocketCallBacks) where T : class
        {
            callbackContainer.Add(monoBehaviourSocketCallBacks as T);
        }

        private static void RemoveCallbackTarget<T>(List<T> callbackContainer, MonoBehaviourSocketCallBacks monoBehaviourSocketCallBacks) where T : class
        {
            callbackContainer.Remove(monoBehaviourSocketCallBacks as T);
        }
    }
}
