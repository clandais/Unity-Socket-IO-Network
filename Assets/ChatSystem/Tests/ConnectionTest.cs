using System.Collections;
using System.Threading.Tasks;
using Klem.SocketChat.ChatSystem;
using Klem.SocketChat.ChatSystem.DataClasses;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ConnectionTest
{
    [Test]
    public async void TestConnection()
    {
        await SocketIONetwork.Connect();
        Assert.IsTrue(SocketIONetwork.IsConnected);
        SocketIONetwork.Disconnect();
        Assert.IsFalse(SocketIONetwork.IsConnected);
    }
}
