using System.Collections;
using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using TMPro;
using UnityEngine;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("SocketChat/SimpleChatSample/InfoPanel")]
    public class InfoPanel : MonoBehaviourSocketCallBacks
    {
        private Queue<string> _infoQueue = new Queue<string>();
        
        [SerializeField] private TMP_Text infoText;
        
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float fadeDelay = 1f;
        
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;

            StartCoroutine(FadeInOut());
        }
        
        #region SocketIONetwork Callbacks
        public override void OnDisconnecting(string reason)
        {
            _infoQueue.Enqueue($"[OnDisconnecting] Disconnecting from chat. Reason : {reason}");
        }

        public override void OnDisconnected(string reason)
        {
            _infoQueue.Enqueue($"[OnDisconnected] Disconnected from chat. Reason : {reason}");
        }


        public override void OnReconnectAttempt(int attempt)
        {
            _infoQueue.Enqueue($"Reconnect attempt : {attempt}");
        }

        public override void OnRoomCreated(Room obj)
        {
            _infoQueue.Enqueue($"Room created : {obj.Name}");
        }


        public override void OnRoomJoined(Room room)
        {
            _infoQueue.Enqueue($"Joined room : {room.Name}");
        }

        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            _infoQueue.Enqueue($"Connected at : {connection.GetDate().Hours}:{connection.GetDate().Minutes}\nChatId : {connection.ChatId}");
        }

        public override void OnReconnected(int attempts)
        {
            _infoQueue.Enqueue($"Reconnected after {attempts} attempts");
        }

        public override void OnRoomListUpdate(Room[] rooms)
        {
            _infoQueue.Enqueue($"Room list updated : {rooms.Length} rooms");
        }

        public override void OnRoomUserJoined(RoomAndUser andUser)
        {
            _infoQueue.Enqueue($"User {andUser.User.Username} joined room {andUser.Room.Name}");
        }
        

        public override void OnNewUserConnectedToMaster(SocketIOUser user)
        {
            _infoQueue.Enqueue($"New user connected to master : {user.Username}");
        }
        
        public override void OnRoomLeft(Room room)
        {
            _infoQueue.Enqueue($"Left room : {room.Name}");
        }

        public override void OnRoomLeftByOtherUser(RoomAndUser room)
        {
            _infoQueue.Enqueue($"Left room by other user : {room.User.Username}");
        }
        
        
        #endregion
        
        private IEnumerator FadeInOut()
        {

            while (Application.isPlaying)
            {
                if (_infoQueue.Count > 0)
                {
                    
                    infoText.text = _infoQueue.Dequeue();
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
                else
                {
                    yield return null;
                }
                
            }
        }
    }
}
