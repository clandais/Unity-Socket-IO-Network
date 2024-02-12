using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.SimpleChatSample.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("SocketChat/SimpleChatSample/RoomListPanel")]
    public class RoomListPanel : MonoBehaviourSocketCallBacks
    {
        [SerializeField] private ScrollRect roomListScrollView;
        [SerializeField] private RoomDetails roomDetailsPrefab;
        
        [SerializeField] private GameObject spinnerGo;
        
        private CanvasGroup _canvasGroup;

        private Room[] _rooms;
        private List<RoomDetails> _roomDetails = new List<RoomDetails>();
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _canvasGroup.Toggle(false);
            spinnerGo.SetActive(false);
        }
        
        public async Task Refresh()
        {
            spinnerGo.SetActive(true);
            
            StartCoroutine(WaitForSpinnerActive());
            
            _rooms = await SocketIONetwork.GetAllRooms();
            FormatRoomList();
            
            spinnerGo.SetActive(false);
        }

        public override void OnRoomListUpdate(Room[] rooms)
        {
            Debug.Log("OnRoomListUpdate");
            
            _rooms = rooms;
            FormatRoomList();
        }

        private IEnumerator WaitForSpinnerActive()
        {
            while (spinnerGo.activeSelf)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        private void FormatRoomList()
        {
            var text = "";

            for (int i = 0; i < _roomDetails.Count; i++)
            {
                if (_roomDetails[i] != null)
                    Destroy(_roomDetails[i].gameObject);
            }
            _roomDetails.Clear();

            if (_rooms == null || _rooms.Length == 0)
            {
                roomListScrollView.content.ForceUpdateRectTransforms();
                return;
            }
            
            
            for (int i = 0; i < _rooms.Length; i++)
            {
                var instance = Instantiate(roomDetailsPrefab, roomListScrollView.content);
                instance.SetRoom(_rooms[i]);
                _roomDetails.Add(instance);
            }
            
            roomListScrollView.content.ForceUpdateRectTransforms();
        }


    }
}
