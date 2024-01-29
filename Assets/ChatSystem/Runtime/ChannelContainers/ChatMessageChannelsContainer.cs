using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.Infra;
using Klem.SocketChat.ChatSystem.Interfaces;

namespace Klem.SocketChat.ChatSystem.ChannelContainers
{
    internal class ChatMessageChannelsContainer: List<IChatMessageCallbacks>, IChatMessageCallbacks
    {
        public MessageChannel<ChatMessage> GeneralChatMessageChannel { get; } = new MessageChannel<ChatMessage>();
        public MessageChannel<ChatMessage> RoomChatMessageChannel { get; } = new MessageChannel<ChatMessage>();
        
        public ChatMessageChannelsContainer()
        {
            GeneralChatMessageChannel.Subscribe(OnGeneralChatMessage);
            RoomChatMessageChannel.Subscribe(OnRoomChatMessage);
        }
        
        public void OnGeneralChatMessage(ChatMessage message)
        {
            foreach (IChatMessageCallbacks messageCallbacks in this)
            {
                messageCallbacks.OnGeneralChatMessage(message);
            }
        }

        public void OnRoomChatMessage(ChatMessage message)
        {
            foreach (IChatMessageCallbacks messageCallbacks in this)
            {
                messageCallbacks.OnRoomChatMessage(message);
            }
        }
    }
}
