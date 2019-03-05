using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]

public class SetupPlayer : NetworkBehaviour
{

    Player player;

    private void Start()
    {

        if (!isLocalPlayer)
        {
            gameObject.GetComponent<PlayerUI>().canvas.enabled = false;
            return;
        }

        player = gameObject.GetComponent<Player>();

        Debug.Log(GameMaster.instance.availableCameras.Count);

        int order= GameMaster.instance.availableCameras[Random.Range(0, GameMaster.instance.availableCameras.Count)];
        player.order = order;

        player.Camera = Instantiate(GameMaster.instance.allCameras[player.order]);

        CmdAddPlayer(player.order);

        
    } 

    [Command]
    void CmdAddPlayer(int ord)
    {
        RpcAddPlayer(ord);
    }

    [ClientRpc]
    void RpcAddPlayer(int ord)
    {
        GameMaster.instance.AddPlayer(GetComponent<NetworkIdentity>().netId.ToString(),ord);
        Debug.Log($"Camera {ord} ");
        Debug.Log(GameMaster.instance.playerCount + "players");

        Debug.Log(GameMaster.instance.availableCameras.Count + "cameras");
        for (int i = 0; i < GameMaster.instance.availableCameras.Count; i++)
        {
            Debug.Log($"Camera {GameMaster.instance.availableCameras[i]}");
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameMaster.instance.RegisterPlayer(netID, player);

    }

    [Command]
    public void CmdUnregisterPlayer(string netID)
    {
        Debug.Log($"command {GameMaster.instance.playerCount} players");
        RpcUnregisterPlayer(netID);
        TargetDisconnect(connectionToClient);
    }

    [ClientRpc]
    public void RpcUnregisterPlayer(string netID)
    {
        Debug.Log($"rpc {GameMaster.instance.playerCount} players");
        GameMaster.instance.UnregisterPlayer(netID);
        Debug.Log($"rpc {GameMaster.instance.playerCount} players");
        Debug.Log($"{GameMaster.instance.availableCameras.Count} cameras");
    }

    [TargetRpc]
    private void TargetDisconnect(NetworkConnection target)
    {
        Debug.Log("Target");
        GetComponent<PlayerUI>().DropConnection();
    }

    void OnApplicationQuit()
    {
        if (isLocalPlayer)
        {
            CmdUnregisterPlayer(GetComponent<NetworkIdentity>().netId.ToString());
        }
        if (isServer)
        {
            RpcUnregisterPlayer(GetComponent<NetworkIdentity>().netId.ToString());
        }
    }
}
