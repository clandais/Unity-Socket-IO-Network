using System;
using System.Collections.Generic;
using Klem.SocketChat.ChatSystem.DataClasses;
using UnityEngine;
using UnityEngine.UI;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample
{
    public enum ETab
    {
        General,
        Room,
    }

    
    [AddComponentMenu("ChatSystem/SimpleChatSample/TabGroup")]
    public class TabGroup : MonoBehaviourSocketCallBacks
    {
        [SerializeField] private Button generalTabButton;
        public Button GeneralTabButton => generalTabButton;
        [SerializeField] private Button roomTabButton;
        public Button RoomTabButton => roomTabButton;

        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        
        public event Action<ETab> OnTabChanged;
        
        
        private ETab _currentTab = ETab.General;
        public ETab CurrentTab => _currentTab;
        

        private void Start()
        {
            generalTabButton.onClick.AddListener(OnGeneralTabButtonClicked);
            roomTabButton.onClick.AddListener(OnRoomTabButtonClicked);
            roomTabButton.gameObject.SetActive(false);
            
            generalTabButton.GetComponent<Image>().color = activeColor;
            roomTabButton.GetComponent<Image>().color = inactiveColor;
        }


        private void OnGeneralTabButtonClicked()
        {
            
            if (_currentTab == ETab.General)
                return;
            
            generalTabButton.GetComponent<Image>().color = activeColor;
            roomTabButton.GetComponent<Image>().color = inactiveColor;
            _currentTab = ETab.General;
            OnTabChanged?.Invoke(_currentTab);
        }
        
        
        private void OnRoomTabButtonClicked()
        {
            
            if (_currentTab == ETab.Room)
                return;
            
            roomTabButton.GetComponent<Image>().color = activeColor;
            generalTabButton.GetComponent<Image>().color = inactiveColor;
            
            _currentTab = ETab.Room;
            OnTabChanged?.Invoke(_currentTab);
        }

        public override void OnRoomJoined(Room room)
        {
            roomTabButton.gameObject.SetActive(true);
        }

        public override void OnRoomLeft(Room room)
        {
            roomTabButton.gameObject.SetActive(false);
        }
    }
}
