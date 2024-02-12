using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Klem.SocketChat.ChatSystem.Infra
{
    internal class MessageChannel<T> : IMessageChannel<T>
    {
        
        private readonly List<Action<T>> _handlers = new List<Action<T>>();
        
        private readonly Dictionary<Action<T>, bool> _pendingHandlers = new Dictionary<Action<T>, bool>();
        
        public bool IsDisposed { get; private set; }


        public void Publish(T message)
        {
            foreach (Action<T> handler in _pendingHandlers.Keys)
            {
                if (_pendingHandlers[handler])
                {
                    _handlers.Add(handler);
                }
                else
                {
                    _handlers.Remove(handler);
                }
            }
            
            _pendingHandlers.Clear();

            foreach (Action<T> handler in _handlers)
            {
                if (handler != null)
                {
                    handler.Invoke(message);
                }
            }
        }

        public IDisposable Subscribe(Action<T> handler)
        {
            Assert.IsTrue( !IsSubscribed(handler), "Attempting to subscribe with the same handler more than once");

            if (_pendingHandlers.ContainsKey(handler))
            {
                if (!_pendingHandlers[handler])
                {
                    _pendingHandlers.Remove(handler);
                }
            }
            else
            {
                _pendingHandlers[handler] = true;
            }
            
            var subscription = new DisposableSubscription<T>(this, handler);
            return subscription;
        }

        public void Unsubscribe(Action<T> handler)
        {
            if (IsSubscribed(handler))
            {
                if (_pendingHandlers.ContainsKey(handler))
                {
                    if (_pendingHandlers[handler])
                    {
                        _pendingHandlers.Remove(handler);
                    }
                }
                else
                {
                    _pendingHandlers[handler] = false;
                }
            }
        }

        
        private bool IsSubscribed(Action<T> handler)
        {
            bool isPendingRemoval = _pendingHandlers.ContainsKey(handler) && !_pendingHandlers[handler];
            bool isPendingAddition = _pendingHandlers.ContainsKey(handler) && _pendingHandlers[handler];
            return _handlers.Contains(handler) && ! isPendingRemoval || isPendingAddition;
        }
        
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                _handlers.Clear();
                _pendingHandlers.Clear();
            }
        }
    }
}
