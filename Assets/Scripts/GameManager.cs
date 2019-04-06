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

    public List<Wind> winds = new List<Wind>();

    public Table GameTable { get; set; }

    float timeToWait = 0;

    int currentWind;

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

        Invoke("CheckAllPlayersForFlowers", 3.5f);
    }



     void CheckAllPlayersForFlowers()
    {
        if (!isServer) return;
        
        Debug.Log(currentWind+"cur");
        //may need fix
        if (currentWind>=4)
        {
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
                Invoke("BeginPlayerMove", 0.2f);
            return;
        }

        Debug.Log(player.playerTiles.Count);
        player.RpcLieOutTile(player.playerTiles.Count - 1, winds[currentWind].freeFlowerPosition, winds[currentWind].rotation, "flowers");
        //player.playerTiles.RemoveAt(player.playerTiles.Count - 1);

        winds[currentWind].MoveRightFreePosition(ref winds[currentWind].freeFlowerPosition);
        winds[currentWind].MoveLeftFreePosition(ref winds[currentWind].freePosition);

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

    public void InvokeChange()
    {
        Invoke("ChangeTurn", 1.6f);
    }

    public void ChangeTurn()
    {
        Wall.instance.GiveWallTile();

        //Invoke("CheckForFlowers", 1f);
    }


    void BeginPlayerMove()
    {
        Player curPlayer = winds[CurrentWind].player;
        if (curPlayer == null) return;

        curPlayer.TargetSelectLastTile(curPlayer.connectionToClient);
        TargetSetTurn(curPlayer.connectionToClient);

    }

    [TargetRpc]
    void TargetSetTurn(NetworkConnection conn)
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().playerTurn = true;
    }

}
