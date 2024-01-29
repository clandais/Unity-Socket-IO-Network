namespace Klem.SocketChat.ChatSystem.DataClasses
{
    [System.Serializable]
    public class ChatInvite
    {
        public SocketIOUser From;
        public SocketIOUser To;
    }
}
