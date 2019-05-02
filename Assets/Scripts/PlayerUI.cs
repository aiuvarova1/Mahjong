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

    public Text numOfPlayers;
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

    public Button thirdButton;
    public Button firstButton;
    public Button secondButton;

    Combination firstChow;
    Combination secondChow;
    Combination thirdChow;

    Camera mainCam;


    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        networkManager = (NewNetworkManager)NetworkManager.singleton;
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

       
    }

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
        combinationInfo.enabled = false;


        //!!!
        //GameObject names = GameObject.FindGameObjectWithTag("Names");
        //Button[] objects=names.GetComponentsInChildren<Button>();

        //objects[1].GetComponentInChildren<Text>().text = "Pobeda";

        scorePanel.SetActive(false);
        

    }

    public void ChangeCamera()
    {
        
        if(player.Camera.enabled)
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

    #region Coroutines

    [TargetRpc]
    public void TargetShowScores(NetworkConnection conn,int[] scores,int[] oldScores,string[] names,string winner)
    {
        scorePanel.SetActive(true);

        QuitButton.SetActive(true);

        GameObject tableNames = GameObject.FindGameObjectWithTag("Names");
        Button[] objects = tableNames.GetComponentsInChildren<Button>();

        for (int i = 1; i < names.Length; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = names[i - 1];
        }

        tableNames= GameObject.FindGameObjectWithTag("OldScore");
        objects= tableNames.GetComponentsInChildren<Button>();


        for (int i = 1; i < names.Length; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = (oldScores[i - 1]).ToString();
        }

        tableNames = GameObject.FindGameObjectWithTag("Points");
        objects = tableNames.GetComponentsInChildren<Button>();


        for (int i = 1; i < names.Length; i++)
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

        //set column names
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i][0].GetComponentInChildren<Text>().text = (names[i]).ToString();
            for (int j = 1; j < buttons[i].Length; j++)
            {
                buttons[i][j].GetComponentInChildren<Text>().text = matrix[i, j - 1].ToString();
            }
            
        }


        tableNames = GameObject.FindGameObjectWithTag("Total");
        objects = tableNames.GetComponentsInChildren<Button>();

        List<int> total = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                total[i] += matrix[j, i];
            }
           
        }

        for (int i = 1; i < names.Length; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = (total[i-1]).ToString();
        }

        tableNames = GameObject.FindGameObjectWithTag("NewScore");
        objects = tableNames.GetComponentsInChildren<Button>();

        for (int i = 1; i < names.Length; i++)
        {
            objects[i].GetComponentInChildren<Text>().text = (oldScores[i-1]+total[i - 1]).ToString();
        }

    }

    void FillMatrix(ref int[,]matrix,int[] scores,string winner)
    {
        int winNum=-1;

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
                            if(j==0)
                                matrix[j, i] = -scores[0] * 2;
                            else
                                matrix[j, i] = scores[i]-scores[j];
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
                                if(j == winNum)
                                    matrix[j, i] = -scores[j];
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

    [TargetRpc]
    public void TargetShowInfo(NetworkConnection conn ,string info)
    {
        if (LocalizationManager.instance==null|| LocalizationManager.instance.language == "Eng")
            combinationInfo.text = info;
        else
            combinationInfo.text = LocalizationManager.instance.GetLocalizedValue(info);

        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref combinationInfo);
        StopCoroutine(hider);
        StopHideInfo();
        StartCoroutine(hider);
    }
    IEnumerator HideInfo()
    {
        combinationInfo.enabled = true;
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

    void StopHideInfo()
    {
        
        Color fullCol = combinationInfo.color;
        fullCol.a = 1f;
        combinationInfo.color = fullCol;
        hider = HideInfo();
    }

    IEnumerator DeclareCombination(string wind,string combination)
    {
        //!!!
        if(LocalizationManager.instance == null || LocalizationManager.instance.language=="Eng")
            declaration.text = $"{wind} declares {combination}!";
        else
            declaration.text = $"{LocalizationManager.instance.GetLocalizedValue(wind)} объявляет {LocalizationManager.instance.GetLocalizedValue(combination)}!";

        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref declaration);


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
        Color fullCol = declaration.color;
        fullCol.a = 1f;
        declaration.color = fullCol;
    }

    [TargetRpc]
    public void TargetShowDeclaredCombination(NetworkConnection conn,string wind, string combination)
    {
        StartCoroutine(DeclareCombination(wind,combination));
    }



    IEnumerator WaitForStart()
    {

        int countdown = 20;

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
            Debug.Log(countdown);
            countdown--;
        }
        //infoPanel.SetActive(false);
        
        CmdFailToStart();
        StopWaiting();
    }

    [TargetRpc]
    public void TargetStopWaiting(NetworkConnection conn)
    {
        StopWaiting();
    }
    public void StopWaiting()
    {
        StopCoroutine(starter);
        starter = WaitForStart();
        infoPanel.SetActive(false);
        infoText.text = " ";
    }

    [TargetRpc]
    public void TargetStartWaiting(NetworkConnection conn)
    {
        StartCoroutine(starter);
    }

    IEnumerator WaitForMove()
    {
        Debug.Log("wait");
        int countdown = 30;

        countDown.text = "30";

        countDown.enabled = true;

        while (countdown > 0)
        {
            countDown.text = $"{countdown}";

            yield return new WaitForSeconds(1);
            countdown--;
        }
        //countDown.enabled = false;
        
        player.SelectTile(player.selectedTile.tile);
        player.startedCoroutine = false;
        StopWaitingForMove();
        //CmdStopCoroutine();

    }
    //[Command]
    //void CmdStopCoroutine()
    //{
    //    player.startedCoroutine = false;
    //}

    public void LaunchWaitForMove()
    {
        if (isClient)
        {
            
            StartCoroutine(CountDown);
        }
    }

    [TargetRpc]
    public void TargetStopWaitingForMove(NetworkConnection conn)
    {
        StopWaitingForMove();
    }


    public void StopWaitingForMove()
    {
        StopCoroutine(CountDown);
        CountDown = WaitForMove();
        countDown.enabled = false;
        player.startedCoroutine = false;
    }

    IEnumerator WaitForCombination()
    {

        //player.turnForCombination = true;
        CmdSetCombinationTurn(true);
        int countdown = 20;

        countDown.text = "20";

        countDown.enabled = true;

        while (countdown > 0)
        {
            countDown.text = $"{countdown}";

            yield return new WaitForSeconds(1);
            countdown--;
        }
        //countDown.enabled = false;
        

        if (chowPanel.activeSelf)
            chowPanel.SetActive(false);
        if (thirdButton.enabled)
            thirdButton.enabled = false;

       // player.CmdAnswerToServer();
        //GameManager.instance.NumOfAnsweredPlayers++;
        //player.turnForCombination = false;
        CmdSetCombinationTurn(false);
        //player.playerTurn = false;
        StopWaitingForCombination();
    }


    public void LaunchWaitForCombination()
    {

        StartCoroutine(CombinationEnum);
    }


    [TargetRpc]
    public void TargetStopWaitingForCombination(NetworkConnection conn)
    {

        StopWaitingForCombination();

    }

    public void StopWaitingForCombination()
    {
        StopCoroutine(CombinationEnum);
        CombinationEnum = WaitForCombination();
        countDown.enabled = false;
        player.turnForCombination = false;
    }



    [Command]
    void CmdSetCombinationTurn(bool turn)
    {
        player.turnForCombination = turn;
        if (!turn)
            GameManager.instance.NumOfAnsweredPlayers++;
    }

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

    [TargetRpc]
    public void TargetStartDarken(NetworkConnection conn)
    {
        StartCoroutine("DarkenTheScreen");
    }

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

    [TargetRpc]
    public void TargetStartLighten(NetworkConnection conn)
    {
        StartCoroutine("LightenTheScreen");
    }



    #endregion

    #region Quit
    public void DropConnection()
    {
        MatchInfo match = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(match.networkId, match.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
    }

    public void LeaveRoom()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Leave");
            string netID = GetComponent<NetworkIdentity>().netId.ToString();
            player.GetComponent<SetupPlayer>().CmdUnregisterPlayer(netID);

        }

    }
    #endregion

    #region GameMaster methods
    [Command]
    void CmdFailToStart()
    {
        RpcFailToStart();
    }

    [ClientRpc]
    void RpcFailToStart()
    {
        GameMaster.instance.KickAFK();

    }

    public void MarkReady()
    {
        if (ready) return;
        if (isLocalPlayer) CmdMarkReady();
    }

    [Command]
    public void CmdMarkReady()
    {
        RpcMarkReady();
    }

    [ClientRpc]
    public void RpcMarkReady()
    {
        ready = true;
        GameMaster.instance.readyPlayers++;

    }

    [TargetRpc]

    public void TargetRefreshPlayers(NetworkConnection conn, int num)
    {
        numOfPlayers.text = $"Players: {num}/4";
    }

    [TargetRpc]
    public void TargetDisableInfoComponents(NetworkConnection conn)
    {
        QuitButton.SetActive(false);
        numOfPlayers.enabled = false;
    }


    [Command]
    void CmdSetWallBuilt()
    {
        GameManager.instance.wallIsBuilt = true;
    }

    #endregion

    #region Chow
    public void MakeChowChoice(Combination first,Combination second,Combination third)
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

    void GetChowSequence(Combination comb,ref string text)
    {
        if (comb == null)
        {
            Debug.Log("comb is null");
            text = "";
            return;
        }
        for (int i = 0; i < comb.tileList.Count; i++)
        {
            text += $"{comb.tileList[i].name[comb.tileList[i].name.Length - 1]}";
            if (i != 2) text += "-";
        }
    }

    [TargetRpc]
    void TargetGiveChowChoice(NetworkConnection conn, string text1,string text2,string text3)
    {
        if (text3 != "") thirdButton.gameObject.SetActive(true);

        firstButton.GetComponentInChildren<Text>().text = text1;
        secondButton.GetComponentInChildren<Text>().text = text2;
        thirdButton.GetComponentInChildren<Text>().text = text3;

        chowPanel.SetActive(true);
    }



    public void SelectFirstChow()
    {

        CmdSelectFirst();
        DisableChowPanel();
    }

    [Command]
    void CmdSelectFirst()
    {
        player.waitingCombination = firstChow;

        StopWaitForChow();
        
    }

    public void SelectSecondChow()
    {
        CmdSelectSecond();
        DisableChowPanel();
    }

    [Command]
    void CmdSelectSecond()
    {
        player.waitingCombination = secondChow;

        StopWaitForChow();
       // DisableChowPanel();
    }

    public void SelectThirdChow()
    {

        CmdSelectThird();
        DisableChowPanel();

    }

    [Command]
    void CmdSelectThird()
    {
        player.waitingCombination = thirdChow;

        StopWaitForChow();
        //DisableChowPanel();
    }

    void StopWaitForChow()
    {
        GameManager.instance.chowDeclarator = player;
        GameManager.instance.NumOfAnsweredPlayers++;

        
    }

    void DisableChowPanel()
    {
        if (!isLocalPlayer) return;

        StopWaitingForCombination();
        chowPanel.SetActive(false);

        thirdButton.gameObject.SetActive(false);
    }
    #endregion

    public void Update()
    {
        if (isLocalPlayer && leave)
        {
            leave = false;
            LeaveRoom();
        }
 
    }
}
