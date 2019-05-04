﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{
    public const uint playersToStart = 4;
    
    public static GameMaster instance = null;

    public SyncListInt availableCameras = new SyncListInt();
    public List<GameObject> lights = new List<GameObject>();


    int playerCount;

    public int PlayerCount
    {
        get { return playerCount; }
        set
        {
            playerCount = value;
            //if(isServer)
            //    RefreshNumOfPlayers();
        }
    }


    public int readyPlayers;

    //"prepare" by default
    [SyncVar]
    public string gameState = "start";

    [SerializeField]
    public List<Camera> allCameras = new List<Camera>();

    private Dictionary<string, Player> players = new Dictionary<string, Player>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);

        Application.runInBackground = true;

        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }

    }
    private void Start()
    {
        
        if (availableCameras.Count == 0) InitCameras();
    }

    void RefreshNumOfPlayers()
    {
        Debug.Log(playerCount);
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetRefreshPlayers(player.connectionToClient,PlayerCount);
        }
    }


    private void InitCameras()
    {
        for (int i = 0; i < allCameras.Count; i++)
        {
            availableCameras.Add(i);
        }
    }

    public void AddPlayer(string netID, int ord, string wind)
    {
        availableCameras.Remove(ord);
        GameMaster.instance.players[netID].order = ord;
        instance.players[netID].wind= wind;

    }

    public void RegisterPlayer(string netID, Player player)
    {
        players.Add(netID, player);
        if(player.Camera!=null) availableCameras.Remove(player.order);
        PlayerCount++;
    }

    public void UnregisterPlayer(string netID)
    {
        PlayerCount--;
        availableCameras.Add(players[netID].order);
        Debug.Log($"add {players[netID].order} back");
        players.Remove(netID);
    }

    public void TestUnregister(NetworkConnection conn)
    {
        PlayerCount--;
        Debug.Log(PlayerCount);


        Debug.Log("tst");
        string remove = null;

        foreach (string netID in players.Keys)
        {
            Player player = players[netID];
            Debug.Log(player.order);

            try
            {
                if (player.connectionToClient == conn)
                {
                    Debug.Log("found");
                    availableCameras.Add(player.order);
                    Debug.Log($"add {player.order} back");
                    remove = netID;
                    
                }
            }
            catch(MissingReferenceException)
            {
                Debug.Log("эксепшон");
                Debug.Log("found");
                availableCameras.Add(player.order);
                Debug.Log($"add {player.order} back");
                remove = netID;

            }
        }
        players.Remove(remove);

    }

    void GetReady()
    {
        Debug.Log(players.Count + " get ready");
        List<int> orders = new List<int>();
        foreach (Player player in players.Values)
        {
            if (orders.FindIndex(x => x == player.order) == -1 )
                orders.Add(player.order);
            else
                player.GetComponent<PlayerUI>().leave = true;

            player.GetComponent<PlayerUI>().TargetStartWaiting(player.connectionToClient);
        }
    }

    void SetReady()
    {
        Debug.Log(players.Count + " set ready");
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetStopWaiting(player.connectionToClient);
        }
    }

    public void DarkenScreens()
    {
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetStartDarken(player.connectionToClient);
        }
    }

    public void LightenScreens()
    {
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetStartLighten(player.connectionToClient);
        }
    }

    public void KickAFK()
    {
        
        foreach (Player player in players.Values)
        {
            if (!player.GetComponent<PlayerUI>().ready)
                player.GetComponent<PlayerUI>().leave = true;
        }
        gameState = "prepare";
    }

     void DisableInfo()
    {

        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetDisableInfoComponents(player.connectionToClient);
        }
        
    }


    void AssignWinds()
    {
        
        foreach (Player player in players.Values)
        {
            TargetAddWind(player.connectionToClient);
        }
    }

    [TargetRpc]
    void TargetAddWind(NetworkConnection conn)
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().CmdAddWind();
    }

    private void Update()
    {
        if (!isServer) return;
        switch (gameState)
        {
            case "prepare":
                if (PlayerCount == playersToStart) gameState = "ready";
                return;
            case "ready":
                gameState = "waiting";
                GetReady();
                return;
            case "waiting":
                if (playersToStart != PlayerCount)
                {
                    gameState = "prepare";
                    SetReady();

                }
                if (readyPlayers == playersToStart)
                {
                    gameState = "start";
                    SetReady();
                }
                return;
            case "start":
                AssignWinds();
                DisableInfo();
                GameManager.instance.StartGame();

                gameState = "starting";
                return;

            case "starting":

                if (GameManager.instance.wallIsBuilt)
                {
                    GameManager.instance.DistributeTiles();
                    //Debug.Log("free tiles");
                    gameState = "distributing";
                }
                return;
            case "distributing":
                return;
            case "playing":
                GameManager.instance.CurrentWind = 0;
                GameManager.instance.InvokeChange();
                Debug.Log(gameState);
                gameState = "in process";
                return;
            default:
                return;

        }
    }
}
