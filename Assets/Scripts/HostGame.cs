using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HostGame : MonoBehaviour
{

    const uint roomSize = 4;

    public string RoomName { private get; set; }

    private NewNetworkManager networkManager;

    // Use this for initialization
    void Start()
    {
        networkManager = (NewNetworkManager)NewNetworkManager.singleton;
        if(networkManager.matchMaker==null)
            networkManager.StartMatchMaker();
        if (networkManager == null) Debug.Log("((");
    }

    public void CreateRoom()
    {
        if (RoomName!="" && RoomName != null)
        {
            Debug.Log(RoomName+ " created");
            networkManager.matchMaker.CreateMatch(RoomName, roomSize, true, "","","",0,0, networkManager.OnMatchCreate);

        }

    }
    private void Update()
    {

    }






}
