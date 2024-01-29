using System;

namespace Klem.SocketChat.ChatSystem.DataClasses
{
    [System.Serializable]
    public class SocketServerConnection
    {
        public float Date;
        public string ChatId;
        
        /// <summary>
        ///  Returns the date of the connection as a TimeSpan.
        /// </summary>
        /// <returns>
        /// The date of the connection as a TimeSpan.
        /// </returns>
        public TimeSpan GetDate()
        {
            return TimeSpan.FromMilliseconds(Date);
        }
    }
}
