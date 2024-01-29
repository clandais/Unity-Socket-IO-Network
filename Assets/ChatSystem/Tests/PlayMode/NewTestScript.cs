using System.Collections;
using System.Collections.Generic;
using Klem.SocketChat.ChatSystem;
using Klem.SocketChat.ChatSystem.DataClasses;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    [UnityTest]
    public IEnumerator TestConnectionMono()
    {

        var mono = new MonoBehaviourTest<CallBackMonoTest>();
        SocketIONetwork.Connect();
        
        yield return mono;
        
        mono.component.IsTestFinished = false;
        
        SocketIONetwork.Disconnect();
        
        yield return mono;
        
        
        Assert.IsTrue(mono.component.OnConnectedToMasterCalled);
        Assert.IsTrue(mono.component.OnDisconnectedCalled);
    }
    
    public class CallBackMonoTest : MonoBehaviourSocketCallBacks, IMonoBehaviourTest
    {
        public bool IsTestFinished { get;  set; }
        public bool OnConnectedToMasterCalled { get; private set; }
        public bool OnDisconnectedCalled { get; private set; }

        public override void OnConnectedToMaster(SocketServerConnection connection)
        {
            Assert.IsTrue(SocketIONetwork.IsConnected);
            Assert.AreEqual(SocketIONetwork.User.ChatId, connection.ChatId);
            OnConnectedToMasterCalled = true;
            IsTestFinished = true;
        }


        public override void OnDisconnecting(string reason)
        {
            Assert.IsFalse(SocketIONetwork.IsConnected);
            OnDisconnectedCalled = true;
            IsTestFinished = true;
        }
    }
}
