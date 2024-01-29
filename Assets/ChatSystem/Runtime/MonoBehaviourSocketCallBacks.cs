using System;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.Interfaces;
using UnityEngine;

namespace Klem.SocketChat.ChatSystem
{
    public class MonoBehaviourSocketCallBacks : MonoBehaviour,
        IConnectionCallbacks,
        IErrorCallbacks,
        IRoomCallbacks,
        IChatMessageCallbacks
    {
        /// <summary>
        ///     When overriding this method, make sure to call base.OnEnable() to add the callback target.
        /// </summary>
        public virtual void OnEnable()
        {
            SocketIONetwork.AddCallbackTarget(this);
        }

        /// <summary>
        ///     When overriding this method, make sure to call base.OnDisable() to remove the callback target.
        /// </summary>
        public void OnDisable()
        {
            SocketIONetwork.RemoveCallbackTarget(this);
        }


        #region Connection Callbacks
        /// <summary>
        ///     Called when the client is connected to the server.
        /// </summary>
        /// <param name="connection">
        ///     The connection object containing the date of the connection and the chatId. See :
        ///     <see cref="SocketServerConnection" />
        /// </param>
        public virtual void OnConnectedToMaster(SocketServerConnection connection) { }

        /// <summary>
        ///     Called when the client is disconnecting from the server.
        /// </summary>
        /// <param name="reason">
        ///     The reason of the disconnection.
        /// </param>
        public virtual void OnDisconnecting(string reason) { }
        
        /// <summary>
        ///   Called when the client is disconnected from the server.
        /// </summary>
        /// <param name="reason"></param>
        public virtual void OnDisconnected(string reason) { }

        /// <summary>
        ///     Called when the client is trying to reconnect to the server.
        /// </summary>
        /// <param name="attempt">
        ///     The number of the attempt.
        /// </param>
        public virtual void OnReconnectAttempt(int attempt) { }

        /// <summary>
        ///     Called when the client sends a ping to the server.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnPing(EventArgs obj) { }

        /// <summary>
        ///     Called when the client receives a pong from the server.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnPong(int obj) { }
        #endregion

        /// <summary>
        ///     Called when an error is thrown by the server.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnError(string obj) { }


        /// <summary>
        ///     Called when a new user is connected to the master server.
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnNewUserConnectedToMaster(SocketIOUser user) { }



        /// <summary>
        ///     Called when a user leaves a room.
        /// </summary>
        /// <param name="andUser"></param>
        public virtual void OnUserLeftRoom(RoomAndUser andUser) { }

        public virtual void OnGetUser(SocketIOUser user) { }

        public virtual void OnChatInviteReceived(ChatInvite invite) { }

        /// <summary>
        ///     Called when the room list is updated on the server.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnRoomListChanged(Room[] obj) { }

        /// <summary>
        ///     Called when a room is created.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnRoomCreated(Room obj) { }

        /// <summary>
        ///     Called when a room creation failed.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnCreateRoomFailed(string obj) { }

        /// <summary>
        ///     Called on any server error message.
        /// </summary>
        /// <param name="msg"></param>
        public virtual void OnServerErrorMessage(string msg) { }

        /// <summary>
        ///     Called when the client fails to reconnect to the server.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnReconnectFailed(EventArgs e) { }

        /// <summary>
        ///     Called when the client reconnects to the server.
        /// </summary>
        /// <param name="attempts"></param>
        public virtual void OnReconnected(int attempts) { }

        /// <summary>
        ///     Called when the client receives an error upon reconnecting.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnReconnectError(Exception e) { }


        /// <summary>
        ///     Called when the client receives a room list update.
        /// </summary>
        /// <param name="rooms"></param>
        public virtual void OnRoomListUpdate(Room[] rooms) { }
        

        /// <summary>
        ///     Called when the client joins a room.
        /// </summary>
        /// <param name="room"></param>
        public virtual void OnRoomJoined(Room room) { }

        /// <summary>
        ///   Called when a room creation failed.
        /// </summary>
        /// <param name="reason"></param>
        public virtual void OnRoomCreationFailed(string reason) { }
        

        /// <summary>
        ///     Called when a user joins a room.
        /// </summary>
        /// <param name="andUser"></param>
        public virtual void OnRoomUserJoined(RoomAndUser andUser) { }
        
        
        /// <summary>
        ///   Called when the client leaves a room.
        /// </summary>
        /// <param name="room"></param>
        public virtual void OnRoomLeft(Room room) {}

        /// <summary>
        ///  Called when a user leaves a room.
        /// </summary>
        /// <param name="room"></param>
        public virtual void OnRoomLeftByOtherUser(RoomAndUser room) { }

        /// <summary>
        ///   Called when the client receives a general chat message.
        /// </summary>
        /// <param name="message">
        ///    The message object containing the sender and the message. See : <see cref="ChatMessage" />
        /// </param>
        public virtual void OnGeneralChatMessage(ChatMessage message) { }
        
        /// <summary>
        ///  Called when the client receives a room chat message.
        /// </summary>
        /// <param name="message">
        ///   The message object containing the sender and the message. See : <see cref="ChatMessage" />
        /// </param>
        public virtual void OnRoomChatMessage(ChatMessage message) { }
    }
}
