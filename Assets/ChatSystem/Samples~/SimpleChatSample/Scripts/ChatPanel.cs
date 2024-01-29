using System;
using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.SettingsScript;
using Klem.SocketChat.ChatSystem.SimpleChatSample.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("SocketChat/SimpleChatSample/ChatPanel")]
    public class ChatPanel : MonoBehaviourSocketCallBacks
    {
        [FormerlySerializedAs("generalMessagesContainer")] [FormerlySerializedAs("messagesContainer")] [SerializeField] private ScrollRect generalMessagesView;
        [FormerlySerializedAs("roomMessagesContainer")] [SerializeField] private ScrollRect roomMessagesView;
        [SerializeField] private TMP_InputField messageInput;
        [FormerlySerializedAs("chatText")] [SerializeField] private TMP_Text generalChatText;
        [SerializeField] private TMP_Text roomChatText;
        [SerializeField] private TabGroup tabGroup;
        
        private readonly List<string> _messages = new List<string>();
        private readonly List<string> _roomMessages = new List<string>();
        private CanvasGroup _canvasGroup;
        
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            generalChatText.text = "";
            roomChatText.text = "";
            _messages.Clear();
            _roomMessages.Clear();
            
            _canvasGroup.Toggle(false);
            messageInput.onSubmit.AddListener(OnSubmitMessage);
            TMPTextLinkHandler.OnLinkClickedCallback += OnLinkClicked;
            tabGroup.OnTabChanged += OnTabChanged;
        }

        private void OnTabChanged(ETab tab)
        {
            switch (tab)
            {
                case ETab.General:
                    generalMessagesView.gameObject.SetActive(true);
                    roomMessagesView.gameObject.SetActive(false);
                    break;
                case ETab.Room:
                    generalMessagesView.gameObject.SetActive(false);
                    roomMessagesView.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tab), tab, null);
            }
        }

        #region SocketIONetwork Callbacks
        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            _canvasGroup.Toggle(true);
            messageInput.interactable = true;
            tabGroup.GeneralTabButton.Select();
            SetServerMessage($"Connected at {connection.GetDate().Hours}:{connection.GetDate().Minutes}\nChatId : {connection.ChatId}");
        }

        public override void OnDisconnecting(string reason)
        {
            SetServerMessage($"Disconnected from chat. Reason : {reason}");
            messageInput.interactable = false;
        }

        public override void OnError(string obj)
        {
            SetServerMessage($"Error : {obj}");
        }

        public override void OnGeneralChatMessage(ChatMessage message)
        {
            SetMessage(message, ETab.General);
        }

        public override void OnRoomChatMessage(ChatMessage message)
        {
            SetMessage(message, ETab.Room);
        }


        public override void OnChatInviteReceived(ChatInvite invite) { }

        public override void OnNewUserConnectedToMaster(SocketIOUser user)
        {
            SetServerMessage($"User <color=#{user.Color}><b>{user.Username}</b></color> connected to master");
        }


        public override void OnRoomLeft(Room room)
        {
            _roomMessages.Clear();
            roomChatText.text = "";
        }

        public override void OnRoomUserJoined(RoomAndUser andUser)
        {
            SetServerMessage($"User <color=#{andUser.User.Color}><b>{andUser.User.Username}</b></color> joined room {andUser.Room.Name}");
        }

        public override void OnUserLeftRoom(RoomAndUser andUser)
        {
            SetServerMessage($"User <color=#{andUser.User.Color}><b>{andUser.User.Username}</b></color> left room {andUser.Room.Name}");
        }
        #endregion

        private async void OnSubmitMessage(string arg0)
        {
            if (string.IsNullOrEmpty(arg0))
            {
                Debug.LogWarning("Message is empty");
                return;
            }

            switch (tabGroup.CurrentTab)
            {
                case ETab.General:
                    await SocketIONetwork.SendGeneralChatMessage(arg0);
                    break;
                case ETab.Room:
                    await SocketIONetwork.SendRoomChatMessage(arg0);
                    break;
            }
            
            messageInput.text = "";
        }
        
        private void SetServerMessage(string message)
        {
            string colorString = ColorUtility.ToHtmlStringRGB(ChatSettings.Instance.ServerMessageColor);
            SetMessage($"<color=#{colorString}><link=\"server\"><b>Server</b></link></color>", message, ETab.General);
            SetMessage( $"<color=#{colorString}><link=\"server\"><b>Server</b></link></color>", message, ETab.Room);
        }
        
        private void SetMessage(ChatMessage msg, ETab tab)
        {
            SetMessage($"<color=#{msg.Sender.Color}><link=\"{msg.Sender.ChatId}\"><b>{msg.Sender.Username} : </b></link></color>", msg.Message, tab);
        }

        private void SetMessage(string username, string message, ETab tab)
        {
            
            
            string msg = username + ": " + message;

            switch (tab)
            {
                case ETab.General:
                    generalChatText.text += "\n";
                    generalChatText.text += msg;
            
                    _messages.Add(msg);
            
                    generalMessagesView.content.ForceUpdateRectTransforms();
                    break;
                case ETab.Room:
                    roomChatText.text += "\n";
                    roomChatText.text += msg;
            
                    _roomMessages.Add(msg);
            
                    roomMessagesView.content.ForceUpdateRectTransforms();
                    break;
            }
            

        }

        private void OnLinkClicked(string linkId)
        {
            Debug.Log($"Link clicked {linkId}");
        }
    }
}
