using System;

namespace Klem.SocketChat.ChatSystem.DataClasses
{
    [System.Serializable]
    public class Room
    {
        public string Name;
        public int MaxPlayers;
        public int PlayerCount;
        public SocketIOUser[] Players;
        
        public Room(string name, int maxPlayers, int playerCount = 0, SocketIOUser[] players =  null )
        {
            Name = name;
            MaxPlayers = maxPlayers;
            PlayerCount = playerCount;
            Players = players ?? Array.Empty<SocketIOUser>();
        }
    }
}
