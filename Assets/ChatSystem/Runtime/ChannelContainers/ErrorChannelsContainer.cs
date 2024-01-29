using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.Infra;
using Klem.SocketChat.ChatSystem.Interfaces;

namespace Klem.SocketChat.ChatSystem.ChannelContainers
{
    internal class ErrorChannelsContainer : List<IErrorCallbacks>, IErrorCallbacks
    {
        public MessageChannel<string> ServerErrorMessageChannel { get; } = new MessageChannel<string>();
        public MessageChannel<string> ErrorMessageChannel { get; } = new MessageChannel<string>();
        
        public ErrorChannelsContainer()
        {
            ServerErrorMessageChannel.Subscribe(OnServerErrorMessage);
            ErrorMessageChannel.Subscribe(OnError);
        }
        
        public void OnServerErrorMessage(string message)
        {
            foreach (IErrorCallbacks callbacks in this)
            {
                callbacks.OnServerErrorMessage(message);
            }
        }

        public void OnError(string error)
        {
            foreach (IErrorCallbacks callbacks in this)
            {
                callbacks.OnError(error);
            }
        }
    }
}
