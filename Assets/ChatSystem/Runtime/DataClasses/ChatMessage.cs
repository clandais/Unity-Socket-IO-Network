namespace Klem.SocketChat.ChatSystem.DataClasses
{
    [System.Serializable]
    public class ChatMessage
    {
        public SocketIOUser Sender;
        public string Message;
    }
}
