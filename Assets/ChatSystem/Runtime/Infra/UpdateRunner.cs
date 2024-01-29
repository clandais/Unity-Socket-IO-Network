using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klem.SocketChat.ChatSystem.Infra
{
    
    public class UpdateRunner : MonoBehaviour
    {
        
        private readonly Queue<Action> _pendingHandlers = new Queue<Action>();
        private readonly HashSet<Action> _subscribers = new HashSet<Action>();
        
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            _pendingHandlers.Clear();
            _subscribers.Clear();
        }

        private void Update()
        {
            while (_pendingHandlers.Count > 0)
            {
                _pendingHandlers.Dequeue()?.Invoke();
            }

            foreach (Action subscriber in _subscribers)
            {
                subscriber?.Invoke();
            }
            
            Debug.Log($"UpdateRunner : Update contains {_subscribers.Count} subscribers");
        }

        public void Subscribe(Action onUpdate)
        {
            if (_subscribers.Contains(onUpdate))
            {
                return;
            }
            _pendingHandlers.Enqueue(() =>
            {
                if(!_subscribers.Add(onUpdate))
                {
                    Debug.LogError("Failed to add callback container");
                }
            } );
        }
        
        public void Unsubscribe(Action onUpdate)
        {
            if (!_subscribers.Contains(onUpdate))
            {
                return;
            }
            _pendingHandlers.Enqueue(() =>
            {
                if(!_subscribers.Remove(onUpdate))
                {
                    Debug.LogError("Failed to remove callback container");
                }
            } );
        }
        
    }
}
