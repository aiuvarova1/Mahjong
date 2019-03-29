using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{
    const uint playersToStart = 1;
    
    public static GameMaster instance = null;

    public SyncListInt availableCameras = new SyncListInt();

    public int playerCount;

    public int readyPlayers;

    [SyncVar]
    public string gameState = "prepare";

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


    }
    private void Start()
    {
        
        if (availableCameras.Count == 0) InitCameras();
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
        playerCount++;
    }

    public void UnregisterPlayer(string netID)
    {
        playerCount--;
        availableCameras.Add(players[netID].order);
        Debug.Log($"add {players[netID].order} back");
        players.Remove(netID);
    }

    void GetReady()
    {
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetStartWaiting(player.connectionToClient);
        }
    }

    void SetReady()
    {
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

    private void Update()
    {
        if (!isServer) return;

        switch (gameState)
        {
            case "prepare":
                if (playerCount == playersToStart) gameState = "ready";
                return;
            case "ready":
                gameState = "waiting";
                GetReady();
                return;
            case "waiting":
                if (playersToStart != playerCount)
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
                GameManager.instance.StartGame();

                gameState = "starting";
                return;
        }
        //if (playerCount == playersToStart && gameState=="prepare") gameState = "ready";
        //if (gameState == "ready")
        //{
        //    gameState = "waiting";
        //    GetReady();
        //}
        //if (gameState == "waiting" && readyPlayers == playersToStart)
        //{
        //    gameState = "start";
        //    SetReady();
        //}

    }
}
