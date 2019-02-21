using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinGame : MonoBehaviour
{

    List<GameObject> roomList = new List<GameObject>();

    public GameObject roomPrefab;
    public GameObject parentPanel;
    public Text status;

    float repeatTime = 4;
    float currentTime;

    static NewNetworkManager networkManager;

    // Use this for initialization
    void Start()
    {
        //in ms
        currentTime = (repeatTime+5);
        networkManager = (NewNetworkManager)NetworkManager.singleton;
        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();

        status.text = "Loading...";
        RefreshRoomList();
        
    }

    public void RefreshRoomList()
    {
        networkManager.matchMaker.ListMatches(0, 15, "", true, 0, 0, OnMatchList);
    }

    void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        Debug.Log("Refresh");
        status.text = "";
        ClearRoomList();
        if (matches == null || matches.Count == 0 || !success)
        {
            status.text = "No rooms found";
            return;
        }
        
        foreach (MatchInfoSnapshot match in matches)
        {
            CreateRoomButton(match);
        }
    }

    public void CreateRoomButton(MatchInfoSnapshot match)
    {
        GameObject room = Instantiate(roomPrefab);
        room.transform.parent = parentPanel.transform;
        //??
        room.transform.localScale = new Vector3(1, 1, 1);
        // change room info on the button
        RoomListItem roomListItem = room.GetComponent<RoomListItem>();
        if (roomListItem != null) roomListItem.Setup(match, JoinRoom);
        roomList.Add(room);
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }
        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot match)
    {
        Debug.Log("meow");
        networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
    // Update is called once per frame
    void Update()
    {
        //refreshes list if Menu scene is active and lobby panel is on (every 4 seconds)
        if (SceneManager.GetActiveScene().name == "Lobby")

        {
            currentTime -= Time.deltaTime;
           //Debug.Log(currentTime);
            if (currentTime <= 0)
            {
                Debug.Log("Refresh");
                RefreshRoomList();
                currentTime = repeatTime;
            }
        }

    }
}
