using Klem.SocketChat.ChatSystem;
using Klem.SocketChat.ChatSystem.DataClasses;
using Photon.Pun;
using UnityEngine;

namespace TestScripts
{
    public class ChatRoomCreation : MonoBehaviourSocketCallBacks
    {
        private PhotonRoomCreation _photonRoomCreation;

        private const int PhotonIdIndex = 0;

        private void Start()
        {
            _photonRoomCreation = GetComponent<PhotonRoomCreation>();
            _photonRoomCreation.OnPhotonConnectedToMaster.AddListener(OnPhotonConnectedToMaster);
            _photonRoomCreation.OnPhotonLeftRoom.AddListener(OnPhotonLeftRoom);
        }

        private void OnDestroy()
        {
            _photonRoomCreation.OnPhotonConnectedToMaster.RemoveListener(OnPhotonConnectedToMaster);
            _photonRoomCreation.OnPhotonLeftRoom.RemoveListener(OnPhotonLeftRoom);
        }

        private Room _room;


        /// <summary>
        ///     We have joined a SocketIO room, now we need to connect to Photon
        /// </summary>
        /// <param name="obj"></param>
        public override void OnRoomJoined(Room obj)
        {
            _room = obj;
            if (!PhotonNetwork.IsConnected)
                _photonRoomCreation.ConnectToPhoton();
        }


        private async void OnPhotonLeftRoom()
        {
            await SocketIONetwork.LeaveRoom();
        }

        /// <summary>
        ///     We have left a SocketIO room, now we need to disconnect from Photon
        /// </summary>
        /// <param name="room"></param>
        public override void OnRoomLeft(Room room)
        {
            _photonRoomCreation.Disconnect();
        }


        /// <summary>
        ///     Now that we are connected to Photon, we can create or join a room
        /// </summary>
        /// <param name="photonId"></param>
        private async void OnPhotonConnectedToMaster(string photonId)
        {
            Debug.Log($"Connected to Photon, with id {photonId}", gameObject);

            SocketIONetwork.User.OtherIds.Insert(PhotonIdIndex, photonId);
            SocketIONetwork.UpdateUser();

            Debug.Log($"Our room has : {_room.PlayerCount} players", gameObject);

            // We are the first player in the room, we need to create a room in Photon
            if (_room.PlayerCount == 1)
            {
                _photonRoomCreation.CreatedRoom(_room);
            }
            // We are not the first player in the room, we need to join a room in Photon
            else if (_room.PlayerCount > 1)
            {
                _photonRoomCreation.JoinRoom(_room);
            }
            else
            {
                Debug.LogError("Room has no players", gameObject);
            }
        }

        public override void OnDisconnecting(string reason)
        {
            _photonRoomCreation.Disconnect();
        }
    }
}
