using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klem.SocketChat.ChatSystem.SettingsScript
{
    /// <summary>
    ///   This class is a ScriptableObject that contains the settings for the Chat System.
    /// </summary>
    [CreateAssetMenu(menuName = "Klem/ChatSystem/Chat System Settings", fileName = "Chat System Settings")]
    public class ChatSettings : ScriptableObject
    {
        private static ChatSettings _instance;
        public static ChatSettings Instance {
            get
            {
                if (_instance == null)
                {
                    ChatSettings[] assets = Resources.LoadAll<ChatSettings>("");
                    if (assets == null || assets.Length < 1)
                    {
                        throw new Exception("Could not find any ChatSettings asset in the Resources Folder");
                    }
                    
                    // Warn if there are more than one
                    if (assets.Length > 1)
                    {
                        Debug.LogWarning("There are more than one ChatSettings asset in the Resources Folder. Using the first one");
                    }

                    _instance = assets[0];
                }
                
                return _instance;
            }
        }
        
        [field:SerializeField] public string ServerUri { get; private set; }
        [field:SerializeField] public string ServerPort { get; private set; }
        [field:SerializeField] public string ServerToken { get; private set; }
        /// <summary>
        ///  If true, the Chat System will log all the messages in the console.
        /// </summary>
        [field:SerializeField] public bool EnableLogging { get; private set; }
        
        [field:SerializeField] public Color ServerMessageColor { get; private set; }
        [field:SerializeField] public Color UserMessageColor { get; private set; }
        
        [field:SerializeField] public List<Color> UserColors { get; private set; }
        
    }
}
