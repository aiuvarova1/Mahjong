using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




public class PlayerUI : NetworkBehaviour
{
    static NewNetworkManager networkManager;
    Player player;

    [SerializeField]
    Text infoText;

    [SerializeField]
    GameObject infoPanel;

    [SerializeField]
    GameObject startButton;

    public Text playerWind;

    public GameObject QuitButton;

    public Text countDown;
    public Text declaration;
    public Text combinationInfo;


    [SerializeField]
    public Canvas canvas;

    IEnumerator starter;
    public IEnumerator CountDown;
    public IEnumerator CombinationEnum;
    IEnumerator hider;

    public bool leave = false;
    public bool ready = false;

    public GameObject blackScreen;

    public Text toolTipText;

    public GameObject chowPanel;
    public GameObject scorePanel;
    public GameObject leavePanel;

    public Button thirdButton;
    public Button firstButton;
    public Button secondButton;

    Combination firstChow;
    Combination secondChow;
    Combination thirdChow;

    public Camera mainCam;

    #region start and finish

    //initialization
    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        networkManager = (NewNetworkManager)NetworkManager.singleton;
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    //initialization
    private void Awake()
    {
        starter = WaitForStart();
        CountDown = WaitForMove();
        CombinationEnum = WaitForCombination();
        hider = HideInfo();

        infoPanel.SetActive(false);
        countDown.enabled = false;

        thirdButton.gameObject.SetActive(false);
        chowPanel.SetActive(false);
        declaration.enabled = false;
        //combinationInfo.enabled = false;

        leavePanel.SetActive(false);
        scorePanel.SetActive(false);
    }

    //refreshes ui data
    public void Refresh()
    {
        StopAllCoroutines();

        starter = WaitForStart();
        CountDown = WaitForMove();
        CombinationEnum = WaitForCombination();
        hider = HideInfo();

        leave = false;
        ready = false;

        infoPanel.SetActive(false);
        countDown.enabled = false;

        thirdButton.gameObject.SetActive(false);
        chowPanel.SetActive(false);
        declaration.enabled = false;
        combinationInfo.enabled = false;

        QuitButton.SetActive(true);
        startButton.SetActive(true);
    }
    #endregion

    #region info and scores

    //builds score table
    [TargetRpc]
    public void TargetShowScores(NetworkConnection conn, int[] scores, int[] oldScores, string[] names, string winner)
    {
        scorePanel.SetActive(true);
        QuitButton.SetActive(true);

        GameObject tableNames = GameObject.FindGameObjectWithTag("Names");
        Button[] objects = tableNames.GetComponentsInChildren<Button>();

        for (int i = 1; i < names.Length + 1; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = names[i - 1];
        }

        tableNames = GameObject.FindGameObjectWithTag("OldScore");
        objects = tableNames.GetComponentsInChildren<Button>();

        for (int i = 1; i < names.Length + 1; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = (oldScores[i - 1]).ToString();
        }

        tableNames = GameObject.FindGameObjectWithTag("Points");
        objects = tableNames.GetComponentsInChildren<Button>();

        for (int i = 1; i < names.Length + 1; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = (scores[i - 1]).ToString();
        }

        tableNames = GameObject.FindGameObjectWithTag("Points0");
        Button[] points0 = tableNames.GetComponentsInChildren<Button>();

        tableNames = GameObject.FindGameObjectWithTag("Points1");
        Button[] points1 = tableNames.GetComponentsInChildren<Button>();

        tableNames = GameObject.FindGameObjectWithTag("Points2");
        Button[] points2 = tableNames.GetComponentsInChildren<Button>();

        tableNames = GameObject.FindGameObjectWithTag("Points3");
        Button[] points3 = tableNames.GetComponentsInChildren<Button>();

        List<Button[]> buttons = new List<Button[]>() { points0, points1, points2, points3 };

        int[,] matrix = new int[4, 4];

        FillMatrix(ref matrix, scores, winner);


        //tableNames = GameObject.FindGameObjectWithTag("Combinations");
        //objects = tableNames.GetComponentsInChildren<Button>();
        //set column names
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i][0].GetComponentInChildren<Text>().text = (names[i]).ToString();
            //objects[i + 1].GetComponentInChildren<Text>().text = (names[i]).ToString();

            for (int j = 1; j < buttons[i].Length; j++)
            {
                buttons[i][j].GetComponentInChildren<Text>().text = matrix[i, j - 1].ToString();
            }
        }

        tableNames = GameObject.FindGameObjectWithTag("Total");
        objects = tableNames.GetComponentsInChildren<Button>();

        List<int> total = new List<int>() { 0, 0, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                total[i] += matrix[j, i];
            }
        }

        for (int i = 1; i < names.Length + 1; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = (total[i - 1]).ToString();
        }

        tableNames = GameObject.FindGameObjectWithTag("NewScore");
        objects = tableNames.GetComponentsInChildren<Button>();

        for (int i = 1; i < names.Length + 1; i++)
        {

            objects[i].GetComponentInChildren<Text>().text = (oldScores[i - 1] + total[i - 1]).ToString();
        }
        player.oldScore = oldScores[player.order] + total[player.order];
        CmdSetScore(player.oldScore);

    }

    //sets new total score on server
    [Command]
    void CmdSetScore(int score)
    {
        player.oldScore = score;
    }

    //fills table matrix of points
    void FillMatrix(ref int[,] matrix, int[] scores, string winner)
    {
        int winNum = -1;

        switch (winner)
        {
            case "East":
                winNum = 0;
                break;
            case "South":
                winNum = 1;
                break;
            case "West":
                winNum = 2;
                break;
            case "North":
                winNum = 3;
                break;
            default:
                Debug.Log("invalid winner wind");
                return;

        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                //winner's column
                if (i == winNum)
                {
                    //if east wins
                    if (winNum == 0)
                    {
                        if (j != i)
                            matrix[j, i] = scores[i] * 2;
                        else
                            matrix[j, i] = 0;
                    }
                    else
                    {
                        if (i != j)
                        {
                            matrix[j, i] = scores[i];
                            if (j == 0) matrix[j, i] *= 2;
                        }
                        else
                            matrix[j, i] = 0;
                    }
                }
                //other players
                else
                {
                    //if east wins
                    if (winNum == 0)
                    {
                        if (j != i)
                        {
                            //east row
                            if (j == 0)
                                matrix[j, i] = -scores[0] * 2;
                            else
                                matrix[j, i] = scores[i] - scores[j];
                        }

                        else
                            matrix[j, i] = 0;
                    }
                    else
                    {
                        if (i != j)
                        {
                            //east column
                            if (i == 0)
                            {
                                //winner row
                                if (j == winNum)
                                    matrix[j, i] = -scores[j] * 2;
                                else
                                    matrix[j, i] = (scores[i] - scores[j]) * 2;

                            }
                            else
                            {
                                if (j == winNum)
                                    matrix[j, i] = -scores[j];
                                else if (j == 0)
                                    matrix[j, i] = (scores[i] - scores[j]) * 2;
                                else
                                    matrix[j, i] = (scores[i] - scores[j]);
                            }
                        }
                        else
                            matrix[j, i] = 0;
                    }
                }
            }

        }
    }

    //chacges combination info for client
    [TargetRpc]
    public void TargetInfo(NetworkConnection conn, string info)
    {
        ChangeInfo(info);
    }

    //chacges combination info for client
    public void ChangeInfo(string info)
    {
        StopCoroutine(hider);
        StopHideInfo();
        combinationInfo.enabled = true;
        combinationInfo.text = LocalizationManager.instance.GetLocalizedValue(info);
    }

    //invokes cmdcontinue
    public void AskToContinueGame()
    {
        CmdContinue();
    }

    //tells server that player wants to continue
    [Command]
    void CmdContinue()
    {
        GameMaster.instance.readyToContinuePlayers++;

        GetComponent<Player>().RpcRefresh();
        GetComponent<Player>().TargetRefresh(connectionToClient);
    }

    //changes camera at the beginning of the new round
    public void ChangeCamera()
    {

        if (player.Camera.enabled)
        {
            player.Camera.enabled = false;
            mainCam.enabled = true;
        }
        else
        {
            player.Camera.enabled = true;
            mainCam.enabled = false;
        }
    }

    //shows info about disconnected player
    [TargetRpc]
    public void TargetShowLeftPlayerInfo(NetworkConnection conn, string name, string wind)
    {
        leavePanel.SetActive(true);
        leavePanel.GetComponentInChildren<Text>().text = $"{name} ({LocalizationManager.instance.GetLocalizedValue(wind)}) " +
            $"{LocalizationManager.instance.GetLocalizedValue("has left the room")}";
    }
    #endregion

    #region Coroutines

    //shows combination info
    [TargetRpc]
    public void TargetShowInfo(NetworkConnection conn, string info)
    {
        if (LocalizationManager.instance == null || LocalizationManager.instance.language == "Eng")
            combinationInfo.text = info;
        else
            combinationInfo.text = LocalizationManager.instance.GetLocalizedValue(info);

        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref combinationInfo);
        StopCoroutine(hider);
        StopHideInfo();
        StartCoroutine(hider);
    }

    //hides combination info
    IEnumerator HideInfo()
    {
        combinationInfo.enabled = true;

        Color fullCol = combinationInfo.color;
        fullCol.a = 1f;
        combinationInfo.color = fullCol;

        yield return new WaitForSeconds(2f);

        while (combinationInfo.color.a > 0)
        {
            Color col = combinationInfo.color;
            col.a -= 0.1f;
            combinationInfo.color = col;
            yield return new WaitForSeconds(0.3f);
        }
        combinationInfo.enabled = false;
        StopHideInfo();
    }

    //refreshes hider coroutine
    void StopHideInfo()
    {
        Color fullCol = combinationInfo.color;
        fullCol.a = 1f;
        combinationInfo.color = fullCol;
        hider = HideInfo();
    }

    //shows and hides info about combination declaration
    IEnumerator DeclareCombination(string wind, string combination)
    {
        if (combination == "" && wind == "")
        {
            if (LocalizationManager.instance != null)
                declaration.text = LocalizationManager.instance.GetLocalizedValue("Draw!");
        }
        //!!!
        else
        {
            if (LocalizationManager.instance == null || LocalizationManager.instance.language == "Eng")
                declaration.text = $"{wind} declares {combination}!";
            else
                declaration.text = $"{LocalizationManager.instance.GetLocalizedValue(wind)} объявляет {LocalizationManager.instance.GetLocalizedValue(combination)}!";
        }
        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref declaration);

        Color fullCol = declaration.color;
        fullCol.a = 1f;
        declaration.color = fullCol;

        declaration.enabled = true;
        yield return new WaitForSeconds(2f);

        while (declaration.color.a > 0)
        {
            Color col = declaration.color;
            col.a -= 0.1f;
            declaration.color = col;
            yield return new WaitForSeconds(0.3f);
        }
        declaration.enabled = false;
        fullCol = declaration.color;
        fullCol.a = 1f;
        declaration.color = fullCol;
    }

    //plays combination audio and shows info
    [TargetRpc]
    public void TargetShowDeclaredCombination(NetworkConnection conn, string wind, string combination, bool needDeclare, int audioNum)
    {
        if (needDeclare)
            StartCoroutine(DeclareCombination(wind, combination));

        if (audioNum != -1 && AudioManager.instance != null)
        {
            GetComponent<AudioSource>().clip = AudioManager.instance.combSounds[audioNum];
            GetComponent<AudioSource>().Play();
        }
    }

    //waiting for start of the round
    IEnumerator WaitForStart()
    {
        int countdown = 30;

        string prefix;

        if (LocalizationManager.instance == null || LocalizationManager.instance.language == "Eng")
            prefix = "Ready to start? ";
        else
            prefix = "Готовы начать?";

        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref infoText);

        infoPanel.SetActive(true);

        while (countdown > 0)
        {
            if (ready)
            {
                if (LocalizationManager.instance != null && LocalizationManager.instance.language == "Eng")
                    prefix = "Waiting for other \n   players...";
                else
                    prefix = "Ожидаем других игроков...";
                startButton.SetActive(false);
            }
            infoText.text = prefix + $"({countdown})";
            yield return new WaitForSeconds(1);

            countdown--;
        }
        CmdFailToStart();
        StopWaiting();
    }

    //stops coroutine for the start
    [TargetRpc]
    public void TargetStopWaiting(NetworkConnection conn)
    {
        StopWaiting();
    }

    //refreshes start coroutine
    public void StopWaiting()
    {
        StopCoroutine(starter);
        starter = WaitForStart();
        startButton.SetActive(true);

        infoText.text = " ";
        infoPanel.SetActive(false);
    }

    //launches waiting for the start
    [TargetRpc]
    public void TargetStartWaiting(NetworkConnection conn)
    {
        ready = false;
        StopWaiting();
        StartCoroutine(starter);
    }

    //countdown for making a move
    IEnumerator WaitForMove()
    {
        int countdown = 30;

        countDown.text = "30";
        combinationInfo.enabled = true;

        combinationInfo.text = LocalizationManager.instance.GetLocalizedValue("Your move");
        countDown.enabled = true;

        while (countdown > 0)
        {
            countDown.text = $"{countdown}";
            yield return new WaitForSeconds(1);
            countdown--;
        }

        if (player.selectedTile != null)
            player.SelectTile(player.selectedTile.tile);
        player.startedCoroutine = false;
        StopWaitingForMove();
    }

    //launches coroutine
    public void LaunchWaitForMove()
    {
        if (isClient)
        {
            StartCoroutine(CountDown);
        }
    }

    //stops moving coroutine
    [TargetRpc]
    public void TargetStopWaitingForMove(NetworkConnection conn)
    {
        StopWaitingForMove();
    }

    //refreshes move coroutine
    public void StopWaitingForMove()
    {
        StopCoroutine(CountDown);
        CountDown = WaitForMove();
        countDown.enabled = false;
        player.startedCoroutine = false;
        combinationInfo.enabled = false;
    }

    //countdown to declare combination
    IEnumerator WaitForCombination()
    {
        CmdSetCombinationTurn(true);
        int countdown = 20;

        combinationInfo.enabled = true;
        combinationInfo.text = LocalizationManager.instance.GetLocalizedValue("Your turn");

        countDown.text = "20";
        countDown.enabled = true;

        while (countdown > 0)
        {
            countDown.text = $"{countdown}";
            yield return new WaitForSeconds(1);
            countdown--;
        }

        if (chowPanel.activeSelf)
            chowPanel.SetActive(false);
        if (thirdButton.enabled)
            thirdButton.enabled = false;

        CmdSetCombinationTurn(false);
        StopWaitingForCombination();
    }

    //launches combination countdown
    public void LaunchWaitForCombination()
    {
        StartCoroutine(CombinationEnum);
    }

    //stops combination coroutine
    [TargetRpc]
    public void TargetStopWaitingForCombination(NetworkConnection conn)
    {
        StopWaitingForCombination();
    }

    //refreshes combination coroutine
    public void StopWaitingForCombination()
    {
        StopCoroutine(CombinationEnum);
        CombinationEnum = WaitForCombination();
        countDown.enabled = false;
        player.turnForCombination = false;
        combinationInfo.enabled = false;

        if (chowPanel.activeSelf)
            chowPanel.SetActive(false);
        if (thirdButton.enabled)
            thirdButton.enabled = false;
    }

    //sets player turn for combination on server
    [Command]
    void CmdSetCombinationTurn(bool turn)
    {
        player.turnForCombination = turn;
        if (!turn)
            GameManager.instance.NumOfAnsweredPlayers++;
    }

    //darkens the screen on cient
    IEnumerator DarkenTheScreen()
    {
        yield return new WaitForSeconds(0.325f);
        for (float f = 0; f < 1.1; f += 0.1f)
        {

            Color color = blackScreen.GetComponent<Image>().color;
            color.a = f;
            blackScreen.GetComponent<Image>().color = color;
            yield return new WaitForSeconds(0.0025f);
        }
    }

    //darkens the screen on client
    [TargetRpc]
    public void TargetStartDarken(NetworkConnection conn)
    {
        StartCoroutine("DarkenTheScreen");
    }

    //lightens the screen on client
    IEnumerator LightenTheScreen()
    {
        //yield return new WaitForSeconds(0.4f);
        for (float f = 1; f > -0.1; f -= 0.1f)
        {

            Color color = blackScreen.GetComponent<Image>().color;
            color.a = f;

            if (f < 0) color.a = 0;
            blackScreen.GetComponent<Image>().color = color;

            yield return new WaitForSeconds(0.005f);
        }
        CmdSetWallBuilt();
    }

    //starts lightning
    [TargetRpc]
    public void TargetStartLighten(NetworkConnection conn)
    {
        StartCoroutine("LightenTheScreen");
    }

    #endregion

    #region Quit

    //disconnects player
    public void DropConnection()
    {
        Debug.Log("disc");
        MatchInfo match = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(match.networkId, match.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
    }

    //indicates server command for disconnection
    public void Update()
    {
        if (isLocalPlayer && leave)
        {
            leave = false;
            DropConnection();
        }

    }

    #endregion

    #region GameMaster methods

    //tells server to kick player
    [Command]
    void CmdFailToStart()
    {
        RpcFailToStart();
    }

    //kicks player on clients
    [ClientRpc]
    void RpcFailToStart()
    {
        GameMaster.instance.KickAFK();

    }

    //marks player ready to start
    public void MarkReady()
    {
        if (ready) return;
        if (isLocalPlayer) CmdMarkReady();
    }

    //marks player ready on server
    [Command]
    public void CmdMarkReady()
    {
        RpcMarkReady();
    }

    //marks player ready on clients
    [ClientRpc]
    public void RpcMarkReady()
    {
        ready = true;
        GameMaster.instance.readyPlayers++;
    }


    //disables quitbutton
    [TargetRpc]
    public void TargetDisableInfoComponents(NetworkConnection conn)
    {
        QuitButton.SetActive(false);
        //numOfPlayers.enabled = false;
    }

    //indicates that the wall is built
    [Command]
    void CmdSetWallBuilt()
    {
        GameManager.instance.wallIsBuilt = true;
    }

    #endregion

    #region Chow

    //starts chow choice
    public void MakeChowChoice(Combination first, Combination second, Combination third)
    {
        //RefreshChows();
        string firstText = "";
        string secondText = "";
        string thirdText = "";

        firstChow = first;
        secondChow = second;
        thirdChow = third;

        GetChowSequence(first, ref firstText);
        GetChowSequence(second, ref secondText);
        GetChowSequence(third, ref thirdText);

        TargetGiveChowChoice(player.connectionToClient, firstText, secondText, thirdText);

    }

    //fills chow buttons
    void GetChowSequence(Combination comb, ref string text)
    {
        if (comb == null)
        {
            text = "";
            return;
        }
        for (int i = 0; i < comb.tileList.Count; i++)
        {
            text += $"{comb.tileList[i].name[comb.tileList[i].name.Length - 1]}";
            if (i != 2) text += "-";
        }
    }

    //shows chow buttons on client
    [TargetRpc]
    void TargetGiveChowChoice(NetworkConnection conn, string text1, string text2, string text3)
    {
        if (text3 != "") thirdButton.gameObject.SetActive(true);

        firstButton.GetComponentInChildren<Text>().text = text1;
        secondButton.GetComponentInChildren<Text>().text = text2;
        thirdButton.GetComponentInChildren<Text>().text = text3;

        chowPanel.SetActive(true);
    }


    //commands to select first button
    public void SelectFirstChow()
    {

        CmdSelectFirst();
        DisableChowPanel();
    }

    //selects first chow on server
    [Command]
    void CmdSelectFirst()
    {
        player.waitingCombination = firstChow;
        StopWaitForChow();

    }

    //commands to select second button
    public void SelectSecondChow()
    {
        CmdSelectSecond();
        DisableChowPanel();
    }

    //selects second chow on server
    [Command]
    void CmdSelectSecond()
    {
        player.waitingCombination = secondChow;

        StopWaitForChow();
        // DisableChowPanel();
    }

    //commands to select third button
    public void SelectThirdChow()
    {
        CmdSelectThird();
        DisableChowPanel();
    }

    //selects third chow on server
    [Command]
    void CmdSelectThird()
    {
        player.waitingCombination = thirdChow;
        StopWaitForChow();
    }

    //indicates answer on server
    void StopWaitForChow()
    {
        GameManager.instance.chowDeclarator = player;
        GameManager.instance.NumOfAnsweredPlayers++;
    }

    //hides buttons
    void DisableChowPanel()
    {
        if (!isLocalPlayer) return;

        StopWaitingForCombination();
        chowPanel.SetActive(false);

        thirdButton.gameObject.SetActive(false);
    }
    #endregion 
}
