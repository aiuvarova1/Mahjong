using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class Player : NetworkBehaviour
{
    public List<Tile> playerTiles = new List<Tile>();

    public List<Combination> openedTiles = new List<Combination>();
    public List<Combination> closedCombinations = new List<Combination>();

    public List<Tile> flowers = new List<Tile>();

    public Camera Camera { get; set; }
    string name = "";
    public string wind;
    public int order;

    public int score;
    public int oldScore = 2000;

    public Tile tileToMove;
    public bool needToCheckMoving = false;
    bool needFreeTile = false;

    public bool playerTurn = false;

    public bool startedCoroutine = false;
    public bool turnForCombination = false;

    public Tile selectedTile;

    Vector3 freeSpacePosition;
    int freeSpaceIndex;

    public Combination waitingCombination;
    public bool isFreeTile = false;

    List<Vector3> positionList;

    #region sorting
    public void Sort()
    {
        if (GameMaster.instance.gameState == "end") return;
        RpcSort();
    }

    [ClientRpc]
    public void RpcSort()
    {
        Debug.Log("rpc");
        try
        {
            SortTiles();
        }
        catch (Exception)
        {
            Debug.Log("ex");
            return;
        }

    }

    [ClientRpc]
    public void RpcFix()
    {
        for (int i = 0; i < playerTiles.Count; i++)
        {
            playerTiles[i].tile.transform.position = new Vector3(playerTiles[i].tile.transform.position.x, 1.4f, playerTiles[i].tile.transform.position.z);
        }
    }

    public void SortTiles()
    {

        Debug.Log(playerTiles[playerTiles.Count - 1].name);

        playerTiles.Sort();

        Debug.Log("sorted");

        //playerTiles[0].tile.transform.position = startPosition;
        for (int i = 0; i < playerTiles.Count; i++)
        {
            playerTiles[i].tile.transform.rotation = Quaternion.Euler(-90, playerTiles[i].tile.transform.eulerAngles.y, 0);
            // Debug.Log(playerTiles[i]);
            playerTiles[i].tile.transform.position = positionList[i];
            // Debug.Log($"{i}, {playerTiles[i].name}");
        }
    }

    [ClientRpc]
    public void RpcFillPositionList(Vector3[] pos)
    {
        positionList = new List<Vector3>(pos);
    }

    #endregion


    #region player turn
    [TargetRpc]
    public void TargetSetCombinationTurn(NetworkConnection conn)
    {
        GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerUI>().LaunchWaitForCombination();
    }

    [Command]
    void CmdSetPlayerTurn(bool turn)
    {
        if (GameMaster.instance.gameState == "end") return;
        playerTurn = turn;
    }
    #endregion

    #region lying tiles

    [ClientRpc]
    void RpcTakeTableTile(Vector3 freePosition, float rotation, bool closed)
    {
        GameManager.instance.GameTable.lastTile.tile.GetComponent<BezierMove>().LieOut(freePosition, rotation, closed);
    }

    [ClientRpc]
    public void RpcLieOutTile(int index, Vector3 freePosition, float rotation, string array)
    {
        Debug.Log(playerTiles.Count);

        playerTiles[index].tile.GetComponent<BezierMove>().LieOut(freePosition, rotation, false);

        tileToMove = playerTiles[index];


        switch (array)
        {
            case "flowers":
                needToCheckMoving = true;
                needFreeTile = true;
                flowers.Add(playerTiles[index]);
                break;
            case "table":

                if (isLocalPlayer)
                {
                    gameObject.GetComponent<PlayerUI>().StopWaitingForMove();

                    playerTurn = false;
                    CmdSetPlayerTurn(false);

                    CmdPrepareForCombinations();
                }

                break;
            default:
                break;

        }
        EnableToolTipsForTile(index);

        playerTiles.RemoveAt(index);

    }

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
            Debug.Log(Wall.instance.freeTiles.Count - 1);
            playerTiles.Add(Wall.instance.freeTiles[Wall.instance.freeTiles.Count - 1]);
            Wall.instance.freeTiles.RemoveAt(Wall.instance.freeTiles.Count - 1);

        }

        if (isLocalPlayer)
            EnableToolTipsForTile(playerTiles.Count - 1);

    }

    [Command]
    public void CmdLieTileOnTable(int index)
    {
        if (GameMaster.instance.gameState == "end") return;

        GameManager.instance.canStopTheGame = false;
        freeSpacePosition = playerTiles[index].tile.transform.position;
        freeSpacePosition.y = 1.4f;
        freeSpaceIndex = index;

        Table t = GameManager.instance.GameTable;

        GameManager.instance.GameTable.lastTile = playerTiles[index];
        RpcSetLastTile(index);

        RpcLieOutTile(index, t.CurrentPosition, t.rotation, "table");
        t.MoveRightStartPosition();

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;

        Invoke("InvokeDelete", 0.5f);
    }

    [ClientRpc]
    void RpcSetLastTile(int index)
    {
        GameManager.instance.GameTable.lastTile = playerTiles[index];
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
        playerTiles[index].tile.transform.position = position;
    }
    #endregion

    #region SelectTile

    public void SelectTile(GameObject tile)
    {
        int index = CheckList(tile);
        if (!playerTurn || !startedCoroutine || index == -1 ) return;

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
            CmdSetPlayerTurn(false);
        }
    }

    [TargetRpc]
    public void TargetSelectLastTile(NetworkConnection conn)
    {

        Invoke("InvokeSelect", 0.2f);
    }

    public void InvokeSelect()
    {
        try
        {
            playerTiles[playerTiles.Count - 1].tile.GetComponent<BezierMove>().SelectTile();
            selectedTile = playerTiles[playerTiles.Count - 1];
        }
        catch (ArgumentOutOfRangeException)
        {
            return;
        }
    }
    #endregion

    #region checking
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
    #endregion

    #region checkCombinations

    public void Pass()
    {
        if (!isLocalPlayer) return;
        CmdPass();
    }

    [Command]
    public void CmdPass()
    {

        if (!turnForCombination)
        {
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "It's not your turn");
            return;
        }
        turnForCombination = false;
        GameManager.instance.NumOfAnsweredPlayers++;
        gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
    }

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
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "It's not your turn");
            return;
        }

        int nextWind = GameManager.instance.CurrentWind + 1;

        if (nextWind > 3)
            nextWind = 0;

        if (GameManager.instance.winds[nextWind].player != this)
        {
            Debug.Log("not next player");
            GameManager.instance.NumOfAnsweredPlayers++;
            turnForCombination = false;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "You are not the next player");
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
            GameManager.instance.NumOfAnsweredPlayers++;
            Debug.Log("not suit");
            turnForCombination = false;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "Chow can not be declared of winds or dragons");
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
                firstCombination = new Chow(playerTiles[firstInd], playerTiles[secondInd], tileInDemand);
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
                    firstCombination = new Chow(playerTiles[firstInd], tileInDemand, playerTiles[secondInd]);
                //firstCombination = $"{tileNum - 1}-{tileNum}-{tileNum+1}";

                else
                    secondCombination = new Chow(playerTiles[firstInd], tileInDemand, playerTiles[secondInd]);

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
                    firstCombination = new Chow(tileInDemand, playerTiles[firstInd], playerTiles[secondInd]);
                //firstCombination = $"{tileNum}-{tileNum+1}-{tileNum + 2}";
                else if (secondCombination == null)
                    //secondCombination = $"{tileNum}-{tileNum + 1}-{tileNum + 2}";
                    secondCombination = new Chow(tileInDemand, playerTiles[firstInd], playerTiles[secondInd]);
                else
                    //thirdCombination = $"{tileNum}-{tileNum + 1}-{tileNum + 2}";
                    thirdCombination = new Chow(tileInDemand, playerTiles[firstInd], playerTiles[secondInd]);

            }
        }
        if (firstCombination == null && secondCombination == null && thirdCombination == null)
        {
            turnForCombination = false;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            Debug.Log("no chow");
            GameManager.instance.NumOfAnsweredPlayers++;
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "No tiles for chow");
            return;
        }
        Debug.Log((firstCombination == null) + "first");
        Debug.Log((secondCombination == null) + "second");
        Debug.Log((thirdCombination == null) + "third");


        if (secondCombination == null)
        {
            GameManager.instance.chowDeclarator = this;
            waitingCombination = firstCombination;
            GameManager.instance.NumOfAnsweredPlayers++;
            turnForCombination = false;
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
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "It's not your turn");
            return;
        }

        int firstIndex = -1;
        Debug.Log(GameManager.instance.GameTable.lastTile.name + "last tile");
        Debug.Log(FindNumOfSimilarTiles(GameManager.instance.GameTable.lastTile.name, ref firstIndex));

        if (FindNumOfSimilarTiles(GameManager.instance.GameTable.lastTile.name, ref firstIndex) < 2)
        {
            GameManager.instance.NumOfAnsweredPlayers++;
            turnForCombination = false;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "No tiles for Pung");
            return;
        }
        waitingCombination = new Pung(playerTiles[firstIndex], playerTiles[firstIndex + 1], GameManager.instance.GameTable.lastTile);
        GameManager.instance.pungDeclarator = this;
        GameManager.instance.NumOfAnsweredPlayers++;
        turnForCombination = false;
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
                gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "It's not your turn");
                return;
            }

            int firstIndex = -1;

            if (FindNumOfSimilarTiles(GameManager.instance.GameTable.lastTile.name, ref firstIndex) < 3)
            {
                GameManager.instance.NumOfAnsweredPlayers++;
                turnForCombination = false;
                gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);
                gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "No tiles for Kong");
                return;
            }
            waitingCombination = new Kong(playerTiles[firstIndex], playerTiles[firstIndex + 1], playerTiles[firstIndex + 2], GameManager.instance.GameTable.lastTile, true);
            GameManager.instance.kongDeclarator = this;
            GameManager.instance.NumOfAnsweredPlayers++;
            turnForCombination = false;
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

            foreach (Combination comb in openedTiles)
            {
                Pung pung;
                if (comb is Pung) pung = (Pung)comb;
                else
                {
                    Debug.Log("not pung");
                    continue;
                }
                Debug.Log(pung.tileList[0].name);

                int tileIndex = CheckForTileInArray(pung.tileList[0].name);
                Debug.Log(tileIndex);

                if (tileIndex != -1)
                {
                    Debug.Log("kong from pung");
                    DeclareKongFromPung(pung, tileIndex);
                    return;
                }
            }

            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "No tiles for Kong");

        }
    }

    public void CheckMahJong()
    {
        if (isLocalPlayer)
            CmdCheckMahJong();
    }

    [Command]
    void CmdCheckMahJong()
    {
        if (!playerTurn && !turnForCombination)
        {
            Debug.Log("not your turn");
            gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "It's not your turn");
            return;
        }

        List<Tile> closedTiles = new List<Tile>();

        for (int i = 0; i < playerTiles.Count; i++)
        {
            closedTiles.Add(playerTiles[i]);
        }

        if (turnForCombination)
        {
            closedTiles.Add(GameManager.instance.GameTable.lastTile);
        }
        closedTiles.Sort();
        Debug.Log("closed tiles" + closedTiles.Count);

        //0-bamboos,1-dots,2-symbols,3-winds,4-dragons
        List<List<Tile>> suitSets = new List<List<Tile>>();
        List<List<Combination>> closedCombinations = new List<List<Combination>>();

        for (int i = 0; i < 5; i++)
        {
            suitSets.Add(new List<Tile>());
            closedCombinations.Add(new List<Combination>());
        }

        int pairSetNum = -1;
        List<Tile> setWithPair = new List<Tile>();

        FillSuitSets(ref suitSets, closedTiles);

        for (int i = 0; i < suitSets.Count; i++)
        {
            List<Tile> set = suitSets[i];

            if (set.Count % 3 == 2 && setWithPair.Count == 0)
            {
                setWithPair = set;
                pairSetNum = i;

            }
            else if (set.Count % 3 != 0)
            {
                Debug.Log("wrong %3");
                AbortMahJong();
                return;
            }
        }
        Debug.Log("set with pair" + setWithPair.Count);

        //set is sorted
        //try to find pair set combinations
        for (int i = 0; i < setWithPair.Count - 1; i++)
        {
            if (setWithPair[i].name == setWithPair[i + 1].name)
            {
                Debug.Log(setWithPair[i].name + "equal");
                List<Combination> combinations = new List<Combination>();

                List<Tile> setWithoutPair = new List<Tile>(suitSets[pairSetNum]);

                setWithoutPair.RemoveAt(i);
                setWithoutPair.RemoveAt(i);

                if (setWithoutPair.Count == 0 || FillOneSuitCombinations(setWithoutPair, ref combinations))
                {
                    Debug.Log("fill true" + combinations.Count);
                    closedCombinations[pairSetNum] = combinations;
                    Debug.Log("fill true" + closedCombinations[pairSetNum].Count);
                    closedCombinations[pairSetNum].Add(new Pair(setWithPair[i], setWithPair[i + 1]));
                    break;
                }
            }
            if (i == setWithPair.Count - 2)
            {
                Debug.Log("no fillonesuit variants in pair");
                AbortMahJong();
                return;
            }
        }

        //other suits
        for (int i = 0; i < suitSets.Count; i++)
        {
            Debug.Log(i + " " + suitSets.Count + " " + pairSetNum);
            if (i != pairSetNum)
            {
                List<Combination> combinations = new List<Combination>();

                if (suitSets[i].Count == 0 || FillOneSuitCombinations(suitSets[i], ref combinations))
                {
                    Debug.Log(combinations.Count);
                    closedCombinations[i] = combinations;
                    Debug.Log(closedCombinations[i].Count);

                }
                else
                {
                    Debug.Log("no fillonesuit variants");
                    AbortMahJong();
                    return;
                }
            }
        }

        waitingCombination = new MahJong(closedCombinations);

        GameManager.instance.mahJongDeclarator = this;
        GameManager.instance.NumOfAnsweredPlayers++;
        turnForCombination = false;
        gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);

        if (playerTurn)
        {
            gameObject.GetComponent<PlayerUI>().StopWaitingForMove();
            DeclareMahJong(GameManager.instance.CurrentWind);
        }
        //here create new waiting comb mahjong
        //open table tile combination
        //stop game and calculate points

    }

    bool FillOneSuitCombinations(List<Tile> suitSet, ref List<Combination> combinationList)
    {
        Debug.Log("fill" + suitSet.Count);
        for (int i = 0; i < suitSet.Count - 2; i++)
        {
            Debug.Log(suitSet[i].name);
            for (int j = i + 1; j < suitSet.Count - 1; j++)
            {
                Debug.Log(suitSet[j].name);
                for (int k = j + 1; k < suitSet.Count; k++)
                {
                    Debug.Log(suitSet[k].name);
                    List<Tile> newList = new List<Tile>(suitSet);
                    if (suitSet[i].name == suitSet[j].name && suitSet[j].name == suitSet[k].name)
                    {
                        Debug.Log("pung");
                        for (int m = 0; m < 3; m++)
                        {
                            //!!!!!!!!!!!!!!!!!!!
                            newList.RemoveAt(i);
                        }
                        if (newList.Count == 0)
                        {
                            combinationList.Add(new Pung(suitSet[i], suitSet[j], suitSet[k]));
                            return true;
                        }
                        else
                        {
                            if (FillOneSuitCombinations(newList, ref combinationList))
                            {
                                combinationList.Add(new Pung(suitSet[i], suitSet[j], suitSet[k]));
                                return true;
                            }
                        }
                    }
                    if (!char.IsDigit(suitSet[i].name[1])) continue;

                    Debug.Log($"{char.IsLower(suitSet[i].name[0])},i {int.Parse(suitSet[i].name[1].ToString())}, j {int.Parse(suitSet[j].name[1].ToString())},k {int.Parse(suitSet[k].name[1].ToString())}");
                    if (char.IsLower(suitSet[i].name[0])
                      && int.Parse(suitSet[i].name[1].ToString()) + 1 == int.Parse(suitSet[j].name[1].ToString())
                      && int.Parse(suitSet[j].name[1].ToString()) + 1 == int.Parse(suitSet[k].name[1].ToString()))
                    {

                        //newList.RemoveAt(i);
                        newList.RemoveAt(newList.FindIndex(x => x.name == suitSet[i].name));
                        newList.RemoveAt(newList.FindIndex(x => x.name == suitSet[j].name));
                        newList.RemoveAt(newList.FindIndex(x => x.name == suitSet[k].name));

                        if (newList.Count == 0)
                        {
                            combinationList.Add(new Chow(suitSet[i], suitSet[j], suitSet[k]));
                            return true;
                        }
                        else
                        {
                            if (FillOneSuitCombinations(newList, ref combinationList))
                            {
                                combinationList.Add(new Chow(suitSet[i], suitSet[j], suitSet[k]));
                                return true;
                            }
                        }
                    }
                }
            }
        }


        return false;
    }

    void FillSuitSets(ref List<List<Tile>> suitSets, List<Tile> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            switch (tiles[i].name[0])
            {
                case 'b':
                    suitSets[0].Add(tiles[i]);
                    break;
                case 'd':
                    suitSets[1].Add(tiles[i]);
                    break;
                case 's':
                    suitSets[2].Add(tiles[i]);
                    break;
                case 'W':
                    if (tiles[i].name[1] == 'e')
                        suitSets[3].Add(tiles[i]);
                    else
                        suitSets[4].Add(tiles[i]);
                    break;
                case 'E':
                case 'S':
                case 'N':
                    suitSets[3].Add(tiles[i]);
                    break;
                case 'R':
                case 'G':
                    suitSets[4].Add(tiles[i]);
                    break;
                default:
                    Debug.Log("wrong tile name");
                    break;


            }
        }
    }

    void AbortMahJong()
    {
        if (turnForCombination)
        {
            GameManager.instance.NumOfAnsweredPlayers++;
            turnForCombination = false;
            gameObject.GetComponent<PlayerUI>().TargetStopWaitingForCombination(connectionToClient);

        }
        gameObject.GetComponent<PlayerUI>().TargetShowInfo(connectionToClient, "No MahJong");
    }

    #endregion

    #region Combination Declaration

    void LieCombinationTiles(int windPos)
    {
        int firstIndex = -1;
        GameManager.instance.GameTable.MoveLeftStartPosition();

        for (int i = 0; i < waitingCombination.tileList.Count; i++)
        {
            Debug.Log(GameManager.instance.GameTable.lastTile.name);
            //playerTiles[i].tile.GetComponent<BezierMove>().speed = 30;
            if (waitingCombination.tileList[i] == GameManager.instance.GameTable.lastTile)
            {
                bool closed = (i == 3) ? true : false;
                RpcTakeTableTile(GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, closed);
            }
            else
            {
                int index = playerTiles.FindIndex((x) => x == waitingCombination.tileList[i]);
                Debug.Log(index + "index");
                Debug.Log(playerTiles[index].name);
                if (firstIndex == -1)
                {
                    freeSpacePosition = playerTiles[index].tile.transform.position;
                    freeSpacePosition.y = 1.2f;
                    freeSpaceIndex = index;
                    firstIndex = index;
                }
                Debug.Log(i);
                if (i != 3)
                    RpcLieOutCombinationTile(GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, waitingCombination.tileList[i].name, false);
                else
                    RpcLieOutCombinationTile(GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, waitingCombination.tileList[i].name, true);
            }
            //RpcLieOutTile(i, GameManager.instance.winds[windPos].freeOpenPosition, GameManager.instance.winds[windPos].rotation, "combination");
            GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        }
    }

    [ClientRpc]
    void RpcLieOutCombinationTile(Vector3 freePosition, float rotation, string name, bool closed)
    {
        int index = playerTiles.FindIndex((x) => x.name == name);

        playerTiles[index].tile.GetComponent<BezierMove>().LieOut(freePosition, rotation, closed);
        EnableToolTipsForTile(index);

        playerTiles.RemoveAt(index);
    }

    public void DeclareClosedKong(int firstIndex)
    {
        playerTurn = false;
        GameManager.instance.TargetSetTurn(connectionToClient, false);
        gameObject.GetComponent<PlayerUI>().TargetStopWaitingForMove(connectionToClient);

        int windPos = GameManager.instance.CurrentWind;
        openedTiles.Add(new Kong(playerTiles[firstIndex], playerTiles[firstIndex + 1], playerTiles[firstIndex + 2], playerTiles[firstIndex + 3], false));

        freeSpacePosition = playerTiles[firstIndex].tile.transform.position;
        freeSpacePosition.y = 1.2f;
        freeSpaceIndex = firstIndex;

        string name = playerTiles[firstIndex].name;

        for (int i = firstIndex; i < firstIndex + 4; i++)
        {
            if (i == firstIndex || i == firstIndex + 3)
                RpcLieOutCombinationTile(GameManager.instance.winds[GameManager.instance.CurrentWind].freeOpenPosition, GameManager.instance.winds[GameManager.instance.CurrentWind].rotation, name, true);
            else
                RpcLieOutCombinationTile(GameManager.instance.winds[GameManager.instance.CurrentWind].freeOpenPosition, GameManager.instance.winds[GameManager.instance.CurrentWind].rotation, name, false);

            GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);

        }
        GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        GameManager.instance.DeclareCombination("closed Kong", 2);
        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;

        Invoke("InvokeDelete", 1.5f);

        Invoke("AskForFreeTile", 1.7f);
    }

    public void DeclareKongFromPung(Pung pung, int index)
    {
        playerTurn = false;
        GameManager.instance.TargetSetTurn(connectionToClient, false);
        gameObject.GetComponent<PlayerUI>().TargetStopWaitingForMove(connectionToClient);

        //!!!! only on server
        Kong kong = new Kong(pung, playerTiles[index], true);
        openedTiles.Remove(pung);
        openedTiles.Add(kong);

        freeSpacePosition = playerTiles[index].tile.transform.position;
        freeSpacePosition.y = 1.2f;
        freeSpaceIndex = index;

        RpcLieOutCombinationTile(pung.additionalPosition, GameManager.instance.winds[GameManager.instance.CurrentWind].rotation, playerTiles[index].name, true);

        GameManager.instance.DeclareCombination("Kong", 2);

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;
        Invoke("InvokeDelete", 1.2f);

        Invoke("AskForFreeTile", 1.5f);

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
        GameManager.instance.DeclareCombination("Pung", 1);

        Invoke("InvokeDelete", 1.8f);

        openedTiles.Add(waitingCombination);

        Invoke("AskForMove", 2f);

    }


    public void DeclareChow(int windPos)
    {
        LieCombinationTiles(windPos);
        GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        GameManager.instance.CurrentWind = windPos;

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;

        GameManager.instance.DeclareCombination("Chow", 0);

        Invoke("InvokeDelete", 1.8f);

        openedTiles.Add(waitingCombination);

        Invoke("AskForMove", 2f);
    }


    public void DeclareKong(int windPos)
    {
        LieCombinationTiles(windPos);

        GameManager.instance.winds[windPos].MoveRightFreePosition(ref GameManager.instance.winds[windPos].freeOpenPosition);
        GameManager.instance.CurrentWind = windPos;

        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;

        GameManager.instance.DeclareCombination("Kong", 2);

        Invoke("InvokeDelete", 1.8f);

        openedTiles.Add(waitingCombination);
        Invoke("AskForFreeTile", 2f);
    }

    public void DeclareMahJong(int windPos)
    {
        if (playerTurn)
        {
            Debug.Log("stop");
           // gameObject.GetComponent<PlayerUI>().StopWaitingForMove();
            GameMaster.instance.readyToContinuePlayers = 0;
            GameMaster.instance.gameState = "end";

        }
        MahJong mahjong = (MahJong)waitingCombination;

        Debug.Log("Declare");

        Combination combToOpen = null;
        int combinationNum = -1;

        Debug.Log(mahjong.closedCombinations.Count);

        Debug.Log(GameManager.instance.GameTable.lastTile.name);

        for (int k = 0; k < mahjong.closedCombinations.Count; k++)
        {
            for (int i = 0; i < mahjong.closedCombinations[k].Count; i++)
            {
                Combination comb = mahjong.closedCombinations[k][i];

                for (int j = 0; j < comb.tileList.Count; j++)
                {
                    if (comb.tileList[j] == GameManager.instance.GameTable.lastTile)
                    {
                        combinationNum = i;
                        Debug.Log("found" + i);


                        combToOpen = comb;

                        Debug.Log(mahjong.closedCombinations[k].Count);

                        mahjong.closedCombinations[k].Remove(combToOpen);

                        Debug.Log(mahjong.closedCombinations[k].Count);

                        waitingCombination = combToOpen;

                        LieCombinationTiles(windPos);
                        GameManager.instance.CurrentWind = windPos;

                        GameManager.instance.winds[GameManager.instance.CurrentWind].freePosition = freeSpacePosition;

                        Invoke("InvokeDelete", 1.8f);

                        openedTiles.Add(waitingCombination);

                        break;
                    }
                }
            }
        }

        GameManager.instance.DeclareCombination("Mahjong", 3);
        mahjong.openedCombinations = openedTiles;
        mahjong.flowers = flowers;

        GameManager.instance.RefreshWinds();

        GameManager.instance.FinishGame(mahjong);

    }

    void AskForFreeTile()
    {
        Wall.instance.GiveFreeTile();
    }

    void AskForMove()
    {
        GameManager.instance.BeginPlayerMove();
    }

    #endregion

    #region extras

    void EnableCheck()
    {
        needToCheckMoving = true;
    }

    [ClientRpc]
    public void RpcOpenTiles(float rotation)
    {
        Debug.Log("rpcopen");
        for (int i = 0; i < playerTiles.Count; i++)
        {
            Debug.Log(i);
            playerTiles[i].tile.GetComponent<BezierMove>().ShowTile(rotation);
        }
    }

    void GetNextFlower()
    {
        Debug.Log("get next");
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

                break;
            case "South":
                GameManager.instance.winds[1].player = this;
                break;
            case "West":
                GameManager.instance.winds[2].player = this;
                break;
            case "North":
                GameManager.instance.winds[3].player = this;
                break;
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

        if (isServer && needToCheckMoving)
        {

            if (!CheckTileMoving(tileToMove))
            {
                if (needFreeTile)
                {
                    if (Wall.instance.freeTiles.Count == 0)
                    {
                        Debug.Log("count 0");
                        return;
                    }

                    if (Wall.instance.freeTileIsMoving || Wall.instance.freeTiles.Count - 1 < 0)
                    {
                        Debug.Log(Wall.instance.freeTileIsMoving);
                        return;
                    }

                    if (Wall.instance.freeTileIsMoving)
                    {
                        Debug.Log("free move");
                        return;
                    }

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

                    Invoke("AskForFreeTile", 0.1f);
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
                    //  Invoke("Sort", 0.5f);
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


    #region finish game

    [ClientRpc]
    public void RpcRefresh()
    {

        Debug.Log("rpcrefresh");

        needToCheckMoving = false;
        needFreeTile = false;

        playerTurn = false;

        startedCoroutine = false;
        turnForCombination = false;

        playerTiles = new List<Tile>();

        openedTiles = new List<Combination>();
        closedCombinations = new List<Combination>();

        flowers = new List<Tile>();
        isFreeTile = false;
    }

    [TargetRpc]
    public void TargetRefresh(NetworkConnection conn)
    {
        Debug.Log("target refresh");
        GetComponent<PlayerUI>().Refresh();
        GameManager.instance.OnRefresh();

    }

    //public void testRefr()
    //{
    //    CmdRefr();
    //}

    //[Command]
    //public void CmdRefr()
    //{
    //    GameManager.instance.stopTheGame = true;
    //}
#endregion
}
