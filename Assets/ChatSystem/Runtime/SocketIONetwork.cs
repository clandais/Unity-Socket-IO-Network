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


    #region Properties Partial
    
    /// <summary>
    ///   <para>The main class of the SocketIO Network.</para>
    ///   <para>Contains all the methods to connect to the server, create and join rooms and send messages.</para>
    /// <example>Get started by creating a new <see cref="Klem.SocketChat.ChatSystem.SettingsScript.ChatSettings"/> ScriptableObject in your `Resources` folder.
    ///   <code>
    ///     SocketIONetwork.Connect();
    ///   </code>
    /// </example>
    /// </summary>
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
    #endregion

    #region Callbacks Partial
    public static partial class SocketIONetwork
    {
        #region Socket Events Callback Channels
        private static readonly ConnectionChannelsContainer ConnectionChannels = new ConnectionChannelsContainer();
        private static readonly ErrorChannelsContainer ErrorChannels = new ErrorChannelsContainer();
        private static readonly RoomChannelsContainer RoomChannels = new RoomChannelsContainer();
        private static readonly ChatMessageChannelsContainer ChatMessageChannels = new ChatMessageChannelsContainer();

        /// <summary>
        ///     Add a MonoBehaviourSocketCallBacks to the callback channels.<br />
        ///     Done automatically when the MonoBehaviourSocketCallBacks is added to the scene.<br />
        /// </summary>
        /// <param name="callbacks">
        ///     The MonoBehaviourSocketCallBacks to add to the callback channels.<br />
        /// </param>
        public static void AddCallbackTarget(MonoBehaviourSocketCallBacks callbacks)
        {

            AddCallbackTarget(ConnectionChannels, callbacks);
            AddCallbackTarget(ErrorChannels, callbacks);
            AddCallbackTarget(RoomChannels, callbacks);
            AddCallbackTarget(ChatMessageChannels, callbacks);

        }


        /// <summary>
        ///     Remove a MonoBehaviourSocketCallBacks from the callback channels.<br />
        ///     Done automatically when the MonoBehaviourSocketCallBacks is removed from the scene.<br />
        /// </summary>
        /// <param name="monoBehaviourSocketCallBacks">
        ///     The MonoBehaviourSocketCallBacks to remove from the callback channels.<br />
        /// </param>
        public static void RemoveCallbackTarget(MonoBehaviourSocketCallBacks monoBehaviourSocketCallBacks)
        {
            RemoveCallbackTarget(ConnectionChannels, monoBehaviourSocketCallBacks);
            RemoveCallbackTarget(ErrorChannels, monoBehaviourSocketCallBacks);
            RemoveCallbackTarget(RoomChannels, monoBehaviourSocketCallBacks);
            RemoveCallbackTarget(ChatMessageChannels, monoBehaviourSocketCallBacks);
        }
        #endregion


    }
    #endregion

    public static partial class SocketIONetwork
    {
        private static SocketIOUnity Socket { get; set; }


        private const string LogPrefix = "<color=\"blue\"> [SocketManager] </color> ";


        #region Socket Methods
        /// <summary>
        ///     Connect to the socket server using the settings provided in the ChatSettings ScriptableObject (that must be in the Resources folder)<br />
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
            EditorApplication.quitting +=  () => { Disconnect(); };
#else
            Application.quitting += ** () => {  Disconnect(); };
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

            Socket.OnUnityThread(SocketUserEventsIn.ON_USER_UPDATED, OnUserUpdated);

            Socket.OnUnityThread(SocketConnectionEvents.CONNECTION, OnConnectedToMaster);
            Socket.OnUnityThread(SocketConnectionEvents.ON_NEW_USER_CONNECTED_TO_MASTER, OnNewUserConnectedToMaster);
            Socket.OnUnityThread(SocketConnectionEvents.USER_DISCONNECTING, OnPlayerDisconnecting);
            
            #region Room Events
            Socket.OnUnityThread(SocketRoomEventsIn.CREATE_ROOM_FAILED, OnCreateRoomFailed);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_CREATED, OnNewRoomCreated);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_LIST_UPDATED, OnRoomListUpdated);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_JOINED_BY_CURRENT_USER, OnRoomJoined);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_NEW_USER_JOINED, OnUserJoinedRoom);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_LEFT_BY_CURRENT_USER, OnRoomLeft);
            Socket.OnUnityThread(SocketRoomEventsIn.ON_ROOM_LEFT_BY_OTHER_USER, OnRoomLeftByOtherUser);
            #endregion
            
            Socket.OnUnityThread(SocketMessageEventsIn.ON_GENERAL_MESSAGE_RECEIVED, OnGeneralChatMessage);
            Socket.OnUnityThread(SocketMessageEventsIn.ON_ROOM_MESSAGE_RECEIVED, OnRoomChatMessage);

        }

        /// <summary>
        ///     Disconnect from the socket server.<br />
        /// </summary>
        public static void Disconnect()
        {
            if (IsConnected)
            {
                Socket.Disconnect();
                if (ChatSettings.Instance.EnableLogging)
                {
                    Debug.Log($"{LogPrefix} disconnected.");
                }
            }
            else
            {
                Socket?.Dispose();
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
        public static async Task SendGeneralChatMessage(string message)
        {

            ChatMessage chatMessage = new ChatMessage
            {
                Message = message,
                Sender = User,
            };
            
            await Socket.EmitAsync(SocketMessageEventsOut.SEND_GENERAL_MESSAGE, response =>
            {
                chatMessage = response.GetValue<ChatMessage>();
            }, chatMessage);

        }
        
        public static async Task SendRoomChatMessage(string arg0)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Message = arg0,
                Sender = User,
            };
            
            await Socket.EmitAsync(SocketMessageEventsOut.SEND_ROOM_MESSAGE, response =>
            {
                chatMessage = response.GetValue<ChatMessage>();
            }, chatMessage);
        }

        public static async Task<SocketIOUser[]> ListIds(bool log = false)
        {
            SocketIOUser[] users = null;
            await Socket.EmitAsync(SocketUserEventsOut.GET_ALL_USERS, reponse =>
            {
                users = reponse.GetValue<SocketIOUser[]>();
            });
            
            
            if (log)
            {
                for (int i = 0; i < users.Length; i++)
                {
                    Debug.Log($"{LogPrefix} user {i} {users[i].Username}");
                }
            }
            
            return users;
        }

        public static async Task<SocketIOUser> GetChatUser(string chatId)
        {
            SocketIOUser user = null;
            await Socket.EmitAsync(SocketUserEventsOut.GET_USER, response =>
            {
                user = response.GetValue<SocketIOUser>();
            }, chatId);
            return user;
        }

        #region Room Methods
        
        /// <summary>
        ///     GetAllRooms from the server.<br />
        /// </summary>
        /// <returns>
        ///     The list of rooms.<br />
        /// </returns>
        public static async Task<Room[]> GetAllRooms(int maxRooms = 100, bool log = false)
        {
            Room[] rooms = null;
            await Socket.EmitAsync(SocketRoomEventsOut.ROOM_GET_ALL,
                response =>
                {
                    rooms = response.GetValue<Room[]>();
                }, maxRooms);

            if (log)
            {
                for (int i = 0; i < rooms.Length; i++)
                {
                    Debug.Log($"{LogPrefix} room {i} {rooms[i]}");
                }
            }
            
            return rooms;
        }

        public static void JoinRandomRoom()
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} joining random Room");
            Socket.Emit(SocketRoomEventsOut.ROOM_JOIN_RANDOM);
        }
        
        public static void JoinRoom(Room room)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} join-room {room}");
            
            Socket.Emit(SocketRoomEventsOut.ROOM_JOIN, room);
        }
        
        /// <summary>
        ///     Create a room on the server.<br />
        /// </summary>
        /// <param name="room">
        ///     The room to create.<br />
        /// </param>
        public static void CreateRoom(Room room)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} create-room {room}");
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
                if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} leave-room {CurrentRoom}");
                await Socket.EmitAsync(SocketRoomEventsOut.ROOM_LEAVE, CurrentRoom);
            }
        }
        

        
        #endregion




        public static void UpdateUser()
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} update-user {User}");
            Socket.Emit(SocketUserEventsOut.USER_UPDATE, User);
        }
        
        public static void UpdateUser(SocketIOUser user)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} update-user {user}");
            Socket.Emit(SocketUserEventsOut.USER_UPDATE, user);
        }
        #endregion


        #region Socket Connection Callbacks
        private static void OnConnected(object sender, EventArgs e)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} Connected to socket server: {Socket.Connected} {(sender as SocketIO).Id} sender {sender} e {e}");
        }

        private static void OnPing(object sender, EventArgs e)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} OnPing {e}");

            ConnectionChannels.PingMessageChannel.Publish(e);
        }

        private static void OnPong(object sender, TimeSpan e)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} OnPong sender {(sender as SocketIO).Id} TimeSpan {e.Milliseconds}");
            
            TimeSpanSinceLastPing = e;
            ConnectionChannels.PongMessageChannel.Publish(e.Milliseconds);
        }

        private static void OnDisconnected(object sender, string e)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} OnDisconnected {e}");
            CurrentRoom = null;
            User.ChatId = null;
            User.Color = null;
            User.OtherIds = null;
            
            ConnectionChannels.DisconnectMessageChannel.Publish(e);
        }


        private static void OnReconnectError(object sender, Exception e)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} OnReconnectError sender {sender} e {e}");
            ConnectionChannels.ReconnectErrorMessageChannel.Publish(e);
        }

        private static void OnReconnected(object sender, int e)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} OnReconnected sender {sender} e {e}");

            ConnectionChannels.ReconnectedMessageChannel.Publish(e);
        }

        private static void OnReconnectFailed(object sender, EventArgs e)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} OnReconnectFailed sender {sender} e {e}");
            
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
                Debug.LogWarning($"{LogPrefix} Socket Error: sender {sender} e {e}");
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

        private static void OnConnectedToMaster(SocketIOResponse data)
        {

            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} [connection] response {data.GetValue<SocketServerConnection>()}");
            }

            SocketServerConnection connection = data.GetValue<SocketServerConnection>();
            User.ChatId = connection.ChatId;
            User.OtherIds = new List<string>();
            ConnectionChannels.ConnectionMessageChannel.Publish(connection);
        }
        
        private static async void OnUserUpdated(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} on-user-updated response {data.GetValue<SocketIOUser>()}");
            }

            User = data.GetValue<SocketIOUser>();
            if (User.Color == null)
            {
                Color color = ChatSettings.Instance.UserColors[Random.Range(0, ChatSettings.Instance.UserColors.Count)];
                User.Color = ColorUtility.ToHtmlStringRGB(color);
                await Socket.EmitAsync(SocketUserEventsOut.USER_UPDATE, User);    
            }
            
        }

        private static void OnNewUserConnectedToMaster(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} on-new-user-connected-to-master response {data.GetValue<SocketIOUser>().Username}");
            ConnectionChannels.NewUserConnectedToMasterMessageChannel.Publish(data.GetValue<SocketIOUser>());
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
                Debug.LogWarning($"{LogPrefix} error {data.GetValue<string>()}");
            }
            ErrorChannels.ErrorMessageChannel.Publish(data.GetValue<string>());
        }

        private static void OnPlayerDisconnecting(SocketIOResponse data)
        {
            CurrentRoom = null;
            ConnectionChannels.DisconnectingMessageChannel.Publish(data.GetValue<string>());
        }

        private static void OnGeneralChatMessage(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} chat-message response {data.GetValue<ChatMessage>()}");
            }
            
            ChatMessageChannels.GeneralChatMessageChannel.Publish(data.GetValue<ChatMessage>());
        }

        private static void OnRoomChatMessage(SocketIOResponse data)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} room-chat-message response {data.GetValue<ChatMessage>()}");
            ChatMessageChannels.RoomChatMessageChannel.Publish(data.GetValue<ChatMessage>());
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
        
        /// <summary>
        ///   Called when the current user joins a room.<br />
        /// </summary>
        /// <param name="obj"></param>
        private static void OnRoomJoined(SocketIOResponse obj)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} on-room-creation-success response {obj.GetValue<Room>()}");
            
            CurrentRoom = obj.GetValue<Room>();
            RoomChannels.RoomJoinedMessageChannel.Publish(CurrentRoom);
            
        }

        
        /// <summary>
        ///   Called when a user joins the current room.<br />
        /// </summary>
        /// <param name="data"></param>
        private static void OnUserJoinedRoom(SocketIOResponse data)
        {
            RoomAndUser roomAndUser = data.GetValue<RoomAndUser>();
            
            if (ChatSettings.Instance.EnableLogging)
            {
                Debug.Log($"{LogPrefix} on-user-joined-room response {roomAndUser}");
            }

            CurrentRoom = roomAndUser.Room;
            RoomChannels.RoomUserJoinedMessageChannel.Publish(roomAndUser);
        }
        
        
        private static void OnRoomLeft(SocketIOResponse obj)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} on-room-left response {obj.GetValue<Room>()}");
            CurrentRoom = null;
            RoomChannels.RoomLeftMessageChannel.Publish(obj.GetValue<Room>());
        }
        
        private static async void OnRoomLeftByOtherUser(SocketIOResponse obj)
        {
            if (ChatSettings.Instance.EnableLogging) Debug.Log($"{LogPrefix} on-room-left-by-other-user response {obj.GetValue<RoomAndUser>()}");
            
            CurrentRoom = obj.GetValue<RoomAndUser>().Room;
            
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
