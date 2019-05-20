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

    //indicates server disconnect on client 
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        hostLeft = true;
        base.OnClientDisconnect(conn);
    }

    //indicates client disconnect on server
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GameMaster.instance.TestUnregister(conn);
        base.OnServerDisconnect(conn);
    }
}

