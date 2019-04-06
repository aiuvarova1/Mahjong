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


    [SerializeField]
    public Canvas canvas;

    IEnumerator starter;

    public bool leave = false;
    public bool ready = false;

    public GameObject blackScreen;


    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        networkManager = (NewNetworkManager)NetworkManager.singleton;
        //if(isLocalPlayer)
       // blackScreen = GameObject.Find("BlackScreen");
            
        
       // StartCoroutine(starter);
    }

    private void Awake()
    {
        starter = WaitForStart();
        infoPanel.SetActive(false);
    }


    [TargetRpc]

    public void TargetRefreshPlayers(NetworkConnection conn,int num)
    {
        numOfPlayers.text = $"Players: {num}/4";
    }

    [TargetRpc]
    public void TargetDisableInfoComponents(NetworkConnection conn)
    {
        QuitButton.SetActive(false);
        numOfPlayers.enabled = false;
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
        for (float f = 1; f >-0.1; f -= 0.1f)
        {

            Color color = blackScreen.GetComponent<Image>().color;
            
            color.a = f;

            if (f < 0) color.a = 0;

            blackScreen.GetComponent<Image>().color = color;

            yield return new WaitForSeconds(0.005f);
        }
        CmdSetWallBuilt();
    }

    [Command]
    void CmdSetWallBuilt()
    {
        GameManager.instance.wallIsBuilt = true;
    }

    [TargetRpc]
    public void TargetStartLighten(NetworkConnection conn)
    {
        StartCoroutine("LightenTheScreen");
    }


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

    [TargetRpc]
    public void TargetStartWaiting(NetworkConnection conn)
    {
        Debug.Log(starter == null);
        StartCoroutine(starter);
    }

    IEnumerator WaitForStart()
    {

        int countdown = 20;

        string prefix = "Ready to start? ";
        infoPanel.SetActive(true);

        while (countdown > 0)
        {
            if (ready == true)
            {
                prefix = "Waiting for other \n   players...";
                startButton.SetActive(false);
            }
            infoText.text = prefix + $"({countdown})";
            yield return new WaitForSeconds(1);
            Debug.Log(countdown);
            countdown--;
        }
        infoPanel.SetActive(false);
        CmdFailToStart();
    }

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
    public void TargetStopWaiting(NetworkConnection conn)
    {
        StopCoroutine(starter);
        infoPanel.SetActive(false);
        infoText.text = " ";
    }
    

    public void Update()
    {
        if (isLocalPlayer && leave)
        {
            leave = false;
            LeaveRoom();
        }
 
    }
}
