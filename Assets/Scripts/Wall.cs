using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wall : NetworkBehaviour
{

    public List<List<WallPair>> tiles;
    public List<Tile> freeTiles = new List<Tile>();
    public static Wall instance = null;
    //public List<Tile> tilesOnTheTable;

    public int beginningWall;
    public int beginningPair;

    int currentWall;
    int currentPair;

    public bool isMoving = false;

    public bool freeTileIsMoving = false;

    //to avoid wrong current wind
    int currentWind;
    int freeTilesCount = 2;

    bool checkFreeTiles = false;

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
        // int restNum = Random.Range(2, 13);

        int wallNum = 0;
        int restNum = 2;

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
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, currentWall, currentPair, "");

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
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, currentWall, currentPair, "upper");
                    }
                    else
                    {
                        RpcMoveLower(currentWall, currentPair,
                            GameManager.instance.winds[windNum].freePosition, GameManager.instance.winds[windNum].rotation);

                        if (GameManager.instance.winds[windNum].player != null)
                            TargetAddTileToPlayerArray(GameManager.instance.winds[windNum].player.connectionToClient, currentWall, currentPair, "lower");
                    }

                    GameManager.instance.winds[windNum].MoveRightFreePosition(ref GameManager.instance.winds[windNum].freePosition);
                }
            }
        }


        CheckForTheEndOfTheWall(ref currentWall, ref currentPair);
        RpcRefreshCurrentPair(currentWall, currentPair);


    }

    void CheckForTheEndOfTheWall(ref int currentWall, ref int currentPair)
    {
        currentPair++;

        if (currentPair >= tiles[currentWall].Count)
        {
            currentWall++;
            if (currentWall == 4)
                currentWall = 0;
            currentPair = 0;
        }
    }

    [ClientRpc]
    void RpcRefreshCurrentPair(int wallNum, int pairNum)
    {
        currentWall = wallNum;
        currentPair = pairNum;
    }


    [TargetRpc]
    void TargetAddTileToPlayerArray(NetworkConnection conn, int currentWall, int currentPair, string tile)
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.CmdAddTileToPlayerArray(currentWall, currentPair, tile);
        // player.InvokeToolTip();
    }

    public void GiveWallTile()
    {
        Wind curWind = GameManager.instance.winds[GameManager.instance.CurrentWind];
        if (curWind.player == null) return;

        if (CheckDraw(currentWall, currentPair))
        {
            GameManager.instance.DeclareDraw();
            return;
        }

        if (!tiles[currentWall][currentPair].upperTile.isOwned)
        {

            curWind.player.tileToMove = tiles[currentWall][currentPair].upperTile;

            tiles[currentWall][currentPair].upperTile.tile.GetComponent<BezierMove>().check = true;
            tiles[currentWall][currentPair].upperTile.tile.GetComponent<BezierMove>().owner = curWind.player;

            RpcMoveUpper(currentWall, currentPair, curWind.freePosition, curWind.rotation);
            //Invoke("SetMoving", 0.5f);

            if (curWind.player != null)
                TargetAddTileToPlayerArray(curWind.player.connectionToClient, currentWall, currentPair, "upper");

            tiles[currentWall][currentPair].upperTile.isOwned = true;
        }
        else
        {

            curWind.player.tileToMove = tiles[currentWall][currentPair].lowerTile;

            tiles[currentWall][currentPair].lowerTile.tile.GetComponent<BezierMove>().check = true;
            tiles[currentWall][currentPair].lowerTile.tile.GetComponent<BezierMove>().owner = curWind.player;

            RpcMoveLower(currentWall, currentPair, curWind.freePosition, curWind.rotation);
            //!
            //Invoke("SetMoving", 0.5f);

            if (curWind.player != null)
                TargetAddTileToPlayerArray(curWind.player.connectionToClient, currentWall, currentPair, "lower");

            CheckForTheEndOfTheWall(ref currentWall, ref currentPair);

        }

        curWind.MoveRightFreePosition(ref curWind.freePosition);

    }

    bool CheckDraw(int wall, int pair)
    {
        int curWall = beginningWall;
        int curPair = beginningPair;

        for (int i = 0; i < 6; i++)
        {
            curPair--;
            if (curPair < 0)
            {
                curPair = 17;
                curWall--;
                if (curWall < 0)
                    curWall = 3;
            }
        }

        if (wall == curWall && pair == curPair)
            return true;
        return false;

    }



    [ClientRpc]
    void RpcMoveUpper(int currentWall, int currentPair, Vector3 freePosition, float rotation)
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

        // Debug.Log("free tile is moving in rpc" + freeTileIsMoving);
        beginningWall = wallNum;
        beginningPair = restNum;

        beginningPair--;
        if (beginningPair < 0)
        {
            beginningPair = 17;
            beginningWall--;
            if (beginningWall < 0)
                beginningWall = 3;
        }

        Debug.Log($"wallNum {wallNum}, restNum {restNum}");
        freeTiles.Add(tiles[wallNum][restNum].upperTile);
        freeTiles.Add(tiles[wallNum][restNum].lowerTile);

        checkFreeTiles = true;

        freeTileIsMoving = false;
        // tiles[wallNum][restNum].upperTile.tile.transform.Rotate(0, 45, 0);
        CheckAndFill(wallNum, restNum);
    }

    void CheckAndFill(int wallNum, int num)
    {

        int prevWall = wallNum - 1;
        if (prevWall < 0) prevWall = 3;

        float newRotation = tiles[prevWall][0].lowerTile.tile.transform.eulerAngles.y;
        float oldRotation = tiles[wallNum][0].lowerTile.tile.transform.eulerAngles.y;


        if (num >= 2)
        {
            freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][num - 2].upperTile.tile.transform.position, true, oldRotation);
            freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][num - 1].upperTile.tile.transform.position, false, oldRotation);

        }
        else if (num == 1)
        {
            //Debug.Log(rotation);
            //freeTiles[1].tile.transform.rotation = Quaternion.Euler(0, rotation, 0);
            Debug.Log(freeTiles[1].tile.transform.rotation.y);
            freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[prevWall][17].upperTile.tile.transform.position, true, newRotation);

            freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][num - 1].upperTile.tile.transform.position, false, oldRotation);


        }
        else if (num == 0)
        {
            freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[prevWall][16].upperTile.tile.transform.position, true, newRotation);
            freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[prevWall][17].upperTile.tile.transform.position, false, newRotation);


        }
    }




    public void GiveFreeTile()
    {
        freeTilesCount--;
        Wind curWind = GameManager.instance.winds[GameManager.instance.CurrentWind];
        currentWind = GameManager.instance.CurrentWind;
        curWind.player.isFreeTile = true;

        TargetAddTileToPlayerArray(curWind.player.connectionToClient, 0, 0, "free");

        curWind.player.tileToMove = freeTiles[freeTiles.Count - 1];

        freeTiles[freeTiles.Count - 1].tile.GetComponent<BezierMove>().check = true;
        freeTiles[freeTiles.Count - 1].tile.GetComponent<BezierMove>().owner = GameManager.instance.winds[GameManager.instance.CurrentWind].player;

        GameManager.instance.winds[currentWind].player.tileToMove = freeTiles[freeTiles.Count - 1];


        // Invoke("GiveFreeTiles", 0.01f);
        RpcGiveFreeTile(curWind.freePosition, curWind.rotation);
        //Invoke("RemoveFromFreeTiles", 0.5f);
        //curWind.player.needToCheckMoving = true;

        curWind.MoveRightFreePosition(ref curWind.freePosition);
        // Debug.Log(curWind.freePosition);


        //was 0.6


        // Invoke("CheckFreeTiles", 0.6f);

    }

    [ClientRpc]
    void RpcGiveFreeTile(Vector3 freePosition, float rotation)
    {

        freeTiles[freeTiles.Count - 1].tile.GetComponent<BezierMove>().Move(freePosition, rotation);
    }

    void GiveFreeTiles()
    {
        Wind curWind = GameManager.instance.winds[GameManager.instance.CurrentWind];
        // Debug.Log(freeTiles.Count + "free tile");
        RpcGiveFreeTile(curWind.freePosition, curWind.rotation);
        curWind.MoveRightFreePosition(ref curWind.freePosition);

        //GameManager.instance.winds[GameManager.instance.CurrentWind].player.tileToMove = freeTiles[freeTiles.Count - 1];
        //GameManager.instance.winds[currentWind].player.tileToMove = freeTiles[freeTiles.Count - 1];
        ////SetMoving();
        //RpcRemoveFromFree();
    }

    void SetMoving()
    {
        GameManager.instance.winds[GameManager.instance.CurrentWind].player.needToCheckMoving = true;
    }
    [ClientRpc]
    void RpcRemoveFromFree()
    {
        freeTiles.RemoveAt(freeTiles.Count - 1);
    }



    void CheckFreeTiles()
    {
        Debug.Log(freeTiles.Count + "check");
        Debug.Log(freeTilesCount);
        if (freeTilesCount == 0)
        {
            freeTilesCount = 2;
            freeTileIsMoving = true;
            RpcLieFreeTiles(beginningWall, beginningPair);

        }
    }

    private void Update()
    {
        if (!isServer) return;

        if (checkFreeTiles && freeTiles.Count == 0)
        {
            checkFreeTiles = false;
            CheckFreeTiles();
        }
    }

    public void Refresh()
    {
        Debug.Log("wall refreah");
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = 0; j < tiles[i].Count; j++)
            {
                Destroy(tiles[i][j].upperTile.tile);
                Destroy(tiles[i][j].lowerTile.tile);
            }
        }
        freeTiles = new List<Tile>();

        //public List<Tile> tilesOnTheTable;


        isMoving = false;

        freeTileIsMoving = false;

        freeTilesCount = 2;

        checkFreeTiles = false;
    }
}
