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

    public bool leave = false;
    public bool ready = false;

    public GameObject blackScreen;

    public Text toolTipText;

    public GameObject chowPanel;

    public Button thirdButton;
    public Button firstButton;
    public Button secondButton;

    Combination firstChow;
    Combination secondChow;
    Combination thirdChow;


    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        networkManager = (NewNetworkManager)NetworkManager.singleton;

    }

    private void Awake()
    {
        starter = WaitForStart();
        CountDown = WaitForMove();
        CombinationEnum = WaitForCombination();

        infoPanel.SetActive(false);
        countDown.enabled = false;

        thirdButton.gameObject.SetActive(false);
        chowPanel.SetActive(false);
        declaration.enabled = false;
        combinationInfo.enabled = false;
        

    }

    #region Coroutines

    [TargetRpc]
    public void TargetShowInfo(NetworkConnection conn ,string info)
    {
        combinationInfo.text = info;
        StartCoroutine(HideInfo());
    }
    IEnumerator HideInfo()
    {
        combinationInfo.enabled = true;
        yield return new WaitForSeconds(4f);

        while (combinationInfo.color.a > 0)
        {
            Color col = combinationInfo.color;
            col.a -= 0.1f;
            combinationInfo.color = col;
            yield return new WaitForSeconds(0.3f);
        }
        combinationInfo.enabled = false;
        Color fullCol = combinationInfo.color;
        fullCol.a = 1f;
        combinationInfo.color = fullCol;

    }

    IEnumerator DeclareCombination(string wind,string combination)
    {
        declaration.text = $"{wind} declares {combination}!";
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

        string prefix = "Ready to start? ";
        infoPanel.SetActive(true);

        while (countdown > 0)
        {
            if (ready)
            {
                prefix = "Waiting for other \n   players...";
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
        yield return new WaitForSeconds(2);
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
        yield return new WaitForSeconds(0.3f);
        for (float f = 0; f < 1.1; f += 0.1f)
        {

            Color color = blackScreen.GetComponent<Image>().color;
            color.a = f;
            blackScreen.GetComponent<Image>().color = color;

            yield return new WaitForSeconds(0.005f);
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
