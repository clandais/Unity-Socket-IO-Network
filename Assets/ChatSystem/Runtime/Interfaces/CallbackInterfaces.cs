using System;
using Klem.SocketChat.ChatSystem.DataClasses;

namespace Klem.SocketChat.ChatSystem.Interfaces
{

    public interface ICallbacksContainer
    {
        void AddCallbackTarget(MonoBehaviourSocketCallBacks target);
        void RemoveCallbackTarget(MonoBehaviourSocketCallBacks target);
    }
    
    public interface IConnectionCallbacks
    {
        void OnConnectedToMaster(SocketServerConnection connection);
        void OnDisconnecting(string reason);
        void OnDisconnected(string reason);
        void OnReconnectAttempt(int attempt);
        void OnPing(EventArgs e);
        void OnPong(int latencyMs);
        void OnReconnectFailed(EventArgs e);
        void OnReconnected(int attempts);
        void OnReconnectError(Exception e);
    }
    
    public interface IErrorCallbacks
    {
        void OnServerErrorMessage(string message);
        void OnError(string error);
    }
    
    public interface IChatCallbacks
    {
        
    }
    
    public interface IRoomCallbacks
    {
        void OnRoomListUpdate(Room[] rooms);
        void OnRoomCreated(Room room);
        void OnRoomJoined(Room room);
        void OnRoomUserJoined(RoomAndUser roomAndUser);
        void OnRoomCreationFailed(string reason);
        void OnRoomLeft(Room room);
        void OnRoomLeftByOtherUser(RoomAndUser room);
    }
    
    public interface IChatMessageCallbacks
    {
        void OnGeneralChatMessage(ChatMessage message);
        void OnRoomChatMessage(ChatMessage message);
    }
}

