using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPTextLinkHandler : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text _text;
        private Canvas _canvas;
        private Camera _camera;

        
        public delegate void OnLinkClicked(string linkId);
        public static event OnLinkClicked OnLinkClickedCallback;
        
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _canvas = GetComponentInParent<Canvas>();
            _camera = _canvas.worldCamera;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, _camera);
            if (linkIndex == -1)
                return;
            
            TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkIndex];
            OnLinkClickedCallback?.Invoke(linkInfo.GetLinkID());
        }
    }
}
