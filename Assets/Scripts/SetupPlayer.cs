using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(Player))]

public class SetupPlayer : NetworkBehaviour
{

    public Player player;

    //initialization
    private void Start()
    {

        if (isServer)
        {
            SetOrder();
        }

        try
        {
            if (isLocalPlayer)
                AudioManager.instance.SetTheme();
        }
        catch (Exception ex)
        {
            return;
        }

        if (!isLocalPlayer)
        {
            gameObject.GetComponent<PlayerUI>().canvas.enabled = false;
            return;
        }
    }

    //adds player name on server
    [Command]
    void CmdAddPlayerName(string wind, string name)
    {
        TargetAddPlayerName(connectionToClient, wind, name);
    }

    //adds player's name on other players' labels
    [TargetRpc]
    public void TargetAddPlayerName(NetworkConnection conn, string wind, string name)
    {
        gameObject.GetComponent<UiWinds>().SetName(wind, name);
    }

    //changes wind for the new round
    [TargetRpc]
    public void TargetChangeWind(NetworkConnection conn)
    {
        GameMaster.instance.lights[player.order].SetActive(false);

        player.order--;
        if (player.order == -1)
            player.order = 3;

        AssignWind();

        GameMaster.instance.lights[player.order].SetActive(true);
        Camera cam = Instantiate(GameMaster.instance.allCameras[player.order]);

        Destroy(player.Camera.gameObject);

        player.Camera = cam;
        float newRotation = player.Camera.transform.eulerAngles.y;

        GetComponent<PlayerUI>().mainCam.enabled = true;
        GetComponent<PlayerUI>().mainCam.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, newRotation, Camera.main.transform.eulerAngles.z);

        GetComponent<PlayerUI>().mainCam.enabled = false;
        gameObject.GetComponent<UiWinds>().AssignWinds();

        CmdChangeWind(player.order, player.wind);
    }

    //commands to change player's name
    [Command]
    public void CmdChangeWind(int order, string wind)
    {
        RpcChangeWind(order, wind);
    }

    //tells new wind to other clients
    [ClientRpc]
    public void RpcChangeWind(int order, string wind)
    {
        player.order = order;
        player.wind = wind;
    }

    //refreshes old score on all clients
    [ClientRpc]
    public void RpcRefreshOldScore()
    {
        player.oldScore = 2000;
    }

    //sets player's wind on server
    void SetOrder()
    {
        int order = 0;
        try
        {
            if (GameMaster.instance.availableCameras.Count == 0)
                throw new ArgumentException();

            order = GameMaster.instance.availableCameras[UnityEngine.Random.Range(0, GameMaster.instance.availableCameras.Count)];
            //order = GameMaster.instance.availableCameras[UnityEngine.Random.Range(0, GameMaster.instance.availableCameras.Count-2)];
        }
        catch (Exception ex)
        {
            GetComponent<PlayerUI>().DropConnection();
        }
        player = gameObject.GetComponent<Player>();

        TargetSetOrder(player.connectionToClient, order);
    }

    //sets camera and wind on player
    [TargetRpc]
    void TargetSetOrder(NetworkConnection conn, int order)
    {
        player = gameObject.GetComponent<Player>();

        player.order = order;
        if (PlayerPrefs.instance != null)
            player.name = PlayerPrefs.instance.Name;

        gameObject.tag = "Player";
        player.Camera = Instantiate(GameMaster.instance.allCameras[player.order]);

        float newRotation = player.Camera.transform.eulerAngles.y;
        Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, newRotation, Camera.main.transform.eulerAngles.z);

        GameMaster.instance.lights[order].SetActive(true);
        AssignWind();
        gameObject.GetComponent<UiWinds>().AssignWinds();

        CmdAddPlayer(player.order, player.wind, player.name);
    }

    //assigns wind by order
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
        if (LocalizationManager.instance == null)
            player.GetComponent<PlayerUI>().playerWind.text = $"Your wind: {player.wind}";
        else
            player.GetComponent<PlayerUI>().playerWind.text = $"{LocalizationManager.instance.GetLocalizedValue("Your wind")}: " +
                $"{LocalizationManager.instance.GetLocalizedValue(player.wind)}";

    }

    //adds player data on server
    [Command]
    void CmdAddPlayer(int ord, string wind, string name)
    {
        // GameMaster.instance.AddPlayer(GetComponent<NetworkIdentity>().netId.ToString(), ord, wind);
        RpcAddPlayer(ord, wind, name);
        GameMaster.instance.AddLabel(this);
    }

    //adds player data on clients
    [ClientRpc]
    void RpcAddPlayer(int ord, string wind, string name)
    {
        player = gameObject.GetComponent<Player>();

        player.wind = wind;
        player.name = name;
        //if (isServer) return;
        GameMaster.instance.AddPlayer(GetComponent<NetworkIdentity>().netId.ToString(), ord, wind);

        GameObject.FindGameObjectWithTag("Player").GetComponent<UiWinds>().SetName(wind, name);

    }

    //adds new player on initialization
    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameMaster.instance.RegisterPlayer(netID, player);
    }

    //invokes player's disconnect
    [TargetRpc]
    private void TargetDisconnect(NetworkConnection target)
    {
        GetComponent<PlayerUI>().DropConnection();
    }

}
