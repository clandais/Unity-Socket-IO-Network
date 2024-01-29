using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("SocketChat/SimpleChatSample/ErrorPanel")]
    public class ErrorPanel : MonoBehaviourSocketCallBacks
    {
        [SerializeField] private TMP_Text errorText;
        
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float fadeDelay = 1f;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }
        
        #region SocketIONetwork Callbacks
        public override void OnError(string obj)
        {
            errorText.text = obj;
            StartCoroutine( FadeInOut() );
        }

        public override void OnServerErrorMessage(string msg)
        {
            errorText.text = msg;
            StartCoroutine( FadeInOut() );
        }

        public override void OnReconnectFailed(EventArgs e)
        {
            errorText.text = "Reconnect failed" + e;
            StartCoroutine( FadeInOut() );
        }

        public override void OnReconnectError(Exception e)
        {
            errorText.text = "Reconnect error" + e;
            StartCoroutine( FadeInOut() );
        }
        #endregion
        
        private IEnumerator FadeInOut()
        {
            _canvasGroup.alpha = 1f;
            
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }
            
            yield return new WaitForSeconds(fadeDelay);
            
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }
        }
    }
}
