using System.Collections.Generic;

namespace Klem.SocketChat.ChatSystem.DataClasses
{
    [System.Serializable]
    public class SocketIOUser
    {
        public string ChatId;
        public string Username;
        public List<string> OtherIds = new List<string>();
        public string Color;
        public string RoomId;
    }
}
