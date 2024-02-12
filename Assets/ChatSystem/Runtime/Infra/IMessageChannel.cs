using System;

namespace Klem.SocketChat.ChatSystem.Infra
{
    internal interface IPublisher<in T>
    {
        void Publish(T message);
    }
    
    internal interface ISubscriber<out T>
    {
        IDisposable Subscribe(Action<T> handler);
        void Unsubscribe(Action<T> handler);
    }   
    
    internal interface IMessageChannel<T> : IPublisher<T>, ISubscriber<T>, IDisposable
    {
        bool IsDisposed { get; }
    }
    
    
}
