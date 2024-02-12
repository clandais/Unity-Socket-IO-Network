using Klem.SocketChat.ChatSystem;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using Room = Klem.SocketChat.ChatSystem.DataClasses.Room;

namespace TestScripts
{
    public class PhotonRoomCreation : MonoBehaviourPunCallbacks
    {
        public UnityEvent<string> OnPhotonConnectedToMaster;
        public UnityEvent OnPhotonLeftRoom;


        public void ConnectToPhoton()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Already connected to Photon", gameObject);
                return;
            }

            PhotonNetwork.ConnectUsingSettings();
        }


        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnConnectedToMaster()
        {
            OnPhotonConnectedToMaster.Invoke(PhotonNetwork.LocalPlayer.UserId);
            PhotonNetwork.NickName = SocketIONetwork.User.Username;
        }

        public override void OnLeftRoom()
        {
            OnPhotonLeftRoom.Invoke();
        }

        public void CreatedRoom(Room room)
        {
            PhotonNetwork.CreateRoom(room.Name, new RoomOptions
            {
                MaxPlayers = room.MaxPlayers,
            });
        }


        public void JoinRoom(Room room)
        {
            PhotonNetwork.JoinRoom(room.Name);
        }
    }
}
