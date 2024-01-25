using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.SettingsScript;
using Klem.SocketChat.ChatSystem.SimpleChatSample.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("ChatSystem/SimpleChatSample/NetworkInfoPanel")]
    public class NetworkInfoPanel : MonoBehaviourSocketCallBacks
    {
        [SerializeField] private Button disconnectButton;
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private Color infoColor; 
        
        private CanvasGroup _canvasGroup;
        
        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.Toggle(false);

            infoText.text = "";
            
            disconnectButton.onClick.AddListener(OnDisconnectClicked);
        }

        private void OnDestroy()
        {
            disconnectButton.onClick.RemoveAllListeners();
        }

        #region SocketIONetwork Callbacks
        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            _canvasGroup.Toggle(true);
            
            string colorString = ColorUtility.ToHtmlStringRGB(ChatSettings.Instance.ServerMessageColor);
            
            infoText.text = $"<color=#{colorString}>Connected at : </color>{connection.GetDate().Hours}:{connection.GetDate().Minutes}\nChatId : {connection.ChatId}";
            infoText.text += $"\n<color=#{colorString}>User name :</color> {SocketIONetwork.User.Username}";
            infoText.text += $"\n<color=#{colorString}>User SocketId :</color> {SocketIONetwork.User.ChatId}";
            infoText.text += $"\n<color=#{colorString}>User Color :</color> {SocketIONetwork.User.Color}";
            infoText.text += $"\n<color=#{colorString}>User PhotonId :</color> {SocketIONetwork.User.PhotonId}";
            
        }

        public override void OnDisconnecting(string reason)
        {
            _canvasGroup.Toggle(false);
            infoText.text = $"Disconnected from chat. Reason : {reason}";
        }
        #endregion
        
        
        private async void OnDisconnectClicked()
        {
            await SocketIONetwork.Disconnect();
        }
    }
}
