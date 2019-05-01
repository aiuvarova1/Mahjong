using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(Player))]

public class SetupPlayer : NetworkBehaviour
{

    Player player;

    private void Start()
    {
        

        if (isServer)
        {
            SetOrder();
        }


        if (!isLocalPlayer)
        {
            gameObject.GetComponent<PlayerUI>().canvas.enabled = false;
            return;
        }
        
    } 

    void SetOrder()
    {
        int order = 0;
        try
        {
            order = GameMaster.instance.availableCameras[UnityEngine.Random.Range(0, GameMaster.instance.availableCameras.Count)];
            //order = GameMaster.instance.availableCameras[UnityEngine.Random.Range(0, GameMaster.instance.availableCameras.Count-2)];
        }
        catch (ArgumentOutOfRangeException)
        {
            GetComponent<PlayerUI>().DropConnection();
        }
        player = gameObject.GetComponent<Player>();

        TargetSetOrder(player.connectionToClient, order);
    }

    [TargetRpc]
    void TargetSetOrder(NetworkConnection conn,int order)
    {
        player = gameObject.GetComponent<Player>();

        player.order = order;

        gameObject.tag = "Player";

        player.Camera = Instantiate(GameMaster.instance.allCameras[player.order]);

        float newRotation = player.Camera.transform.eulerAngles.y;
        Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, newRotation, Camera.main.transform.eulerAngles.z);
        AssignWind();

        CmdAddPlayer(player.order, player.wind);
    }

    void AssignWind()
    {
        switch (player.order)
        {
            case 0:
                player.wind = "East";

                break;
            case 1:
                player.wind = "South";

                break;
            case 2:
                player.wind = "West";

                break;
            case 3:
                player.wind = "North";

                break;
            default:
                Debug.LogError($"Order {player.order} is incorrect");
                return;

        }
        player.GetComponent<PlayerUI>().playerWind.text = $"Your wind: {player.wind}";

    }

    [Command]
    void CmdAddPlayer(int ord,string wind)
    {
        RpcAddPlayer(ord,wind);
    }

    [ClientRpc]
    void RpcAddPlayer(int ord,string wind)
    {
        GameMaster.instance.AddPlayer(GetComponent<NetworkIdentity>().netId.ToString(),ord,wind);
        Debug.Log($"Camera {ord}, wind {wind}");
        Debug.Log(GameMaster.instance.PlayerCount + "players");

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
        Debug.Log($"command {GameMaster.instance.PlayerCount} players");
        RpcUnregisterPlayer(netID);
        TargetDisconnect(connectionToClient);
    }

    [ClientRpc]
    public void RpcUnregisterPlayer(string netID)
    {
        Debug.Log($"rpc {GameMaster.instance.PlayerCount} players");
        GameMaster.instance.UnregisterPlayer(netID);
        Debug.Log($"rpc {GameMaster.instance.PlayerCount} players");
        Debug.Log($"{GameMaster.instance.availableCameras.Count} cameras");
    }


    [TargetRpc]
    private void TargetDisconnect(NetworkConnection target)
    {
        Debug.Log("Target");
        GetComponent<PlayerUI>().DropConnection();
    }

    //void OnApplicationQuit()
    //{
    //    if (isLocalPlayer)
    //    {
    //        CmdUnregisterPlayer(GetComponent<NetworkIdentity>().netId.ToString());
    //    }
    //    if (isServer)
    //    {
    //        RpcUnregisterPlayer(GetComponent<NetworkIdentity>().netId.ToString());
    //    }
    //}
}
