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

    int currentWall;
    int currentPair;

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
       int wallNum = Random.Range(0, tiles.Count);
        int restNum = Random.Range(2, 13);

       // int wallNum = 0;
       //int  restNum = 12;

        beginningWall = wallNum;
        beginningPair = restNum;
       

        Debug.Log(wallNum);
        Debug.Log(restNum);

        RpcLieFreeTiles(wallNum, restNum);

    }

    public void DistributeTiles()
    {

        currentWall = beginningWall;
        currentPair = beginningPair;

        Debug.Log(GameManager.instance.winds.Count + "winds");

        for (int i = 0; i < 4; i++)
        {
            if (i < 3)
            {
                for (int windNum = 0; windNum < GameManager.instance.winds.Count; windNum++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        
                        CheckForTheEndOfTheWall(ref currentWall, ref currentPair);

                        RpcMoveUpper(currentWall, currentPair,
                            GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        GameManager.instance.winds[windNum].MoveRightFreePosition(ref GameManager.instance.winds[windNum].freePosition);

                        RpcMoveLower(currentWall, currentPair,
                            GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        GameManager.instance.winds[windNum].MoveRightFreePosition(ref GameManager.instance.winds[windNum].freePosition);

                        if (GameManager.instance.winds[windNum].player != null)
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, currentWall, currentPair,"");

                    }

                }

            }
            else
            {
                for (int windNum = 0; windNum < GameManager.instance.winds.Count; windNum++)
                {

                    if (windNum % 2 == 0)
                    {
                        
                        CheckForTheEndOfTheWall(ref currentWall, ref currentPair);

                        RpcMoveUpper(currentWall, currentPair,
                                GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        if (GameManager.instance.winds[windNum].player != null)
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, currentWall, currentPair,"upper");
                    }
                    else
                    {
                        RpcMoveLower(currentWall, currentPair,
                            GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        if (GameManager.instance.winds[windNum].player != null)
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, currentWall, currentPair,"lower");
                    }

                    GameManager.instance.winds[windNum].MoveRightFreePosition(ref GameManager.instance.winds[windNum].freePosition);
                }
            }
        }

        
        CheckForTheEndOfTheWall(ref currentWall, ref currentPair);
        RpcRefreshCurrentPair(currentWall, currentPair);


    }

    void CheckForTheEndOfTheWall(ref int currentWall,ref int currentPair)
    {
        currentPair++;

        if (currentPair>=tiles[currentWall].Count)
        {
            currentWall++;
            if (currentWall == 4)
                currentWall = 0;
            currentPair = 0;
        }
    }

    [ClientRpc]
    void RpcRefreshCurrentPair(int wallNum,int pairNum)
    {
        currentWall = wallNum;
        currentPair = pairNum;
    }



    ////don't forget
    //public void UpdateWallList()
    //{
    //    int currentWall = beginningWall;
    //    int currentPair = beginningPair;

    //    for (int i = 0; i < 27; i++)
    //    {
    //        RpcRemovePair(currentWall, currentPair);

    //        Debug.Log(tiles[currentWall].Count + "wallcount");
    //        if (currentPair >= tiles[currentWall].Count)
    //        {
    //            currentWall++;
    //            if (currentWall == 4)
    //                currentWall = 0;
    //            currentPair = 0;
    //        }
    //        tiles[currentWall].RemoveAt(currentPair);
            
            
    //    }
    //}

    [TargetRpc]
    void TargetAddTileToPlayerArray(NetworkConnection conn,int currentWall,int currentPair,string tile)
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.CmdAddTileToPlayerArray(currentWall,currentPair,tile);
    }
    
    public void GiveWallTile()
    {
        Wind curWind = GameManager.instance.winds[GameManager.instance.CurrentWind];

        if (!tiles[currentWall][currentPair].upperTile.isOwned)
        {
           
            //curWind.player.tileToMove = tiles[currentWall][currentPair].upperTile;

            RpcMoveUpper(currentWall, currentPair, curWind.freePosition, curWind.rotation);
            if (curWind.player != null)
                TargetAddTileToPlayerArray(curWind.player.connectionToClient, currentWall, currentPair, "upper");

            tiles[currentWall][currentPair].upperTile.isOwned = true;
        }
        else
        {
          
            RpcMoveLower(currentWall, currentPair, curWind.freePosition, curWind.rotation);

            if (curWind.player != null)
                TargetAddTileToPlayerArray(curWind.player.connectionToClient, currentWall, currentPair, "lower");

            CheckForTheEndOfTheWall(ref currentWall,ref currentPair);
        }

        curWind.MoveRightFreePosition(ref curWind.freePosition);

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
        Debug.Log($"cur wall{currentWall},cur pair{currentPair}");
        Debug.Log(tiles[currentWall].Count);
        tiles[currentWall].RemoveAt(currentPair);
    }

    [ClientRpc]
    void RpcLieFreeTiles(int wallNum, int restNum)

    {
        beginningWall = wallNum;
        beginningPair = restNum;

        beginningPair--;

        freeTiles.Add(tiles[wallNum][restNum].upperTile);
        freeTiles.Add(tiles[wallNum][restNum].lowerTile);

        freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][restNum - 2].upperTile.tile, true);
        freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][restNum - 1].upperTile.tile, false);

    }


    public void GiveFreeTile()
    {
        Wind curWind = GameManager.instance.winds[GameManager.instance.CurrentWind];

        TargetAddTileToPlayerArray(curWind.player.connectionToClient, 0, 0, "free");

        curWind.player.tileToMove = freeTiles[freeTiles.Count - 1];

        RpcGiveFreeTile(curWind.freePosition, curWind.rotation);
        Invoke("RemoveFromFreeTiles", 0.5f);
        //curWind.player.needToCheckMoving = true;

        curWind.MoveRightFreePosition(ref curWind.freePosition);
        Debug.Log(curWind.freePosition);


        
        Invoke("CheckFreeTiles", 0.6f);
        
    }

    [ClientRpc]
    void RpcGiveFreeTile(Vector3 freePosition, float rotation)
    {
        freeTiles[freeTiles.Count - 1].tile.GetComponent<BezierMove>().Move(freePosition,rotation);
    }

    void RemoveFromFreeTiles()
    {
        GameManager.instance.winds[GameManager.instance.CurrentWind].player.tileToMove = freeTiles[freeTiles.Count - 1];
        GameManager.instance.winds[GameManager.instance.CurrentWind].player.needToCheckMoving = true;
        RpcRemoveFromFree();
    }
    [ClientRpc]
    void RpcRemoveFromFree()
    {
        freeTiles.RemoveAt(freeTiles.Count - 1);
    }



    void CheckFreeTiles()
    {
        if (freeTiles.Count == 0)
            RpcLieFreeTiles(beginningWall, beginningPair);
    }

}
