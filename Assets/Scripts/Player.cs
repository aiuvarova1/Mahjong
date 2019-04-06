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

    Tile selectedTile;

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

    public void SelectTile()
    {
        if (!isLocalPlayer) return;
        Debug.Log("select");
    }

    [ClientRpc]
    public void RpcLieOutTile(int index,Vector3 freePosition,float rotation,string array)
    {
        Debug.Log(playerTiles.Count);
        playerTiles[index].tile.GetComponent<BezierMove>().LieOut(freePosition,rotation);

        tileToMove = playerTiles[index];
        needToCheckMoving = true;
        needFreeTile = true;

        switch (array)
        {
            case "flowers":
                flowers.Add(playerTiles[index]);
                break;
        }

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
                GameManager.instance.winds[0].player=this;
                return;
            case "South":
                GameManager.instance.winds[1].player = this ;
                return;
            case "West":
                GameManager.instance.winds[2].player=this;
                return;
            case "North":
                GameManager.instance.winds[3].player=this;
                return;
            default:

                return;

        }
    }

    [Command]
    public void CmdAddTileToPlayerArray(int currentWall, int currentPair, string tile)
    {
        RpcAddTileToPlayerArray(currentWall,currentPair,tile);
    }

    [ClientRpc]
    public void RpcAddTileToPlayerArray( int currentWall, int currentPair, string tile)
    {
       
        if (tile == "upper")
            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].upperTile);
        else if (tile == "lower")
            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].lowerTile);
        else if(tile=="")
        {
            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].upperTile);
            playerTiles.Add(Wall.instance.tiles[currentWall][currentPair].lowerTile);
        }
        else if (tile == "free")
        {
            playerTiles.Add(Wall.instance.freeTiles[Wall.instance.freeTiles.Count - 1]);
            
        }

    }

    public void SelectTile(GameObject tile)
    {
        Tile tileToSelect = CheckList(tile);
        if ( tileToSelect== null) return;
        Debug.Log("my tile");
        //if (selectedTile != tileToSelect)
        // {
        Debug.Log(selectedTile != tileToSelect);
            tileToSelect.tile.GetComponent<BezierMove>().SelectTile();
        if(selectedTile!=null)
            selectedTile.tile.GetComponent<BezierMove>().DeselectTile();
        selectedTile = tileToSelect;
       // }
       


    }


    Tile CheckList(GameObject tile)
    {
        foreach (Tile playerTile in playerTiles)
        {
            if (playerTile.tile == tile) return playerTile;
        }
        return null;
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
            if(!CheckTileMoving(tileToMove))
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
                Invoke("Sort", 1.2f);
                Invoke("GetNextFlower", 1.5f);
                

            }

        }
    }

    void GetNextFlower()
    {
        GameManager.instance.CheckForFlowers();
    }

}
