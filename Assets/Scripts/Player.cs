using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public List<Tile> playerTiles = new List<Tile>();
    public List<Tile> openedTiles = new List<Tile>();
    public List<Tile> flowers = new List<Tile>();

    public Vector3 startPosition;

    public Camera Camera { get; set; }
    string name;
    public string wind;
    public int order;

    public Tile tileToMove;
    public bool needToCheckMoving = false;
    bool needFreeTile = false;

    public bool playerTurn = false;
    bool startedCoroutine = false;

    public Tile selectedTile;

    Vector3 freeSpacePosition;
    int freeSpaceIndex;

    //delegate void 

    void Sort()
    {
        RpcSort();
    }

    [ClientRpc]
    public void RpcSort()
    {
        Debug.Log("rpc");
        SortTiles();

    }

    public void SortTiles()
    {
        List<Vector3> positions = CreatePositionList();

        Debug.Log(playerTiles.Count + "player tiles");
        Debug.Log(playerTiles[playerTiles.Count - 1].name);
        playerTiles.Sort();
        Debug.Log("sorted");

        //playerTiles[0].tile.transform.position = startPosition;
        for (int i = 0; i < playerTiles.Count; i++)
        {
            // Debug.Log(playerTiles[i]);
            playerTiles[i].tile.transform.position = positions[i];
            // Debug.Log($"{i}, {playerTiles[i].name}");
        }
    }

    //public void SelectTile()
    //{
    //    if (!isLocalPlayer) return;
    //    Debug.Log("select");
    //}

    [ClientRpc]
    public void RpcLieOutTile(int index, Vector3 freePosition, float rotation, string array)
    {
        Debug.Log(playerTiles.Count);
        playerTiles[index].tile.GetComponent<BezierMove>().LieOut(freePosition, rotation);

        tileToMove = playerTiles[index];

        switch (array)
        {
            case "flowers":
                needToCheckMoving = true;
                needFreeTile = true;
                flowers.Add(playerTiles[index]);
                break;
            case "table":
                GameManager.instance.GameTable.tableTiles.Add(playerTiles[index]);
                if(isLocalPlayer)
                    gameObject.GetComponent<PlayerUI>().StopWaitingForMove();

                break;
        }
        EnableToolTipsForTile(index);

        playerTiles.RemoveAt(index);
        CmdRemoveFromArray(index);

        //CheckTileMoving(playerTiles[index]);

        Debug.Log("lie out");

    }

    [Command]
    void CmdRemoveFromArray(int index)
    {
        // playerTiles.RemoveAt(index);
        Debug.Log(playerTiles.Count);
    }

    [ClientRpc]
    public void RpcRemoveFromArray(int index)
    {
        playerTiles.RemoveAt(index);
    }

    List<Vector3> CreatePositionList()
    {
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < playerTiles.Count; i++)
        {
            pos.Add(playerTiles[i].tile.transform.position);
        }
        return pos;
    }

    public bool CheckForFlowers()
    {

        foreach (Tile tile in playerTiles)
        {

            if (tile.name.Contains("f"))
            {
                Debug.Log(tile.name);
                return true;
            }
        }
        return false;

    }

    [Command]
    public void CmdAddWind()
    {
        switch (wind)
        {
            case "East":
                GameManager.instance.winds[0].player = this;
                return;
            case "South":
                GameManager.instance.winds[1].player = this;
                return;
            case "West":
                GameManager.instance.winds[2].player = this;
                return;
            case "North":
                GameManager.instance.winds[3].player = this;
                return;
            default:

                return;

        }
    }

    [Command]
    public void CmdAddTileToPlayerArray(int currentWall, int currentPair, string tile)
    {
        RpcAddTileToPlayerArray(currentWall, currentPair, tile);
    }

    

    public void EnableToolTipsForTile(int index)
    {
        ToolTip tip = playerTiles[index].tile.GetComponent<ToolTip>();
        tip.enabled = true;

    }


    [ClientRpc]
    public void RpcAddTileToPlayerArray(int currentWall, int currentPair, string tile)
    {

        if (tile == "upper")
            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].upperTile);
        else if (tile == "lower")
            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].lowerTile);
        else if (tile == "")
        {
            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].upperTile);
            if (isLocalPlayer)
                EnableToolTipsForTile(playerTiles.Count - 1);

            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].lowerTile);
        }
        else if (tile == "free")
        {
            playerTiles.Add(Wall.instance.freeTiles[Wall.instance.freeTiles.Count - 1]);

        }

        if (isLocalPlayer)
            EnableToolTipsForTile(playerTiles.Count - 1);

    }

    public void SelectTile(GameObject tile)
    {
        int index = CheckList(tile);
        if (!playerTurn || !startedCoroutine || index ==-1) return;

        Tile tileToSelect = playerTiles[index];
        Debug.Log("my tile");
        if (selectedTile != tileToSelect)
        {

            tileToSelect.tile.GetComponent<BezierMove>().SelectTile();
            if (selectedTile != null)
                selectedTile.tile.GetComponent<BezierMove>().DeselectTile();
            selectedTile = tileToSelect;
        }
        else
        {
            CmdLieTileOnTable(index);
            playerTurn = false;
        }
    }

    [Command]
    public void CmdLieTileOnTable(int index)
    {
        freeSpacePosition = playerTiles[index].tile.transform.position;
        freeSpacePosition.y = 1.2f;
        freeSpaceIndex = index;

        Table t = GameManager.instance.GameTable;
        RpcLieOutTile(index, t.CurrentPosition, t.rotation, "table");
        t.MoveRightStartPosition();

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;


        Invoke("InvokeDelete",0.5f);
    }


    void InvokeDelete()
    {
        Wind wind = GameManager.instance.winds[GameManager.instance.CurrentWind];


        for (int i = freeSpaceIndex; i < playerTiles.Count; i++)
        {
            Debug.Log(playerTiles[i].tile == null);
            RpcDeleteFreeSpace(wind.freePosition,i);
            wind.MoveRightFreePosition(ref wind.freePosition);
        }
        Invoke("Sort", 0.1f);
        
    }

    [ClientRpc]
    void RpcDeleteFreeSpace(Vector3 position,int index)
    {
        //Debug.Log(tile==null);
        playerTiles[index].tile.transform.position = position;
    }

    [TargetRpc]
    public void TargetSelectLastTile(NetworkConnection conn)
    {
        playerTiles[playerTiles.Count - 1].tile.GetComponent<BezierMove>().SelectTile();
        selectedTile = playerTiles[playerTiles.Count - 1];

    }


    int CheckList(GameObject tile)
    {
        for (int i = 0; i < playerTiles.Count; i++)
        {
            if (playerTiles[i].tile == tile) return i;
        }
        //foreach (Tile playerTile in playerTiles)
        //{
        //    if (playerTile.tile == tile) return playerTile;
        //}
        return -1;
    }

    bool CheckForMoving()
    {
        foreach (Tile tile in playerTiles)
        {
            if (CheckTileMoving(tile)) return true;
        }
        return false;
    }

    bool CheckTileMoving(Tile tile)
    {
        return tile.tile.GetComponent<BezierMove>().moving;
    }


    private void Update()
    {
        //if (!isServer) Debug.Log("no");

        if (isServer && needToCheckMoving)
        {
            //Debug.Log(tileToMove.name);
            //Debug.Log(CheckTileMoving(tileToMove));
            if (!CheckTileMoving(tileToMove))
            {
                Debug.Log(needFreeTile);
                if (needFreeTile)
                {
                    if (Wall.instance.freeTiles.Count == 0) return;

                    Wall.instance.GiveFreeTile();
                    needFreeTile = false;
                    needToCheckMoving = false;
                    return;
                }
                needToCheckMoving = false;
                if (GameMaster.instance.gameState == "starting" || GameMaster.instance.gameState == "distributing")
                {
                    Invoke("Sort", 1.2f);
                    Invoke("GetNextFlower", 1.5f);
                }
                else
                {
                    Debug.Log("not moving");
                    Invoke("GetNextFlower", 0.01f);
                }


            }

        }


        if (isLocalPlayer && playerTurn && !startedCoroutine)
        {
            gameObject.GetComponent<PlayerUI>().LaunchWaitForMove();
            startedCoroutine = true;
        }
    }

    void GetNextFlower()
    {
        GameManager.instance.CheckForFlowers();
    }

}
