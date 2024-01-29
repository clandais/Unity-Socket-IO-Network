using Klem.SocketChat.ChatSystem.DataClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    
    public class InvitePanel : MonoBehaviour
    {
        public enum ResultType
        {
            Confirm,
            Cancel,
        }
        
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private Button inviteButton;
        [SerializeField] private Button closeButton;

        private CanvasGroup _canvasGroup;
        
        private bool _hasAction = false;
        
        public bool HasAction => _hasAction;
        
        public ResultType Result { get; private set; }

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            inviteButton.onClick.AddListener(() =>
            {
                _hasAction = true;
                Result = ResultType.Confirm;
            });
            
            closeButton.onClick.AddListener(() =>
            {
                _hasAction = true;
                Result = ResultType.Cancel;
            });
        }


        public void Show(SocketIOUser user)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            usernameText.text = user.Username;
        }
        
        public void Hide()
        {
            _hasAction = false;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            inviteButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
