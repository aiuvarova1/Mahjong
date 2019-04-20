using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class Player : NetworkBehaviour
{
    public List<Tile> playerTiles = new List<Tile>();

    public List<Combination> openedTiles = new List<Combination>();

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
    public bool turnForCombination = false;

    public Tile selectedTile;

    Vector3 freeSpacePosition;
    int freeSpaceIndex;

    public Combination waitingCombination;

    //void 


    #region sorting
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

    [ClientRpc]
    public void RpcFix()
    {
        for (int i = 0; i < playerTiles.Count; i++)
        {
            playerTiles[i].tile.transform.position = new Vector3(playerTiles[i].tile.transform.position.x, 1.2f, playerTiles[i].tile.transform.position.z);
        }
    }

    public void SortTiles()
    {
        List<Vector3> positions = CreatePositionList();

        Debug.Log(isServer);

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
    List<Vector3> CreatePositionList()
    {
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < playerTiles.Count; i++)
        {
            pos.Add(playerTiles[i].tile.transform.position);
        }
        return pos;
    }
    #endregion

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
                //GameManager.instance.GameTable.tableTiles.Add(playerTiles[index]);
                GameManager.instance.GameTable.lastTile = playerTiles[index];
                if (isLocalPlayer)
                {
                    gameObject.GetComponent<PlayerUI>().StopWaitingForMove();
                    CmdPrepareForCombinations();

                    //!
                    playerTurn = false;
                }
                else
                {
                    Debug.Log("Launch wait");
                    GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerUI>().LaunchWaitForCombination();
                    //gameObject.GetComponent<PlayerUI>().LaunchWaitForCombination();
                   
                }

                break;


            default:
                break;

        }
        EnableToolTipsForTile(index);

        
           playerTiles.RemoveAt(index);
        //!!!
        //CmdRemoveFromArray(index);
        Debug.Log(playerTiles.Count);

        //CheckTileMoving(playerTiles[index]);

        Debug.Log("lie out");

    }

    [ClientRpc]
    void RpcTakeTableTile(Vector3 freePosition, float rotation)
    {
        GameManager.instance.GameTable.lastTile.tile.GetComponent<BezierMove>().LieOut(freePosition, rotation);
    }




    //[Command]
    //void CmdRemoveFromArray(int index)
    //{
    //    // playerTiles.RemoveAt(index);
    //    Debug.Log(playerTiles.Count);
    //}

    //[ClientRpc]
    //public void RpcRemoveFromArray(int index)
    //{
    //    playerTiles.RemoveAt(index);
    //}

    #region lying tiles

    [Command]
    public void CmdAddTileToPlayerArray(int currentWall, int currentPair, string tile)
    {
        RpcAddTileToPlayerArray(currentWall, currentPair, tile);
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


        Invoke("InvokeDelete", 0.5f);
    }


    void InvokeDelete()
    {
        Wind wind = GameManager.instance.winds[GameManager.instance.CurrentWind];


        for (int i = freeSpaceIndex; i < playerTiles.Count; i++)
        {

            RpcDeleteFreeSpace(wind.freePosition, i);
            wind.MoveRightFreePosition(ref wind.freePosition);
        }
        Invoke("Sort", 0.1f);

    }

    [ClientRpc]
    void RpcDeleteFreeSpace(Vector3 position, int index)
    {
        //Debug.Log(tile==null);
        playerTiles[index].tile.transform.position = position;

    }
    #endregion
    #region SelectTile

    public void SelectTile(GameObject tile)
    {
        int index = CheckList(tile);
        if (!playerTurn || !startedCoroutine || index == -1) return;

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

    [TargetRpc]
    public void TargetSelectLastTile(NetworkConnection conn)
    {

        Invoke("InvokeSelect", 0.2f);
    }

    public void InvokeSelect()
    {
        playerTiles[playerTiles.Count - 1].tile.GetComponent<BezierMove>().SelectTile();
        selectedTile = playerTiles[playerTiles.Count - 1];
    }
    #endregion


    int CheckList(GameObject tile)
    {
        for (int i = 0; i < playerTiles.Count; i++)
        {
            if (playerTiles[i].tile == tile) return i;
        }

        return -1;
    }

    int CheckForTileInArray(string name)
    {
        for (int i = 0; i < playerTiles.Count; i++)
        {
            if (playerTiles[i].name == name) return i;
        }
        return -1;
    }

    //!!!!!
    public void Pass()
    {
        if (!isLocalPlayer) return;

        gameObject.GetComponent<PlayerUI>().StopWaitingForCombination();
        CmdAnswerToServer();
    }

    [Command]
    void CmdAnswerToServer()
    {
        GameManager.instance.numOfAnsweredPlayers++;
    }
    #region checkCombinations

    int FindNumOfSimilarTiles(string name, ref int firstIndex)
    {
        int num = 0;
        for (int i = 0; i < playerTiles.Count; i++)
        {
            if (playerTiles[i].name == name)
            {
                num++;
                if (num == 1) firstIndex = i;

            }
        }
        return num;
    }

    public void CheckChow()
    {
        if (isLocalPlayer)
            CmdCheckChow();
    }

    [Command]
    void CmdCheckChow()
    {
        //if (!isServer) return;
        Debug.Log("check chow");
        if (!turnForCombination)
        {
            Debug.Log("not your turn");
            return;
        }

        int nextWind = GameManager.instance.CurrentWind + 1;

        if (nextWind > 3)
            nextWind = 0;

        if (GameManager.instance.winds[nextWind].player != this)
        {
            Debug.Log("not next player");
            GameManager.instance.numOfAnsweredPlayers++;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            return;

        }

        Chow firstCombination = null;
        Chow secondCombination = null;
        Chow thirdCombination = null;

        Tile tileInDemand = GameManager.instance.GameTable.lastTile;

        string tileName = tileInDemand.name;
        string tileSuit = tileName[0].ToString();
        Debug.Log("demand name" + tileName);
        Debug.Log("demand suit" + tileSuit);

        int tileNum = 0;
        if (!int.TryParse(tileName[tileName.Length - 1].ToString(), out tileNum))
        {
            GameManager.instance.numOfAnsweredPlayers++;
            Debug.Log("not suit");
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            return;
        }
        Debug.Log(tileNum + "tilenum");

        if (tileNum - 1 > 0 && tileNum - 2 > 0)
        {
            string firstName = tileSuit + (tileNum - 2).ToString();
            string secondName = tileSuit + (tileNum - 1).ToString();

            Debug.Log(firstName);
            Debug.Log(secondName);

            int firstInd = CheckForTileInArray(firstName);
            int secondInd = CheckForTileInArray(secondName);
            Debug.Log("first ind" + firstInd);
            Debug.Log("second ind" + secondInd);

            if (firstInd != -1 && secondInd != -1)
            {
                firstCombination = new Chow(playerTiles[firstInd], playerTiles[secondInd], tileInDemand, firstInd);
                //firstCombination = $"{tileNum - 2}-{tileNum - 1}-{tileNum}";
            }
        }
        if (tileNum - 1 > 0 && tileNum + 1 < 10)
        {
            string firstName = tileSuit + (tileNum - 1).ToString();
            string secondName = tileSuit + (tileNum + 1).ToString();
            Debug.Log(firstName);
            Debug.Log(secondName);

            int firstInd = CheckForTileInArray(firstName);
            int secondInd = CheckForTileInArray(secondName);

            if (firstInd != -1 && secondInd != -1)
            {
                if (firstCombination == null)
                    firstCombination = new Chow(playerTiles[firstInd], tileInDemand, playerTiles[secondInd], firstInd);
                //firstCombination = $"{tileNum - 1}-{tileNum}-{tileNum+1}";

                else
                    secondCombination = new Chow(playerTiles[firstInd], tileInDemand, playerTiles[secondInd], firstInd);

            }
        }
        if (tileNum + 1 < 10 && tileNum + 2 < 10)
        {
            string firstName = tileSuit + (tileNum + 1).ToString();
            string secondName = tileSuit + (tileNum + 2).ToString();

            Debug.Log(firstName);
            Debug.Log(secondName);

            int firstInd = CheckForTileInArray(firstName);
            int secondInd = CheckForTileInArray(secondName);

            if (firstInd != -1 && secondInd != -1)
            {
                if (firstCombination == null)
                    firstCombination = new Chow(tileInDemand, playerTiles[firstInd], playerTiles[secondInd], firstInd);
                //firstCombination = $"{tileNum}-{tileNum+1}-{tileNum + 2}";
                else if (secondCombination == null)
                    //secondCombination = $"{tileNum}-{tileNum + 1}-{tileNum + 2}";
                    secondCombination = new Chow(tileInDemand, playerTiles[firstInd], playerTiles[secondInd], firstInd);
                else
                    //thirdCombination = $"{tileNum}-{tileNum + 1}-{tileNum + 2}";
                    thirdCombination = new Chow(tileInDemand, playerTiles[firstInd], playerTiles[secondInd], firstInd);

            }
        }
        if (firstCombination == null && secondCombination == null && thirdCombination == null)
        {
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            Debug.Log("no chow");
            GameManager.instance.numOfAnsweredPlayers++;
            return;
        }
        Debug.Log((firstCombination == null) + "first");
        Debug.Log((secondCombination == null) + "second");
        Debug.Log((thirdCombination == null) + "third");


        if (secondCombination == null)
        {
            GameManager.instance.chowDeclarator = this;
            waitingCombination = firstCombination;
            GameManager.instance.numOfAnsweredPlayers++;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
        }
        else
        {
            Debug.Log("else");
            gameObject.GetComponent<PlayerUI>().MakeChowChoice(firstCombination, secondCombination, thirdCombination);
        }

    }

    public void CheckPung()
    {
        if (isLocalPlayer)
            CmdCheckPung();
    }

    [Command]
    void CmdCheckPung()
    {
        if (!turnForCombination)
        {
            Debug.Log("not your turn");
            return;
        }

        int firstIndex = -1;
        Debug.Log(GameManager.instance.GameTable.lastTile.name+"last tile");
        Debug.Log(FindNumOfSimilarTiles(GameManager.instance.GameTable.lastTile.name, ref firstIndex));

        if (FindNumOfSimilarTiles(GameManager.instance.GameTable.lastTile.name, ref firstIndex) < 2)
        {
            GameManager.instance.numOfAnsweredPlayers++;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            return;
        }
        waitingCombination = new Pung(playerTiles[firstIndex], playerTiles[firstIndex + 1], GameManager.instance.GameTable.lastTile, firstIndex);
        GameManager.instance.pungDeclarator = this;
        GameManager.instance.numOfAnsweredPlayers++;
        gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
    }

    public void CheckKong()
    {
        if (isLocalPlayer)
            CmdCheckKong();
    }

    [Command]
    void CmdCheckKong()
    {

        if (GameManager.instance.winds[GameManager.instance.CurrentWind].player != this)
        {
            if (!turnForCombination)
            {
                Debug.Log("not your turn");
                return;
            }

            int firstIndex = -1;

            if (FindNumOfSimilarTiles(GameManager.instance.GameTable.lastTile.name, ref firstIndex) < 3)
            {
                GameManager.instance.numOfAnsweredPlayers++;
                gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
                return;
            }
            waitingCombination = new Kong(playerTiles[firstIndex], playerTiles[firstIndex + 1], playerTiles[firstIndex + 2], GameManager.instance.GameTable.lastTile, firstIndex);
            GameManager.instance.kongDeclarator = this;
            GameManager.instance.numOfAnsweredPlayers++;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
        }
        else
        {
            foreach (Tile tile in playerTiles)
            {
                int firstIndex = -1;

                if (FindNumOfSimilarTiles(tile.name, ref firstIndex) == 4)
                {
                    DeclareClosedKong(firstIndex);
                    return;

                }
            }

            foreach (Pung pung in openedTiles)
            {
                //int firstIndex = -1;
                if (CheckForTileInArray(pung.tileList[0].name) != -1)
                {
                    DeclareKongFromPung();
                    return;
                }
            }
        }
    }

    public void CheckMahJong()
    {

    }
    #endregion



    #region Combination Declaration

    void LieCombinationTiles(int windPos)
    {
        int firstIndex = -1;
        GameManager.instance.GameTable.MoveLeftStartPosition();

        for (int i = 0; i < waitingCombination.tileList.Count; i++)
        {
            //playerTiles[i].tile.GetComponent<BezierMove>().speed = 30;
            if (waitingCombination.tileList[i] == GameManager.instance.GameTable.lastTile)
                RpcTakeTableTile(GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation);
            else
            {
                int index = playerTiles.FindIndex((x) => x == waitingCombination.tileList[i]);
                Debug.Log(playerTiles[index].name);
                if (firstIndex == -1)
                {
                    freeSpacePosition = playerTiles[index].tile.transform.position;
                    freeSpacePosition.y = 1.2f;
                    freeSpaceIndex = index;
                    firstIndex = index;
                }
                RpcLieOutCombinationTile(GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, waitingCombination.tileList[i].name);
            }
            //RpcLieOutTile(i, GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, "combination");
            GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        }
    }

    [ClientRpc]
    void RpcLieOutCombinationTile(Vector3 freePosition, float rotation, string name)
    {
        int index = playerTiles.FindIndex((x) => x.name == name);

        playerTiles[index].tile.GetComponent<BezierMove>().LieOut(freePosition, rotation);
        EnableToolTipsForTile(index);


        playerTiles.RemoveAt(index);
    }

    public void DeclareClosedKong(int firstIndex)
    {

    }

    public void DeclareKongFromPung()
    {

    }


    public void DeclarePung(int windPos)
    {

        LieCombinationTiles(windPos);

        Pung pung = (Pung)waitingCombination;
        pung.additionalPosition = GameManager.instance.winds[windPos].freeOpenPosition;

        GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);

        GameManager.instance.CurrentWind = windPos;

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;


        Invoke("InvokeDelete", 1.5f);

        RpcAddCombination();

        Invoke("AskForMove", 1.7f);

    }

   

    public void DeclareChow(int windPos)
    {
        LieCombinationTiles(windPos);

        //Debug.Log("comb");
        //foreach (Tile tile in waitingCombination.tileList)
        //{
        //    Debug.Log(tile.name);
        //}

        //int firstIndex = -1;

        //for (int i = 0; i < 3; i++)
        //{
        //    //playerTiles[i].tile.GetComponent<BezierMove>().speed = 30;
        //    if (waitingCombination.tileList[i] == GameManager.instance.GameTable.lastTile)
        //        RpcTakeTableTile(GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation);
        //    else
        //    {
        //        int index = playerTiles.FindIndex((x) => x == waitingCombination.tileList[i]);
        //        Debug.Log(playerTiles[index].name);
        //        if (firstIndex == -1)
        //        {
        //            freeSpacePosition = playerTiles[index].tile.transform.position;
        //            freeSpacePosition.y = 1.2f;
        //            freeSpaceIndex = index;
        //            firstIndex = index;
        //        }
        //        RpcLieOutCombinationTile(GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, waitingCombination.tileList[i].name);
        //    }
        //    //RpcLieOutTile(i, GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, "combination");
        //    GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        //}


        GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        GameManager.instance.CurrentWind = windPos;

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;


        Invoke("InvokeDelete", 1.5f);

        RpcAddCombination();

        Invoke("AskForMove", 1.7f);


    }

    public void DeclareKong(int windPos)
    {
        LieCombinationTiles(windPos);

        waitingCombination.tileList[3].tile.GetComponent<BezierMove>().CloseTile(GameManager.instance.winds[windPos].rotation);

        GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        GameManager.instance.CurrentWind = windPos;

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;


        Invoke("InvokeDelete", 1.5f);

        RpcAddCombination();
        Invoke("AskForFreeTile", 1.8f);

        //Invoke("AskForMove", 0.6f);

    }

    public void DeclareMahJong()
    {

    }

    void AskForFreeTile()
    {
        Wall.instance.GiveFreeTile();
    }

    void AskForMove()
    {
        GameManager.instance.BeginPlayerMove();
    }

    [ClientRpc]
    void RpcAddCombination()
    {
        openedTiles.Add(waitingCombination);
    }

    #endregion

    #region extras

    void InvokeGiveFreeTile()
    {
        Wall.instance.GiveFreeTile();
    }

    void GetNextFlower()
    {
        GameManager.instance.CheckForFlowers();
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

    public void EnableToolTipsForTile(int index)
    {
        ToolTip tip = playerTiles[index].tile.GetComponent<ToolTip>();
        tip.enabled = true;

    }

    [Command]
    void CmdPrepareForCombinations()
    {
        playerTurn = false;
        GameManager.instance.PrepareForCombinations();
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

                if (needFreeTile)
                {
                    if (Wall.instance.freeTiles.Count == 0) return;

                    //Debug.Log(CheckTileMoving(Wall.instance.freeTiles[Wall.instance.freeTiles.Count-1]));

                    Debug.Log(Wall.instance.freeTileIsMoving + "free move");
                    if (Wall.instance.freeTileIsMoving)
                    {
                        Debug.Log("moving");
                        return;

                    }

                    try
                    {
                        if (Wall.instance.freeTiles.Count > 1 && Wall.instance.freeTiles[Wall.instance.freeTiles.Count - 1].tile.transform.position
                            != new Vector3(Wall.instance.tiles
                            [Wall.instance.beginningWall][Wall.instance.beginningPair].upperTile.tile.transform.position.x, 3.4f,
                            Wall.instance.tiles
                            [Wall.instance.beginningWall][Wall.instance.beginningPair].upperTile.tile.transform.position.z))
                        {
                            Debug.Log("Tile is moving!");
                            Debug.Log(Wall.instance.freeTiles.Count);
                            return;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Debug.Log("argument out of range");
                        return;
                    }
                    //Wall.instance.GiveFreeTile();
                    Invoke("InvokeGiveFreeTile", 0.1f);
                    needFreeTile = false;
                    needToCheckMoving = false;
                    return;
                }
                needToCheckMoving = false;
                if (GameMaster.instance.gameState == "starting" || GameMaster.instance.gameState == "distributing")
                {
                    Invoke("Sort", 0.6f);
                    Invoke("GetNextFlower", 0.7f);
                }
                else
                {
                    Debug.Log("not moving");
                    Invoke("GetNextFlower", 0.6f);
                }


            }

        }


        if (isLocalPlayer && playerTurn && !startedCoroutine)
        {
            gameObject.GetComponent<PlayerUI>().LaunchWaitForMove();
            startedCoroutine = true;
        }
    }
    #endregion

}
