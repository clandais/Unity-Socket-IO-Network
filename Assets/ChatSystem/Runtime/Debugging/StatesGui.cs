using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.SettingsScript;
using UnityEngine;

namespace Klem.SocketChat.ChatSystem.Debugging
{
    /// <exclude />
    [AddComponentMenu("Klem/SocketChat/Debugging/StatesGui")]
    public class StatesGui : MonoBehaviourSocketCallBacks
    {
        public Rect GuiOffset = new Rect(600, 180,200, 400);
        public bool DontDestroy = true;
        public bool Latency = true;
        public bool ServerUri = true;
        public bool UserDetails = true;
        public bool RoomDetails = true;
        public bool Buttons = true;

        private Rect GuiRect;
        private static StatesGui Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            if (DontDestroy)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        private void OnDisable()
        {
            if (DontDestroy && Instance == this)
            {
                Instance = null;
            }
        }

        private const float NativeWidth = 800;
        private const float NativeHeight = 600;

        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            SocketIOUser user = SocketIONetwork.User;
            user.ChatId = connection.ChatId;
            Color color = ChatSettings.Instance.UserColors[Random.Range(0, ChatSettings.Instance.UserColors.Count)];
            user.Color = ColorUtility.ToHtmlStringRGB(color);
        }

        private void OnGUI()
        {
            
            //set up scaling
            float rx = Screen.width / NativeWidth;
            float ry = Screen.height / NativeHeight;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

            Rect GuiOffsetRuntime = new Rect(GuiOffset);

            if (GuiOffsetRuntime.x < 0)
            {
                GuiOffsetRuntime.x = Screen.width - GuiOffsetRuntime.width;
            }
            GuiRect.xMin = GuiOffsetRuntime.x;
            GuiRect.yMin = GuiOffsetRuntime.y;
            GuiRect.xMax = GuiOffsetRuntime.x + GuiOffsetRuntime.width;
            GuiRect.yMax = GuiOffsetRuntime.y + GuiOffsetRuntime.height;
            
            using (new GUILayout.AreaScope(GuiRect))
            {

                using (new GUILayout.HorizontalScope())
                {
                    if (Latency)
                    {
                        GUILayout.Label(SocketIONetwork.TimeSpanSinceLastPing.Milliseconds + "ms");
                    }

                    if (ServerUri)
                    {
                        GUILayout.Label(SocketIONetwork.Uri);
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (UserDetails)
                    {
                        GUILayout.Label($"UserName: {SocketIONetwork.User.Username}");
                        GUILayout.Label($"UserId: {SocketIONetwork.User.ChatId}");
                        GUILayout.Label($"Color: {SocketIONetwork.User.Color}");
                        
                        for (int i = 0; i < SocketIONetwork.User.OtherIds.Count; i++)
                        {
                            GUILayout.Label($"Other ID {i}: {SocketIONetwork.User.OtherIds[i]}");   
                        }
                    }
                }


                DrawRoomDetails();
                DrawButtons();
            }

        }

        private void DrawRoomDetails()
        {
            // Room details

            if (RoomDetails)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (!SocketIONetwork.InRoom)
                    {
                        GUILayout.Label("Not in a room");

                    }
                    else
                    {
                        GUILayout.Label($"RoomId: {SocketIONetwork.CurrentRoom.Name}");
                        GUILayout.Label($"MaxPlayers: {SocketIONetwork.CurrentRoom.MaxPlayers}");
                        GUILayout.Label($"PlayerCount: {SocketIONetwork.CurrentRoom.PlayerCount}");

                        using (new GUILayout.VerticalScope())
                        {

                            for (var i = 0; i < SocketIONetwork.CurrentRoom.Players.Length; i++)
                            {
                                GUILayout.Label($"Player {i}: {SocketIONetwork.CurrentRoom.Players[i].Username}");
                            }
                        }
                    }
                }
            }
        }

        private void DrawButtons()
        {
            if (Buttons)
            {
                if (!SocketIONetwork.IsConnected)
                {

                    if (GUILayout.Button("Connect"))
                    {
                        SocketIONetwork.Connect();
                    }
                    return;
                }
                
                
                
                if (GUILayout.Button("Disconnect"))
                {
                    SocketIONetwork.Disconnect();
                }
                
                if (GUILayout.Button("List all rooms"))
                {
                    SocketIONetwork.GetAllRooms(100, true);
                }
                
                if (GUILayout.Button("List all users"))
                {
                    SocketIONetwork.ListIds(true);
                }
                    
                if (SocketIONetwork.InRoom)
                {
                    if (GUILayout.Button("Leave Room"))
                    {
                        SocketIONetwork.LeaveRoom();
                    }
                }
                else
                {
                    if (GUILayout.Button("Join Random Room"))
                    {
                        SocketIONetwork.JoinRandomRoom();
                    }
                }
                
                if (GUILayout.Button("Send Global Chat Message"))
                {
                    SocketIONetwork.SendGeneralChatMessage("Hello General Chat Message!");
                }

                if (SocketIONetwork.InRoom)
                {
                    if (GUILayout.Button("Send Room Message"))
                    {
                        SocketIONetwork.SendRoomChatMessage("Hello Room Chat Message!");
                    }
                }
                
            }
        }
    }
}
