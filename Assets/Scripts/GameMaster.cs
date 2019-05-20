using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{
    public const uint playersToStart = 4;

    public static GameMaster instance = null;

    public SyncListInt availableCameras = new SyncListInt();
    public List<GameObject> lights = new List<GameObject>();

    int roundsPlayed = 0;

    int playerCount;

    public int PlayerCount
    {
        get { return playerCount; }
        set
        {
            playerCount = value;
        }
    }


    public int readyPlayers;
    public int readyToContinuePlayers;
    public int playersToContinue;

    //"prepare" by default
    [SyncVar]
    public string gameState = "prepare";

    [SerializeField]
    public List<Camera> allCameras = new List<Camera>();

    private Dictionary<string, Player> players = new Dictionary<string, Player>();

    #region Start

    //initialization
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

    //refreshes num of ready players
    void Refresh()
    {
        readyPlayers = 0;
    }

    //initialization
    private void Start()
    {
        if (availableCameras.Count == 0) InitCameras();
        GameManager.instance.RefreshEv += Refresh;
    }

    //initializes nums of available cameras
    private void InitCameras()
    {
        for (int i = 0; i < allCameras.Count; i++)
        {
            availableCameras.Add(i);
        }
    }

    //refreshes available cameras
    public void AddPlayer(string netID, int ord, string wind)
    {
        availableCameras.Remove(ord);
        GameMaster.instance.players[netID].order = ord;
        instance.players[netID].wind = wind;

    }

    //adds player to the dictionary
    public void RegisterPlayer(string netID, Player player)
    {
        players.Add(netID, player);
        if (player.Camera != null) availableCameras.Remove(player.order);
        PlayerCount++;
        if (isServer && gameState == "end")
        {
            player.GetComponent<PlayerUI>().TargetInfo(player.connectionToClient, "Waiting for the end of the game...");
        }
    }

    //adds all players' names to labels of the new player
    public void AddLabel(SetupPlayer p)
    {
        foreach (Player player in players.Values)
        {
            p.TargetAddPlayerName(p.player.connectionToClient, player.wind, player.name);
        }
    }

    #endregion

    #region remove player

    //refreshes data after player's disconnection
    public void TestUnregister(NetworkConnection conn)
    {
        PlayerCount--;

        Player player = null;
        string remove = null;

        foreach (string netID in players.Keys)
        {
            player = players[netID];
            try
            {
                if (player.connectionToClient == conn)
                {
                    availableCameras.Add(player.order);
                    remove = netID;
                    break;
                }
            }
            catch (MissingReferenceException)
            {
                availableCameras.Add(player.order);
                remove = netID;
                break;
            }
        }
        RpcRemovePlayerName(player.wind);

        players.Remove(remove);
        if (gameState == "playing" || gameState == "start" || gameState == "starting"
            || gameState == "distributing" || gameState == "in process")
        {
            GameManager.instance.stopTheGame = true;
            GameManager.instance.RefreshAll();

            foreach (string id in players.Keys)
            {
                if (id == remove) return;
                Player p = players[id];
                p.GetComponent<PlayerUI>().TargetShowLeftPlayerInfo(p.connectionToClient, player.name, player.wind);
                player.GetComponent<SetupPlayer>().RpcRefreshOldScore();
            }

        }
    }

    //removes name from the labels of connected players
    [ClientRpc]
    void RpcRemovePlayerName(string wind)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<UiWinds>().RemoveName(wind);
    }

    #endregion
    #region Game Beginning

    //launches waiting of ready affirmation
    void GetReady()
    {
        Debug.Log(players.Count + " get ready");
        List<int> orders = new List<int>();
        foreach (Player player in players.Values)
        {
            if (orders.FindIndex(x => x == player.order) == -1)
                orders.Add(player.order);
            else
                player.GetComponent<PlayerUI>().leave = true;


            player.GetComponent<PlayerUI>().TargetStartWaiting(player.connectionToClient);
        }
    }

    //stops waiting coroutines
    void SetReady()
    {
        Debug.Log(players.Count + " set ready");
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetStopWaiting(player.connectionToClient);
        }
    }

    //darkens players' screens
    public void DarkenScreens()
    {
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetStartDarken(player.connectionToClient);
        }
    }

    //lightens players' screens
    public void LightenScreens()
    {
        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetStartLighten(player.connectionToClient);
        }
    }

    //disconnects players' who didn't confirm start
    public void KickAFK()
    {
        foreach (Player player in players.Values)
        {
            if (!player.GetComponent<PlayerUI>().ready)
                player.GetComponent<PlayerUI>().leave = true;
        }
        gameState = "prepare";
    }

    //disables quit button
    void DisableInfo()
    {

        foreach (Player player in players.Values)
        {
            player.GetComponent<PlayerUI>().TargetDisableInfoComponents(player.connectionToClient);
        }

    }

    //assign players' winds on server
    void AssignWinds()
    {

        foreach (Player player in players.Values)
        {
            TargetAddWind(player.connectionToClient);
        }
    }

    //adds one player's wind to server list
    [TargetRpc]
    void TargetAddWind(NetworkConnection conn)
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().CmdAddWind();
    }

    //changes winds at the beginning of a new round
    void ChangeWinds()
    {
        foreach (Player player in players.Values)
        {
            player.GetComponent<SetupPlayer>().TargetChangeWind(player.connectionToClient);
            TargetAddWind(player.connectionToClient); 
        }
    }

    //controls game state
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
                Invoke("GetReady", 1.2f);
                return;
            case "waiting":
                if (playersToStart != PlayerCount)
                {
                    readyPlayers = 0;
                    gameState = "prepare";
                    SetReady();

                }
                if (readyPlayers == playersToStart)
                {
                    gameState = "start";
                    SetReady();
                    readyPlayers = 0;
                }
                return;
            case "start":
                AssignWinds();
                DisableInfo();
                GameManager.instance.RefreshWinds();
                GameManager.instance.StartGame();

                gameState = "starting";
                return;

            case "starting":

                if (GameManager.instance.wallIsBuilt)
                {
                    GameManager.instance.DistributeTiles();
                    gameState = "distributing";
                }
                return;
            case "distributing":
                return;
            case "playing":
                GameManager.instance.CurrentWind = 0;
                GameManager.instance.InvokeChange();
                gameState = "in process";
                return;
            case "in process":
                return;
            case "end":

                playersToContinue = PlayerCount;

                if (playersToContinue == readyToContinuePlayers && playersToContinue != 0)
                {
                    if (playersToContinue == 4)
                    {
                        if (GameManager.instance.winnerWind != "East")
                        {
                            roundsPlayed++;

                            //switch major wind at the end of the round
                            if (roundsPlayed % 4 == 0)
                            {
                                switch (GameManager.instance.majorWind)
                                {
                                    case "East":
                                        GameManager.instance.majorWind = "South";
                                        break;
                                    case "South":
                                        GameManager.instance.majorWind = "West";
                                        break;
                                    case "West":
                                        GameManager.instance.majorWind = "North";
                                        break;
                                    case "North":
                                        GameManager.instance.majorWind = "East";
                                        break;
                                    default:
                                        break;
                                }
                            }

                            ChangeWinds();
                        }
                    }
                    else
                    {
                        foreach (Player player in players.Values)
                        {
                            player.GetComponent<SetupPlayer>().RpcRefreshOldScore();
                        }
                    }
                    gameState = "prepare";
                }

                return;
            default:
                return;

        }
    }
    #endregion
}
