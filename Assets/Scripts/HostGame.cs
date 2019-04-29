using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HostGame : MonoBehaviour
{

    const uint roomSize = 4;

    public string RoomName { private get; set; }

    private NewNetworkManager networkManager;

    // Use this for initialization
    void Start()
    {
        networkManager = (NewNetworkManager)NewNetworkManager.singleton;
        if(networkManager.matchMaker==null)
            networkManager.StartMatchMaker();
        if (networkManager == null) Debug.Log("((");

        networkManager = (NewNetworkManager)NewNetworkManager.singleton;
        ConnectionConfig myConfig = networkManager.connectionConfig;

        myConfig.NetworkDropThreshold = 95;
        myConfig.OverflowDropThreshold = 30;
        myConfig.InitialBandwidth = 0;
        myConfig.MinUpdateTimeout = 10;
        myConfig.ConnectTimeout = 2000;
        myConfig.PingTimeout = 1500;
        myConfig.DisconnectTimeout = 6000;
        myConfig.PacketSize = 1470; myConfig.SendDelay = 2;
        myConfig.FragmentSize = 1300;
        myConfig.AcksType = ConnectionAcksType.Acks128;
        myConfig.MaxSentMessageQueueSize = 256;
        myConfig.AckDelay = 1;
    }

    public void CreateRoom()
    {
        if (RoomName!="" && RoomName != null)
        {
            Debug.Log(RoomName+ " created");
            networkManager.matchMaker.CreateMatch(RoomName, roomSize, true, "","","",0,0, networkManager.OnMatchCreate);

        }

    }
    private void Update()
    {

    }






}
