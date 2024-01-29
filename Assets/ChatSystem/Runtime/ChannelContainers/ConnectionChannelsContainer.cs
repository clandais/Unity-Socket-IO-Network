using System;
using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using Klem.SocketChat.ChatSystem.Infra;
using Klem.SocketChat.ChatSystem.Interfaces;
using UnityEngine;

namespace Klem.SocketChat.ChatSystem.ChannelContainers
{
    internal class ConnectionChannelsContainer : List<IConnectionCallbacks>, IConnectionCallbacks
    {
        public MessageChannel<SocketServerConnection> ConnectionMessageChannel { get; } = new MessageChannel<SocketServerConnection>();
        public MessageChannel<string> DisconnectingMessageChannel { get; } = new MessageChannel<string>();
        public MessageChannel<int> ReconnectAttemptMessageChannel { get; } = new MessageChannel<int>();
        public MessageChannel<EventArgs> PingMessageChannel { get; } = new MessageChannel<EventArgs>();
        public MessageChannel<int> PongMessageChannel { get; } = new MessageChannel<int>();
        public MessageChannel<string> DisconnectMessageChannel { get; } = new MessageChannel<string>();
        public MessageChannel<Exception> ReconnectErrorMessageChannel { get; } = new MessageChannel<Exception>();
        public MessageChannel<EventArgs> ReconnectFailedMessageChannel { get; } = new MessageChannel<EventArgs>();
        public MessageChannel<int> ReconnectedMessageChannel { get; } = new MessageChannel<int>();
        
        public ConnectionChannelsContainer()
        {
            ConnectionMessageChannel.Subscribe(OnConnectedToMaster);
            DisconnectingMessageChannel.Subscribe(OnDisconnecting);
            DisconnectMessageChannel.Subscribe(OnDisconnected);
            ReconnectAttemptMessageChannel.Subscribe(OnReconnectAttempt);
            PingMessageChannel.Subscribe(OnPing);
            PongMessageChannel.Subscribe(OnPong);
            ReconnectErrorMessageChannel.Subscribe(OnReconnectError);
            ReconnectFailedMessageChannel.Subscribe(OnReconnectFailed);
            ReconnectedMessageChannel.Subscribe(OnReconnected);
        }

        public void OnConnectedToMaster(SocketServerConnection connection)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnConnectedToMaster(connection);
            }
        }

        public void OnDisconnecting(string reason)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnDisconnecting(reason);
            }
        }
        
        public void OnDisconnected(string reason)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnDisconnected(reason);
            }
        }

        public void OnReconnectAttempt(int attempt)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnReconnectAttempt(attempt);
            }
        }

        public void OnPing(EventArgs e)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnPing(e);
            }
        }

        public void OnPong(int latencyMs)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnPong(latencyMs);
            }
        }

        public void OnReconnectFailed(EventArgs e)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnReconnectFailed(e);
            }    
        }

        public void OnReconnected(int attempts)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnReconnected(attempts);
            }
        }

        public void OnReconnectError(Exception e)
        {
            foreach (IConnectionCallbacks callbacks in this)
            {
                callbacks.OnReconnectError(e);
            }
        }
        
    }
}
