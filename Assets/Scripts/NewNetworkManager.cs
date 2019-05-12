using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine;


public class NewNetworkManager : NetworkManager
{
    public static bool hostLeft = false;

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        hostLeft = true;
        base.OnClientDisconnect(conn);
        Debug.Log("Server is stopped from Manager");
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GameMaster.instance.TestUnregister(conn);
        base.OnServerDisconnect(conn);
        Debug.Log("Client is stopped from Manager");
    }
}

