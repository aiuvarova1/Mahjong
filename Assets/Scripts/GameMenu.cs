using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class GameMenu : MonoBehaviour {

    NewNetworkManager networkManager;
	// Use this for initialization
	void Start () {
        networkManager = (NewNetworkManager)NetworkManager.singleton;
	}

    //public void LeaveRoom()
    //{
    //    MatchInfo match = networkManager.matchInfo;

    //    //GetComponent<Player>().AddCamera();
    //    //GameManager.AddCameras(GetComponent<Player>().Cam.name);
    //    networkManager.matchMaker.DropConnection(match.networkId, match.nodeId, 0, networkManager.OnDropConnection);
    //    networkManager.StopHost();
    //}
	
}
