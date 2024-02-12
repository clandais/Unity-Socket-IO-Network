---
uid: monobehavioursocketcallbacks
---
# MonoBehaviourSocketCallbacks

## Overview

`MonoBehaviourSocketCallbacks` is a class extending MonoBehaviour that provides a set of callback methods for handling socket events.

## Example Usage

Create a MonoBehaviour extending [MonoBehaviourSocketCallbacks](xref:Klem.SocketChat.ChatSystem.MonoBehaviourSocketCallBacks) and override the methods you want to handle.

```csharp

public class MyMonoBheaviour : MonoBehaviourSocketCallBacks
{
    // The first callbacks you may want to override are those defined in IConnectionCallbacks
    public override void OnConnectedToMaster(SocketServerConnection connection)
    {
        Debug.Log($"Connected at {connection.GetDate().Hours}:{connection.GetDate().Minutes}\nChatId : {connection.ChatId}");

        // Meanwhile, SocketIONetwork will have updated SocketIOUser

        Debug.Log($"My new chat id {SocketIONetwork.User.ChatId}")
    }

    // This MonoBehaviour will get notified of new users connected to the server
    public override void OnNewUserConnectedToMaster(SocketIOUser user)
    {
        Debug.Log($"New user connected: {user.Username}");
    }
}
```

MonoBehaviourSocketCallBacks implements, internally, the following interfaces:
- IConnectionCallbacks
  - `void OnConnectedToMaster(`[SocketServerConnection](xref:Klem.SocketChat.ChatSystem.DataClasses.SocketServerConnection)` connection);`
  - `void OnNewUserConnectedToMaster(`[SocketIOUser](xref:Klem.SocketChat.ChatSystem.DataClasses.SocketIOUser)` user);`
  - `void OnDisconnecting(string reason);`
  - `void OnDisconnected(string reason);`
  - `void OnReconnectAttempt(int attempt);`
  - `void OnPing(EventArgs e);`
  - `void OnPong(int latencyMs);`
  - `void OnReconnectFailed(EventArgs e);`
  - `void OnReconnected(int attempts);`
  - `void OnReconnectError(Exception e);`
- IErrorCallbacks
  - `void OnServerErrorMessage(string message);`
  - `void OnError(string error);`
- IRoomCallbacks
  - `void OnRoomListUpdate(`[Room](xref:Klem.SocketChat.ChatSystem.DataClasses.Room)`[] rooms);`
  - `void OnRoomCreated(`[Room](xref:Klem.SocketChat.ChatSystem.DataClasses.Room)` room);`
  - `void OnRoomJoined(`[Room](xref:Klem.SocketChat.ChatSystem.DataClasses.Room)` room);`
  - `void OnRoomUserJoined(`[RoomAndUser](xref:Klem.SocketChat.ChatSystem.DataClasses.RoomAndUser)` roomAndUser);`
  - `void OnRoomCreationFailed(string reason);`
  - `void OnRoomLeft(`[Room](xref:Klem.SocketChat.ChatSystem.DataClasses.Room)` room);`
  - `void OnRoomLeftByOtherUser(`[RoomAndUser](xref:Klem.SocketChat.ChatSystem.DataClasses.RoomAndUser)` room);`
- IChatMessageCallbacks
  - `void OnGeneralChatMessage(`[ChatMessage](xref:Klem.SocketChat.ChatSystem.DataClasses.ChatMessage)` message);`
  - `oid OnRoomChatMessage(`[ChatMessage](xref:Klem.SocketChat.ChatSystem.DataClasses.ChatMessage)` message);`

