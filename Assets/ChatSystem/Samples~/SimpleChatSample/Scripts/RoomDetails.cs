using System.Linq;
using Klem.SocketChat.ChatSystem.DataClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    [AddComponentMenu("SocketChat/SimpleChatSample/RoomDetails")]
    public class RoomDetails : MonoBehaviourSocketCallBacks
    {
        [SerializeField] private TMP_Text roomDetailsText;
        [SerializeField] private Button joinRoomButton;

        private Room _room;


        private void Start()
        {
            joinRoomButton.onClick.AddListener(JoinRoom);
        }

        private void OnDestroy()
        {
            joinRoomButton.onClick.RemoveListener(JoinRoom);
        }

        public void SetRoom(Room room)
        {
            _room = room;
            roomDetailsText.text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})";
            
            if (room.PlayerCount >= room.MaxPlayers)
            {
                joinRoomButton.interactable = false;
            }
            
            if (room.PlayerCount == 0)
            {
                Destroy(gameObject);
                return;
            }

            foreach (SocketIOUser socketIOUser in _room.Players)
            {
                if (socketIOUser.ChatId == SocketIONetwork.User.ChatId)
                {
                    joinRoomButton.interactable = false;
                    break;
                }
            }
        }

        private void JoinRoom()
        {
            SocketIONetwork.JoinRoom(_room);
        }

        public override async void OnRoomUserJoined(RoomAndUser andUser)
        {


            if (andUser.User.ChatId == SocketIONetwork.User.ChatId)
            {
                SocketIONetwork.User.RoomId = andUser.Room.Name;
                await SocketIONetwork.UpdateUser(SocketIONetwork.User);
                joinRoomButton.interactable = false;
            }
            
            if (andUser.Room.Name == _room.Name)
            {
                _room = andUser.Room;
                roomDetailsText.text = $"{_room.Name} ({_room.PlayerCount}/{_room.MaxPlayers})";
            }
        }
    }
}
