using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class JoinGame : MonoBehaviour
{

    List<GameObject> roomList = new List<GameObject>();

    public GameObject roomPrefab;
    public GameObject parentPanel;
    public Text status;

    public static GameObject hostPanel;

    

    public static NewNetworkManager networkManager;

    IEnumerator refresher;

    public static void EnableHostPanel()
    {
        if (GameObject.FindGameObjectWithTag("Host") == null) return;
        hostPanel = GameObject.FindGameObjectWithTag("Host");
        //hostPanel.SetActive(true);

    }

    public static void DisableHostPanel()
    {
        if (GameObject.FindGameObjectWithTag("Host") == null) return;
        hostPanel = GameObject.FindGameObjectWithTag("Host");
        hostPanel.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {

        hostPanel = GameObject.FindGameObjectWithTag("Host");

        if (!NewNetworkManager.hostLeft)
            hostPanel.SetActive(false);
        else
            NewNetworkManager.hostLeft = false;

        Debug.Log("started");

        refresher = Refresher();

        networkManager = (NewNetworkManager)NetworkManager.singleton;
        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();

        if (LocalizationManager.instance==null|| LocalizationManager.instance.language == "Eng")
            status.text = "Loading...";
        else
            status.text = "Загрузка...";

        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref status);
        RefreshRoomList();
        StartCoroutine(refresher);
        
    }

    public void RefreshRoomList()
    {
        try
        {
            networkManager.matchMaker.ListMatches(0, 15, "", true, 0, 0, OnMatchList);
        }
        catch(NullReferenceException)
        {
            networkManager.StartMatchMaker();
        }
    }

    void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        Debug.Log("Refresh");
        status.text = "";
        ClearRoomList();
        if (matches == null || matches.Count == 0 || !success)
        {
            if (LocalizationManager.instance==null || LocalizationManager.instance.language == "Eng")
                status.text = "No rooms found";
            else
                status.text = "Нет доступных комнат";

            if (LocalizationManager.instance != null)
                LocalizationManager.instance.ChangeFont(ref status);
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
        room.transform.SetParent(parentPanel.transform);

        room.transform.localScale = new Vector3(1, 1, 1);

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
        networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        StopCoroutine(refresher);
        refresher = Refresher();
        ClearRoomList();

        //status.text = "Joining...";
        //StopCoroutine(Refresher());

        StartCoroutine(WaitForJoin());
    }
    IEnumerator WaitForJoin()
    {

        int countdown = 10;
        while (countdown > 0)
        {
            if (LocalizationManager.instance == null || LocalizationManager.instance.language == "Eng")
                status.text = "Joining... (" + countdown + ")";
            else
                status.text = "Соединение... (" + countdown + ")";

            if (LocalizationManager.instance != null)
                LocalizationManager.instance.ChangeFont(ref status);

            yield return new WaitForSeconds(1);

            countdown--;
        }

        // Failed to connect

        if (LocalizationManager.instance == null || LocalizationManager.instance.language == "Eng")
            status.text = "Failed to connect";
        else
            status.text = "Ошибка подключения";

        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref status);

        yield return new WaitForSeconds(1);

        MatchInfo matchInfo = networkManager.matchInfo;
        if (matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }

        yield return new WaitForSeconds(2);
        StartCoroutine(refresher);

    }

    IEnumerator Refresher()
    {
        yield return new WaitForSeconds(2);
        while (SceneManager.GetActiveScene().name == "Lobby")

        {
            yield return new WaitForSeconds(4);
            
            RefreshRoomList();

        }
    }

}
