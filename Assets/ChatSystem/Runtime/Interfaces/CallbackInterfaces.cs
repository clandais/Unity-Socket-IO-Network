using System;
using Klem.SocketChat.ChatSystem.DataClasses;

namespace Klem.SocketChat.ChatSystem.Interfaces
{
    
    internal interface IConnectionCallbacks
    {
        void OnConnectedToMaster(SocketServerConnection connection);
        void OnNewUserConnectedToMaster(SocketIOUser user);
        void OnDisconnecting(string reason);
        void OnDisconnected(string reason);
        void OnReconnectAttempt(int attempt);
        void OnPing(EventArgs e);
        void OnPong(int latencyMs);
        void OnReconnectFailed(EventArgs e);
        void OnReconnected(int attempts);
        void OnReconnectError(Exception e);
    }
    
    internal interface IErrorCallbacks
    {
        void OnServerErrorMessage(string message);
        void OnError(string error);
    }
    
    internal interface IRoomCallbacks
    {
        void OnRoomListUpdate(Room[] rooms);
        void OnRoomCreated(Room room);
        void OnRoomJoined(Room room);
        void OnRoomUserJoined(RoomAndUser roomAndUser);
        void OnRoomCreationFailed(string reason);
        void OnRoomLeft(Room room);
        void OnRoomLeftByOtherUser(RoomAndUser room);
    }
    
    internal interface IChatMessageCallbacks
    {
        void OnGeneralChatMessage(ChatMessage message);
        void OnRoomChatMessage(ChatMessage message);
    }
}

