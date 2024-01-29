using System;
using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.Infra;
using Klem.SocketChat.ChatSystem.Interfaces;

namespace Klem.SocketChat.ChatSystem.ChannelContainers
{
    internal class RoomChannelsContainer : List<IRoomCallbacks>, IRoomCallbacks
    {
        public MessageChannel<Room[]> RoomListUpdateMessageChannel { get; } = new MessageChannel<Room[]>();
        public MessageChannel<Room> RoomCreatedMessageChannel { get; } = new MessageChannel<Room>();
        public MessageChannel<Room> RoomJoinedMessageChannel { get; } = new MessageChannel<Room>();
        public MessageChannel<RoomAndUser> RoomUserJoinedMessageChannel { get; } = new MessageChannel<RoomAndUser>();
        public MessageChannel<string> RoomCreationFailedMessageChannel { get; } = new MessageChannel<string>();
        
        public MessageChannel<Room> RoomLeftMessageChannel { get; } = new MessageChannel<Room>();
        public MessageChannel<RoomAndUser> RoomLeftByOtherUserMessageChannel { get; } = new MessageChannel<RoomAndUser>();
        
        public RoomChannelsContainer()
        {
            RoomListUpdateMessageChannel.Subscribe(OnRoomListUpdate);
            RoomCreatedMessageChannel.Subscribe(OnRoomCreated);
            RoomJoinedMessageChannel.Subscribe(OnRoomJoined);
            RoomUserJoinedMessageChannel.Subscribe(OnRoomUserJoined);
            RoomCreationFailedMessageChannel.Subscribe(OnRoomCreationFailed);
            RoomLeftMessageChannel.Subscribe(OnRoomLeft);
            RoomLeftByOtherUserMessageChannel.Subscribe(OnRoomLeftByOtherUser);
        }
        
        public void OnRoomListUpdate(Room[] rooms)
        {

            for (int i = this.Count - 1; i >= 0; i--)
            {
                this[i].OnRoomListUpdate(rooms);
            }
            
        }

        public void OnRoomCreated(Room room)
        {
            foreach (IRoomCallbacks callbacks in this)
            {
                callbacks.OnRoomCreated(room);
            }
        }

        public void OnRoomJoined(Room room)
        {
            foreach (IRoomCallbacks callbacks in this)
            {
                callbacks.OnRoomJoined(room);
            }
        }

        public void OnRoomUserJoined(RoomAndUser roomAndUser)
        {
            foreach (IRoomCallbacks callbacks in this)
            {
                callbacks.OnRoomUserJoined(roomAndUser);
            }
        }

        public void OnRoomCreationFailed(string reason)
        {
            foreach (IRoomCallbacks callbacks in this)
            {
                callbacks.OnRoomCreationFailed(reason);
            }
        }

        public void OnRoomLeft(Room room)
        {
            foreach (IRoomCallbacks callbacks in this)
            {
                callbacks.OnRoomLeft(room);
            }
        }

        public void OnRoomLeftByOtherUser(RoomAndUser room)
        {
            foreach (IRoomCallbacks callbacks in this)
            {
                callbacks.OnRoomLeftByOtherUser(room);
            }
        }
    }
}
