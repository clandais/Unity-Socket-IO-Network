using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.SimpleChatSample.Utils;

#if UNITY_EDITOR
using ParrelSync;
#endif

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("ChatSystem/SimpleChatSample/ChatBoot")]
    public class ChatBoot : MonoBehaviourSocketCallBacks
    {
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private Button connectButton;
        [SerializeField] private Button disconnectButton;

        private CanvasGroup _canvasGroup;
        
        private void Start()
        {
            
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.Toggle(true);
            
            usernameInput.onValueChanged.AddListener(OnValidateUsername);
            connectButton.onClick.AddListener(OnConnectClicked);
            disconnectButton.onClick.AddListener(OnDisconnectClicked);

#if UNITY_EDITOR
            if (ClonesManager.IsClone())
            {
                usernameInput.text = ClonesManager.GetArgument();
                connectButton.interactable = true;
            }
            else
            {
                if (PlayerPrefs.HasKey("username"))
                {
                    usernameInput.text = PlayerPrefs.GetString("username");
                    connectButton.interactable = true;
                }
                else
                {
                    connectButton.interactable = false;
                }
            }
#else
            if (PlayerPrefs.HasKey("username"))
            {
                usernameInput.text = PlayerPrefs.GetString("username");
                connectButton.interactable = true;
            }
            else
            {
                connectButton.interactable = false;
            }
#endif
        }


        #region SocketIONetwork Callbacks
        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            _canvasGroup.Toggle(false);
            connectButton.interactable = false;
            disconnectButton.interactable = true;
        }

        public override void OnDisconnecting(string reason)
        {
            _canvasGroup.Toggle(true);
            connectButton.interactable = true;
            disconnectButton.interactable = false;
        }
        #endregion
        
        private async void OnConnectClicked()
        {
            if (string.IsNullOrEmpty(usernameInput.text))
            {
                Debug.LogWarning("Username is empty");
                return;
            }

#if UNITY_EDITOR

            if (!ClonesManager.IsClone())
            {
                PlayerPrefs.SetString("username", usernameInput.text);
                PlayerPrefs.Save();
            }
#else
            PlayerPrefs.SetString("username", usernameInput.text);
            PlayerPrefs.Save();
#endif
            connectButton.interactable = false;
            usernameInput.interactable = false;


            SocketIONetwork.User.Username = usernameInput.text;

            await SocketIONetwork.Connect();

            disconnectButton.interactable = true;
        }
        

        private void OnDisconnectClicked()
        { 
            SocketIONetwork.Disconnect();
            disconnectButton.interactable = false;
            connectButton.interactable = true;
        }

        private void OnValidateUsername(string text)
        {
            connectButton.interactable = !string.IsNullOrEmpty(text);
        }


        private void OnApplicationQuit()
        {
           SocketIONetwork.Disconnect();
        }
    }
}
