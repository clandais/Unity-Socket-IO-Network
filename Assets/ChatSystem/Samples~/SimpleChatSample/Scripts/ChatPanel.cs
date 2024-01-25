using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.SettingsScript;
using Klem.SocketChat.ChatSystem.SimpleChatSample.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("SocketChat/SimpleChatSample/ChatPanel")]
    public class ChatPanel : MonoBehaviourSocketCallBacks
    {
        [SerializeField] private ScrollRect messagesContainer;
        [SerializeField] private TMP_InputField messageInput;
        [SerializeField] private TMP_Text chatText;

        private readonly List<string> _messages = new List<string>();
        
        private void Start()
        {
            chatText.text = "";
            messagesContainer.gameObject.SetActive(false);
            messageInput.gameObject.SetActive(false);
            messageInput.onSubmit.AddListener(OnSubmitMessage);
            TMPTextLinkHandler.OnLinkClickedCallback += OnLinkClicked;
        }

        #region SocketIONetwork Callbacks
        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            messagesContainer.gameObject.SetActive(true);
            messageInput.gameObject.SetActive(true);
            messageInput.interactable = true;
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

        public override void OnChatMessage(ChatMessage message)
        {
            SetMessage(message);
        }

        public override void OnChatInviteReceived(ChatInvite invite) { }

        public override void OnNewUserConnectedToMaster(SocketIOUser user)
        {
            SetServerMessage($"User <color=#{user.Color}><b>{user.Username}</b></color> connected to master");
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

            await SocketIONetwork.SendChatMessage(arg0);
            messageInput.text = "";
        }
        
        private void SetServerMessage(string message)
        {
            string colorString = ColorUtility.ToHtmlStringRGB(ChatSettings.Instance.ServerMessageColor);
            SetMessage($"<color=#{colorString}><link=\"server\"><b>Server</b></link></color>", message);
        }
        
        private void SetMessage(ChatMessage msg)
        {
            SetMessage($"<color=#{msg.Sender.Color}><link=\"{msg.Sender.ChatId}\"><b>{msg.Sender.Username}</b></link>", msg.Message);
        }
        
        private void SetMessage(string username, string message)
        {
            string msg = username + ": " + message;
            
            chatText.text += "\n";
            chatText.text += msg;
            
            _messages.Add(msg);
            
            messagesContainer.content.ForceUpdateRectTransforms();
        }

        private void OnLinkClicked(string linkId)
        {
            Debug.Log($"Link clicked {linkId}");
        }
    }
}
