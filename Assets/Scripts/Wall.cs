using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wall : NetworkBehaviour
{

    public List<List<WallPair>> tiles;
    public List<Tile> freeTiles = new List<Tile>();
    public static Wall instance = null;
    public List<Tile> tilesOnTheTable;

    int beginningWall;
    int beginningPair;

    public bool isMoving = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);

    }

    public void Initialize(List<List<WallPair>> tiles)
    {
        this.tiles = tiles;

    }

    public void AssighFreeTiles()
    {
       // int wallNum = Random.Range(0, tiles.Count);
        //int restNum = Random.Range(2, 13);

        int wallNum = 0;
       int  restNum = 12;

        beginningWall = wallNum;
        beginningPair = restNum;
       

        Debug.Log(wallNum);
        Debug.Log(restNum);

        RpcLieFreeTiles(wallNum, restNum);

    }

    public void DistributeTiles()
    {

        int currentWall = beginningWall;
        int currentPair = beginningPair;
        Debug.Log(GameManager.instance.winds.Count + "winds");

        for (int i = 0; i < 4; i++)
        {
            if (i < 3)
            {
                for (int windNum = 0; windNum < GameManager.instance.winds.Count; windNum++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        currentPair++;
                        if (currentPair == 18)
                        {
                            currentWall++;
                            if (currentWall == 4)
                                currentWall = 0;
                            currentPair = 0;
                        }

                        RpcMoveUpper(currentWall, currentPair,
                            GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        GameManager.instance.winds[windNum].RefreshFreePosition();

                        RpcMoveLower(currentWall, currentPair,
                            GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        GameManager.instance.winds[windNum].RefreshFreePosition();

                        if (GameManager.instance.winds[windNum].player != null)
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, windNum, currentWall, currentPair,"");

                        // GameManager.instance.winds[windNum].player.playerTiles.Add(tiles[currentWall][currentPair].upperTile);

                        // GameManager.instance.winds[windNum].player.playerTiles.Add(tiles[currentWall][currentPair].lowerTile);

                    }



                }

            }
            else
            {
                for (int windNum = 0; windNum < GameManager.instance.winds.Count; windNum++)
                {

                    if (windNum % 2 == 0)
                    {
                        currentPair++;
                        if (currentPair == 18)
                        {
                            currentWall++;

                            if (currentWall == 4)
                                currentWall = 0;
                            
                            currentPair = 0;
                        }
                        RpcMoveUpper(currentWall, currentPair,
                                GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        if (GameManager.instance.winds[windNum].player != null)
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, windNum, currentWall, currentPair,"upper");
                    }
                    else
                    {
                        RpcMoveLower(currentWall, currentPair,
                            GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        if (GameManager.instance.winds[windNum].player != null)
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, windNum, currentWall, currentPair,"lower");
                    }

                    GameManager.instance.winds[windNum].RefreshFreePosition();
                }
            }
        }

    }



    //don't forget
    void RemoveFromWallList()
    {
        int currentWall = beginningWall;
        int currentPair = beginningPair + 1;

        for (int i = 0; i < 26; i++)
        {
            Debug.Log(tiles[currentWall].Count + "wallcount");
            if (currentPair >= tiles[currentWall].Count)
            {
                currentWall++;
                if (currentWall == 4)
                    currentWall = 0;
                currentPair = 0;
            }
            tiles[currentWall].RemoveAt(currentPair);
            RpcRemovePair(currentWall, currentPair);
        }
    }

    [TargetRpc]
    void TargetAddTileToPlayerArray(NetworkConnection conn,int windNum,int currentWall,int currentPair,string tile)
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if(tile=="upper")
             player.playerTiles.Add(tiles[currentWall][currentPair].upperTile);
        else if(tile=="lower")
             player.playerTiles.Add(tiles[currentWall][currentPair].lowerTile);
        else
        {
            player.playerTiles.Add(tiles[currentWall][currentPair].upperTile);
            player.playerTiles.Add(tiles[currentWall][currentPair].lowerTile);
        }

    }

    [ClientRpc]
    void RpcMoveUpper(int currentWall,int currentPair,Vector3 freePosition,float rotation)
    {

        tiles[currentWall][currentPair].upperTile.tile.GetComponent<BezierMove>().
                            Move(freePosition, rotation);
    }

    [ClientRpc]
    void RpcMoveLower(int currentWall, int currentPair, Vector3 freePosition, float rotation)
    {
        tiles[currentWall][currentPair].lowerTile.tile.GetComponent<BezierMove>().
             Move(freePosition,
             rotation);
    }

    [ClientRpc]
    void RpcRemovePair(int currentWall, int currentPair)
    {

        tiles[currentWall].RemoveAt(currentPair);
    }

    [ClientRpc]
    void RpcLieFreeTiles(int wallNum, int restNum)

    {

        beginningWall = wallNum;
        beginningPair = restNum;

        freeTiles.Add(tiles[wallNum][restNum].upperTile);
        freeTiles.Add(tiles[wallNum][restNum].lowerTile);

        freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][restNum - 2].upperTile.tile, true);
        freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][restNum - 1].upperTile.tile, false);

    }

    private void Update()
    {
        if (!isServer) return;

        
        
    }

}
