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

    [SerializeField]
    public Canvas canvas;

    IEnumerator starter;

    public bool leave = false;
    public bool ready = false;


    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        networkManager = (NewNetworkManager)NetworkManager.singleton;
        //if(isLocalPlayer)
            
        
       // StartCoroutine(starter);
    }

    private void Awake()
    {
        starter = WaitForStart();
        infoPanel.SetActive(false);
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
        Debug.Log("wait");
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
        Debug.Log(isLocalPlayer);
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
        Debug.Log(GameMaster.instance.readyPlayers);
    }

    [TargetRpc]
    public void TargetStartGame(NetworkConnection conn)
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
