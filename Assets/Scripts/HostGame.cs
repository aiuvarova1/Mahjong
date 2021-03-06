﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HostGame : MonoBehaviour
{
    const uint roomSize = 4;

    public string RoomName { private get; set; }

    private NewNetworkManager networkManager;

    // Use this for initialization
    void Start()
    {
        networkManager = (NewNetworkManager)NewNetworkManager.singleton;
        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();

        networkManager = (NewNetworkManager)NewNetworkManager.singleton;
        ConnectionConfig myConfig = networkManager.connectionConfig;

        myConfig.NetworkDropThreshold = 95;
        myConfig.OverflowDropThreshold = 70;
        myConfig.InitialBandwidth = 0;
        myConfig.MinUpdateTimeout = 10;
        myConfig.ConnectTimeout = 2000;
        myConfig.PingTimeout = 1500;
        myConfig.DisconnectTimeout = 8000;
        myConfig.PacketSize = 1470; myConfig.SendDelay = 2;
        myConfig.FragmentSize = 1300;
        myConfig.AcksType = ConnectionAcksType.Acks128;
        myConfig.MaxSentMessageQueueSize = 256;
        myConfig.AckDelay = 1;

        networkManager.maxDelay = 0.5f;
    }

    //creates new game room
    public void CreateRoom()
    {
        if (RoomName != "" && RoomName != null)
        {
            GameObject.FindGameObjectWithTag("ok").SetActive(false);
            try
            {
                networkManager.matchMaker.CreateMatch(RoomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
            }
            catch (NullReferenceException)
            {
                GameObject.FindGameObjectWithTag("ok").SetActive(true);
            }
        }
    }
}
