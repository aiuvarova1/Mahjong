using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wall : NetworkBehaviour
{
    public List<List<WallPair>> tiles;
    public List<Tile> freeTiles = new List<Tile>();
    public static Wall instance = null;

    public int beginningWall;
    public int beginningPair;

    int currentWall;
    int currentPair;

    public bool freeTileIsMoving = false;

    //to avoid wrong current wind
    int currentWind;
    int freeTilesCount = 2;

    bool checkFreeTiles = false;

    //initialization
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

    //assigns random place of opening wall
    public void AssighFreeTiles()
    {
        int wallNum = Random.Range(0, tiles.Count);
        int restNum = Random.Range(2, 13);

        beginningWall = wallNum;
        beginningPair = restNum;

        RpcLieFreeTiles(wallNum, restNum);
    }

    //disctributes tiles to players
    public void DistributeTiles()
    {
        currentWall = beginningWall;
        currentPair = beginningPair;

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

    //controls wall and tile numbers
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

    //adds tile to player's array on one client
    [TargetRpc]
    void TargetAddTileToPlayerArray(NetworkConnection conn, int currentWall, int currentPair, string tile)
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.CmdAddTileToPlayerArray(currentWall, currentPair, tile);
    }

    //moves wall tile to player
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

            if (curWind.player != null)
                TargetAddTileToPlayerArray(curWind.player.connectionToClient, currentWall, currentPair, "lower");

            CheckForTheEndOfTheWall(ref currentWall, ref currentPair);
        }

        curWind.MoveRightFreePosition(ref curWind.freePosition);
    }

    //checks whether it is draw or not
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

    //moves upper tile from the pair
    [ClientRpc]
    void RpcMoveUpper(int currentWall, int currentPair, Vector3 freePosition, float rotation)
    {

        tiles[currentWall][currentPair].upperTile.tile.GetComponent<BezierMove>().
                            Move(freePosition, rotation);
    }

    //moves lower tile from the pair
    [ClientRpc]
    void RpcMoveLower(int currentWall, int currentPair, Vector3 freePosition, float rotation)
    {
        tiles[currentWall][currentPair].lowerTile.tile.GetComponent<BezierMove>().
             Move(freePosition,
             rotation);
    }

    //indicates new free tiles
    [ClientRpc]
    void RpcLieFreeTiles(int wallNum, int restNum)
    {
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

        freeTiles.Add(tiles[wallNum][restNum].upperTile);
        freeTiles.Add(tiles[wallNum][restNum].lowerTile);

        checkFreeTiles = true;

        freeTileIsMoving = false;
        CheckAndFill(wallNum, restNum);
    }

    //moves new free tiles
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
            freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[prevWall][17].upperTile.tile.transform.position, true, newRotation);
            freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][num - 1].upperTile.tile.transform.position, false, oldRotation);
        }
        else if (num == 0)
        {
            freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[prevWall][16].upperTile.tile.transform.position, true, newRotation);
            freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[prevWall][17].upperTile.tile.transform.position, false, newRotation);
        }
    }

    //gives free tile to the player
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

        RpcGiveFreeTile(curWind.freePosition, curWind.rotation);

        curWind.MoveRightFreePosition(ref curWind.freePosition);
    }

    //moves free tile to player on clients
    [ClientRpc]
    void RpcGiveFreeTile(Vector3 freePosition, float rotation)
    {
        freeTiles[freeTiles.Count - 1].tile.GetComponent<BezierMove>().Move(freePosition, rotation);
    }

    //checks whether free tiles ended
    void CheckFreeTiles()
    {
        if (freeTilesCount == 0)
        {
            freeTilesCount = 2;
            freeTileIsMoving = true;
            RpcLieFreeTiles(beginningWall, beginningPair);
        }
    }

    //controls free tiles ending
    private void Update()
    {
        if (!isServer) return;

        if (checkFreeTiles && freeTiles.Count == 0)
        {
            checkFreeTiles = false;
            CheckFreeTiles();
        }
    }

    //removes all tiles and refreshes all data
    public void Refresh()
    {
        if (tiles == null) return;
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = 0; j < tiles[i].Count; j++)
            {
                Destroy(tiles[i][j].upperTile.tile);
                Destroy(tiles[i][j].lowerTile.tile);
            }
        }
        tiles.Clear();

        freeTiles = new List<Tile>();
        freeTileIsMoving = false;

        freeTilesCount = 2;
        checkFreeTiles = false;
    }
}
