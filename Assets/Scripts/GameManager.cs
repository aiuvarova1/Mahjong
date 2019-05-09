using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;


public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;
    BinaryFormatter bf;

    public bool wallIsBuilt = false;
    public bool tilesAreGiven = false;
    bool waitForCombinations = false;

    public List<Wind> winds = new List<Wind>();

    public Table GameTable { get; set; }

    int currentWind;
    public string majorWind = "East";

    public delegate void RefreshDel();
    public event RefreshDel RefreshEv;

    public bool stopTheGame = false;
    public bool canStopTheGame = false;

    public string winnerWind;


    int numOfAnsweredPlayers;

    public int NumOfAnsweredPlayers
    {
        get { return numOfAnsweredPlayers; }
        set
        {
            numOfAnsweredPlayers = value;
            Debug.Log(numOfAnsweredPlayers + " num answered");
        }
    }

    public Player chowDeclarator;
    public Player pungDeclarator;
    public Player kongDeclarator;
    public Player mahJongDeclarator;

    public int CurrentWind
    {
        get
        {
            return currentWind;
        }

        set
        {
            if (value > 3) currentWind = 0;
            else currentWind = value;
            Debug.Log("current wind" + currentWind);
        }
    }

    #region Coroutines
    IEnumerator Lighten()
    {
        yield return new WaitForSeconds(1.9f);
        GameMaster.instance.LightenScreens();
    }


    IEnumerator Build()
    {

        yield return new WaitForSeconds(1.8f);

        RpcMakeTilesVisible();

    }
    #endregion

    #region Game Start

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);

        if (winds.Count == 0)
        {
            winds.Add(new East());
            winds.Add(new South());
            winds.Add(new West());
            winds.Add(new North());
        }

        CurrentWind = 0;
        GameTable = new Table();


    }

    private void Start()
    {
        RefreshEv += instance.Refresh;
        RefreshEv += Wall.instance.Refresh;
        RefreshEv += BuildWall.instance.Refresh;
        
    }

    public void StartGame()
    {

        BuildWall.instance.FillIndexes();

        var o = new MemoryStream(); //Create something to hold the data

        bf = new BinaryFormatter(); //Create a formatter
        bf.Serialize(o, BuildWall.instance.indexes); //Save the list

        var data = Convert.ToBase64String(o.GetBuffer()); //Convert the data to a string
        BuildWall.instance.Build(BuildWall.instance.indexes);

        RpcBuildOnAllClients(data);
        //for server

        GameMaster.instance.DarkenScreens();

        StartCoroutine(Build());

        StartCoroutine(Lighten());

        Debug.Log(BuildWall.instance.tiles.Count + "tiles");

    }

    public void DistributeTiles()
    {
        for (int i = 0; i < winds.Count; i++)
        {
            Vector3[] pos = new Vector3[14];
            pos[0] = winds[i].startPosition;
            pos[0].y = 1.4f;

            Vector3 curPos = pos[0];

            for (int j = 1; j < pos.Length; j++)
            {
                winds[i].MoveRightFreePosition(ref curPos);
                pos[j] = curPos;
            }

            if (winds[i].player != null)
                winds[i].player.RpcFillPositionList(pos);
        }


        Wall.instance.AssighFreeTiles();

        Wall.instance.DistributeTiles();

        Invoke("SortTiles", 3f);
        Invoke("FixTilesPositions", 3.2f);

        Invoke("CheckAllPlayersForFlowers", 3.5f);
    }

    void FixTilesPositions()
    {
        for (int i = 0; i < winds.Count; i++)
        {
            if (winds[i].player != null)
                winds[i].player.RpcFix();
        }
    }

    [ClientRpc]
    void RpcBuildOnAllClients(string data)
    {
        if (isServer) return;

        var ins = new MemoryStream(Convert.FromBase64String(data)); //Create an input stream from the string
                                                                    //Read back the data
        bf = new BinaryFormatter();
        List<int> indexes = (List<int>)bf.Deserialize(ins);
        BuildWall.instance.Build(indexes);
    }




    [ClientRpc]
    void RpcMakeTilesVisible()
    {
        // NetworkManager.
        foreach (List<WallPair> lst in BuildWall.instance.tiles)
        {
            foreach (WallPair pair in lst)
            {
                pair.upperTile.MakeVisible();
                pair.lowerTile.MakeVisible();
            }
        }
    }

    public void SortTiles()
    {
        Debug.Log("sort tiles");
        for (int i = 0; i < winds.Count; i++)
        {
            if (winds[i].player != null)
                winds[i].player.RpcSort();
        }
        //RpcSortTiles();
    }
    #endregion

    #region CheckFlowers
    void CheckAllPlayersForFlowers()
    {
        if (!isServer) return;

        Debug.Log(currentWind + "cur");
        //may need fix
        if (currentWind >= 4)
        {
            currentWind = 0;
            GameMaster.instance.gameState = "playing";
            return;
        }
        if (winds[currentWind].player == null || !winds[currentWind].player.CheckForFlowers())
        {
            currentWind++;
            CheckAllPlayersForFlowers();
            return;
        }
        Debug.Log(currentWind);

        CheckForFlowers();

    }

    public void CheckForFlowers()
    {
        Player player = winds[currentWind].player;
        if (!player.CheckForFlowers())
        {
            Debug.Log(GameMaster.instance.gameState);

            if (GameMaster.instance.gameState == "distributing" || GameMaster.instance.gameState == "starting")
            {
                currentWind++;
                CheckAllPlayersForFlowers();
            }
            else
            {
                player.Sort();
                Invoke("BeginPlayerMove", 0.3f);
            }
            return;
        }

        Debug.Log(player.playerTiles.Count);
        player.RpcLieOutTile(player.playerTiles.Count - 1, winds[currentWind].freeFlowerPosition, winds[currentWind].rotation, "flowers");
        //player.playerTiles.RemoveAt(player.playerTiles.Count - 1);

        winds[currentWind].MoveRightFreePosition(ref winds[currentWind].freeFlowerPosition);
        winds[currentWind].MoveLeftFreePosition(ref winds[currentWind].freePosition);

    }
    #endregion


    #region Combinations and turns

    public void DeclareCombination(string combination, int audioNum)
    {
        string wind = winds[CurrentWind].Name;
        foreach (Wind playerWind in winds)
        {

            if (playerWind.player != null && playerWind.Name != wind)
                playerWind.player.gameObject.GetComponent<PlayerUI>().TargetShowDeclaredCombination(playerWind.player.connectionToClient, wind, combination, true, audioNum);
            else if (playerWind.Name == wind)
                playerWind.player.gameObject.GetComponent<PlayerUI>().TargetShowDeclaredCombination(playerWind.player.connectionToClient, wind, combination, false, audioNum);
        }
    }

    public void InvokeChange()
    {
        Invoke("ChangeTurn", 1.6f);
    }

    public void ChangeTurn()
    {
        Wall.instance.GiveWallTile();

        //Invoke("CheckForFlowers", 1f);
    }


    public void BeginPlayerMove()
    {
        Player curPlayer = winds[CurrentWind].player;
        if (curPlayer == null) return;

        curPlayer.TargetSelectLastTile(curPlayer.connectionToClient);
        canStopTheGame = true;
        curPlayer.playerTurn = true;
        TargetSetTurn(curPlayer.connectionToClient, true);

    }

    [TargetRpc]
    public void TargetSetTurn(NetworkConnection conn, bool turn)
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().playerTurn = turn;
    }


    public void PrepareForCombinations()
    {
        for (int i = 0; i < winds.Count; i++)
        {
            if (winds[i].player != null)
                winds[i].player.RpcSort();
        }
        Invoke("Prepare", 2);

    }

    void Prepare()
    {
        Debug.Log("prepare for comb");
        canStopTheGame = true;

        NumOfAnsweredPlayers = 0;
        chowDeclarator = null;
        pungDeclarator = null;
        kongDeclarator = null;
        mahJongDeclarator = null;

        waitForCombinations = true;

        if (winds[CurrentWind].player != null)
            winds[CurrentWind].player.isFreeTile = false;


        for (int i = 0; i < winds.Count; i++)
        {
            if (winds[i].player != null && i != CurrentWind)
                winds[i].player.TargetSetCombinationTurn(winds[i].player.connectionToClient);
        }

    }

    int DefineWind(Player player)
    {
        for (int i = 0; i < winds.Count; i++)
        {
            if (winds[i].player == player)
                return i;
        }
        return -1;
    }




    private void Update()
    {
        if (!isServer) return;


        //set turns!
        if (waitForCombinations && GameMaster.playersToStart - 1 <= NumOfAnsweredPlayers)
        {
            Debug.Log("here");

            if (canStopTheGame && stopTheGame)
            {
                stopTheGame = false;
                RefreshAll();
                return;

            }

            if (mahJongDeclarator != null)
            {
                GameMaster.instance.readyToContinuePlayers = 0;
                GameMaster.instance.gameState = "end"; 
                mahJongDeclarator.DeclareMahJong(DefineWind(mahJongDeclarator));
                RefreshWinds();
            }
            else if (kongDeclarator != null)
            {
                kongDeclarator.DeclareKong(DefineWind(kongDeclarator));
            }
            else if (pungDeclarator != null)
            {
                pungDeclarator.DeclarePung(DefineWind(pungDeclarator));
            }
            else if (chowDeclarator != null)
            {
                chowDeclarator.DeclareChow(DefineWind(chowDeclarator));
            }
            else
            {
                CurrentWind++;
                ChangeTurn();
            }
            
            waitForCombinations = false;
            canStopTheGame = false;
        }
        //if(waitForCombinations)
        //    Debug.Log("server");
        if (canStopTheGame && stopTheGame)
        {
            stopTheGame = false;
            RefreshAll();
            
        }
    }
    #endregion

    #region End of the game
    //!!!!!!!!!
    public void DeclareDraw()
    {
        Debug.Log("Draw");
        RefreshWinds();
        for (int i = 0; i < winds.Count; i++)
        {
            if (winds[i].player != null)
                winds[i].player.GetComponent<PlayerUI>().TargetShowDeclaredCombination(winds[i].player.connectionToClient,
                    "", "",true,-1);
        }

        Invoke("RefreshAll", 4f);

    }

   

    void OpenAllTiles()
    {
        Debug.Log("OpenTiles");
        for (int i = 0; i < winds.Count; i++)
        {
            winds[i].player.RpcOpenTiles(winds[i].rotation);
        }
    }

    public void FinishGame(MahJong mahjong)
    {
        Invoke("OpenAllTiles", 2.3f);

        Player winner = winds[CurrentWind].player;

        winnerWind = winner.wind;

        int score = mahjong.CalculateMahJongPoints(winner.wind, winner.playerTurn, winner.isFreeTile, winner.order);
        winner.score = score;

        for (int i = 0; i < winds.Count; i++)
        {
            if (winds[i].player != winner && winds[i].player != null)
            {
                winds[i].player.score = MahJong.CountNotWinnerScore(ref winds[i].player.closedCombinations,
                    winds[i].player.openedTiles, winds[i].player.playerTiles, winds[i].player.flowers, winds[i].player.wind, i);
            }
        }

        Invoke("SetScores", 2f);
    }

    void SetScores()
    {
        int[] scores = new int[winds.Count];
        string[] names = new string[winds.Count];
        int[] oldScores = new int[winds.Count];

        for (int i = 0; i < winds.Count; i++)
        {
            scores[i] = winds[i].player.score;
            names[i] = winds[i].player.name;
            oldScores[i] = winds[i].player.oldScore;
        }

        for (int i = 0; i < winds.Count; i++)
        {
            winds[i].player.GetComponent<PlayerUI>().TargetShowScores(winds[i].player.connectionToClient, scores, oldScores, names, winds[CurrentWind].player.wind);
        }
    }
    #endregion


    #region refreshing
    public void RefreshAll()
    {
        if (!isServer) return;

        RefreshWinds();
        Debug.Log("Refresh all");
        for (int i = 0; i < winds.Count; i++)
        {
            //winds[i].Refresh();
            Debug.Log(i);
            if (winds[i].player != null)
            {
                Debug.Log("player");
                try
                {
                    winds[i].player.RpcRefresh();

                    winds[i].player.TargetRefresh(winds[i].player.connectionToClient);
                }catch(Exception ex)
                {
                    Debug.Log("ex");
                    continue;
                    
                }
            }
        }



        GameMaster.instance.gameState = "prepare";
    }

    public void RefreshWinds()
    {
        for (int i = 0; i < winds.Count; i++)
        {
            Debug.Log("refresh wind " + i);
            
            winds[i].Refresh();
            Debug.Log(winds[i].freePosition);
        }
    }

    public void OnRefresh()
    {
        RefreshEv?.Invoke();
    }

    public void Refresh()
    {
        Debug.Log("gm refresh");
        wallIsBuilt = false;
        tilesAreGiven = false;
        waitForCombinations = false;

        CurrentWind = 0;
        GameTable = new Table();

        numOfAnsweredPlayers = 0;

        chowDeclarator = null;
        pungDeclarator = null;
        kongDeclarator = null;
        mahJongDeclarator = null;
    }
    #endregion 
}
