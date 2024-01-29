using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.SimpleChatSample.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    public class MultiplayerMenu : MonoBehaviourSocketCallBacks
    {
        #region CanvasGroup
        [Header("CanvasGroups")] [SerializeField]
        private CanvasGroup multiplayerPanel;
        [SerializeField] private CanvasGroup roomCreationPanel;
        [SerializeField] private CanvasGroup roomListPanel;
        #endregion
        
        #region MultiplayerPanel
        
        [Space(10)] [Header("MultiplayerPanel")]
        
        [SerializeField] private Button roomListButton;
        [SerializeField] private Button roomCreationButton;
        #endregion

        #region RoomCreationPanel
        
        [Space(10)] [Header("RoomCreationPanel")]
        
        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private TMP_InputField roomPlayerCountInput;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button cancelRoomButton;
        
        #endregion
        
        #region RoomListPanel
        
        [Space(10)] [Header("RoomListPanel")]
        
        [SerializeField] private ScrollRect roomListScrollView;

        [SerializeField] private Button backToMultiplayerButton;
        
        #endregion

        #region Private Fields
        private CanvasGroup _canvasGroup;

        private const int RoomPlayerDefaultCount = 2;
        private int _roomPlayerCount = RoomPlayerDefaultCount;
        private string _roomName = "";
        private Room[] _rooms;
        private List<RoomDetails> _roomDetails = new List<RoomDetails>();
        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.Toggle(false);
            multiplayerPanel.Toggle(true);
            roomListPanel.Toggle(false);
            roomCreationPanel.Toggle(false);
        }

        private void Start()
        {
            
            roomListButton.onClick.AddListener(OnRoomListClicked);
            roomCreationButton.onClick.AddListener(OnRoomCreationClicked);
            
            roomNameInput.onEndEdit.AddListener(OnRoomNameChanged);
            roomPlayerCountInput.onEndEdit.AddListener(OnRoomPlayerCountChanged);
            
            createRoomButton.onClick.AddListener(OnCreateRoomClicked);
            cancelRoomButton.onClick.AddListener(OnCancelButtonClicked);
            
            backToMultiplayerButton.onClick.AddListener(OnBackToMultiplayerClicked);
        }


        private void OnDestroy()
        {
            roomListButton.onClick.RemoveAllListeners();
            roomCreationButton.onClick.RemoveAllListeners();
            
            roomNameInput.onEndEdit.RemoveAllListeners();
            roomPlayerCountInput.onEndEdit.RemoveAllListeners();
            
            createRoomButton.onClick.RemoveAllListeners();
            cancelRoomButton.onClick.RemoveAllListeners();
            
            backToMultiplayerButton.onClick.RemoveAllListeners();
            
        }
        #endregion


        #region Input Callbacks
        private void OnRoomPlayerCountChanged(string arg0)
        {
            
            Debug.Log($"OnRoomPlayerCountChanged : {arg0}");
            
            bool parseSuccess = int.TryParse(arg0, out int result);

            if ( !parseSuccess || result < 2 || result > 8)
            {
                _roomPlayerCount = RoomPlayerDefaultCount;
            }
            else
            {
                _roomPlayerCount = result;
            }
            
            roomPlayerCountInput.text = _roomPlayerCount.ToString();
            
            ValidateRoomInput();
        }

        private void OnRoomNameChanged(string arg0)
        {
            
            Debug.Log($"OnRoomNameChanged : {arg0}");
            
            if (string.IsNullOrEmpty(arg0))
            {
                roomNameInput.text = $"Room-{SocketIONetwork.User.ChatId}";
            }
            else
            {
                _roomName = arg0;
            }
            
            ValidateRoomInput();
        }
        #endregion


        #region MonoBehaviourSocketCallBacks Callbacks
        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            _canvasGroup.Toggle(true);
        }

        public override void OnDisconnecting(string reason)
        {
            base.OnDisconnecting(reason);

            _canvasGroup.Toggle(false);
        }

        public override void OnRoomJoined(Room room)
        {
            
        }
        #endregion

        #region Button Callbacks
        private void OnRoomCreationClicked()
        {
            multiplayerPanel.Toggle(false);
            roomCreationPanel.Toggle(true);
            ValidateRoomInput();
        }

        private async void OnRoomListClicked()
        {
            multiplayerPanel.Toggle(false);
            roomListPanel.Toggle(true);
            await roomListPanel.GetComponent<RoomListPanel>().Refresh();
        }

        private void OnCancelButtonClicked()
        {

            roomPlayerCountInput.text = "";
            roomNameInput.text = "";
            _roomPlayerCount = RoomPlayerDefaultCount;
            roomCreationPanel.Toggle(false);
            multiplayerPanel.Toggle(true);
        }

        private async void OnCreateRoomClicked()
        {
            if (!ValidateRoomInput())
            {
                Debug.LogError("Invalid Room Input");
                return;
            }
             
            Room playerRoom = new Room(_roomName, _roomPlayerCount);

            SocketIONetwork.CreateRoom(playerRoom);
            
            roomCreationPanel.Toggle(false);
            multiplayerPanel.Toggle(true);
        }
        
        
        private void OnBackToMultiplayerClicked()
        {
            roomListPanel.Toggle(false);
            multiplayerPanel.Toggle(true);
        }
        
        #endregion

        #region Private Methods
        
        private bool ValidateRoomInput()
        {
            bool isValid = !string.IsNullOrEmpty(_roomName);

            if (_roomPlayerCount < 2 || _roomPlayerCount > 8)
            {
                isValid = false;
            }
            
            createRoomButton.interactable = isValid;
            return isValid;
        }
        #endregion
    }
}
