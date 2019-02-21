using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player: NetworkBehaviour {


    //public static List<Camera> availableCameras = allCameras;
    //[SerializeField]
    //List<Camera> allCameras = new List<Camera>();

    //[SyncVar(hook ="myHook")]
    //public SyncListInt availableCameras=new SyncListInt();

    //public List<Camera> allCameras = new List<Camera>();

    //public GameObject playerPrefab;
    //public Camera camera;
    public Camera Cam { get; private set; }
    string name;
    string wind;
    int order;

    GameMaster gameMaster;

    
    //private void myHook(SyncListInt changedList)
    //{
    //    availableCameras = changedList;
    //} 

    [Command]
    void CmdIncrease(int order)
    {
        
        gameMaster.AddPlayer(this,order);
        RpcIncrease(gameMaster.playerCount);
    }

    [ClientRpc]
    void RpcIncrease(int count)
    {
        Debug.Log($"{count} players");
        for (int i = 0; i < gameMaster.availableCameras.Count; i++)
        {
            Debug.Log($"Camera{gameMaster.availableCameras[i]}");
        }
        Debug.Log($"{gameMaster.availableCameras.Count} count");
    }

    void Start()
    {
        GameObject script = GameObject.Find("GameMaster");
        gameMaster = (GameMaster)script.GetComponent(typeof(GameMaster));

        if (!isLocalPlayer)
        {
            Debug.Log("not local "+order);
            return;

        }

        

        //if (availableCameras.Count == 0)
        //{
        //    for (int i = 0; i < 4; i++)
        //    {
        //        Debug.Log("new");
        //        availableCameras.Add(i);
        //    }
        //}

        order = Random.Range(0, gameMaster.availableCameras.Count);
        Debug.Log(order);

        

        Cam = Instantiate(gameMaster.allCameras[order]);
        Debug.Log(Cam.name);

        //CmdRemoveCamera(order);
        
        Cam.enabled = false;

        for (int i = 0; i < gameMaster.players.Count; i++)
        {
            if (gameMaster.players[i].Cam != null && gameMaster.players[i].Cam.enabled)
            {
                gameMaster.players[i].Cam.enabled = false;
            }
        }
        CmdIncrease(order);
        //camera = this.GetComponent<Camera>();
        Cam.enabled = true;

        //GameManager.AddPlayer(this);


        Debug.Log($"av cams {gameMaster.availableCameras.Count}");
    }

    //[Command]
    //void CmdRemoveCamera(int index)
    //{
    //    availableCameras.Remove(order);
    //    RpcRemoveCamera();
    //}

    //[Command]
    //public void CmdAddCamera()
    //{
    //    availableCameras.Add(order);
    //    RpcRemoveCamera();
    //}

    //[ClientRpc]
    //void RpcRemoveCamera()
    //{
        
    //    Debug.Log("count " + availableCameras.Count);
    //}


    //public void AddCamera()
    //{
    //    availableCameras.Add(order);
    //    Debug.Log(availableCameras.Count);
    //}


    // Update is called once per frame
    void Update () {

	}
}
