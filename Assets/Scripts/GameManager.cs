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

    public int numOfAnsweredPlayers;
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

        winds.Add(new East());
        winds.Add(new South());
        winds.Add(new West());
        winds.Add(new North());

        CurrentWind = 0;
        GameTable = new Table();


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
        Wall.instance.AssighFreeTiles();

        Wall.instance.DistributeTiles();

        Invoke("SortTiles", 3f);
        Invoke("FixTilesPositions",3.2f);

        Invoke("CheckAllPlayersForFlowers", 3.5f);
    }

    void FixTilesPositions()
    {
        for(int i = 0; i < winds.Count; i++)
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
        
        Debug.Log(currentWind+"cur");
        //may need fix
        if (currentWind>=4)
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
                Invoke("BeginPlayerMove", 0.3f);
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
        curPlayer.playerTurn = true;
        TargetSetTurn(curPlayer.connectionToClient,true);

    }

    [TargetRpc]
    public void TargetSetTurn(NetworkConnection conn, bool turn)
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().playerTurn = turn;
    }


    public void PrepareForCombinations()
    {
        Invoke("Prepare", 2);

    }

    void Prepare()
    {
        Debug.Log("prepare for comb");

        numOfAnsweredPlayers = 0;
        chowDeclarator = null;
        pungDeclarator = null;
        kongDeclarator = null;
        mahJongDeclarator = null;

        waitForCombinations = true;
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
        if (waitForCombinations && GameMaster.playersToStart - 1 == numOfAnsweredPlayers)
        {
            Debug.Log("here");

            if (mahJongDeclarator != null)
            {
                mahJongDeclarator.DeclareMahJong(DefineWind(mahJongDeclarator));
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
        }
        //if(waitForCombinations)
        //    Debug.Log("server");
    }
    #endregion

    public void DeclareCombination(string combination)
    {
        string wind = winds[CurrentWind].Name;
        foreach (Wind playerWind in winds)
        {
            if (playerWind.player != null && playerWind.Name != wind)
                playerWind.player.gameObject.GetComponent<PlayerUI>().TargetShowDeclaredCombination(playerWind.player.connectionToClient,wind, combination);
        }
    }
}
