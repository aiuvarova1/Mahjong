using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine;


public class NewNetworkManager : NetworkManager
{
    public GameObject error;
    public Text errorText;

    //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    //{
    //    base.OnServerAddPlayer(conn,playerControllerId);

    //}


    void ShowErrorMessage(string message)
    {
        error.SetActive(true);
        errorText.text = message;
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        if (!success)
            ShowErrorMessage("Failed to create the game");
        //else
            //JoinGame.RefreshRoomList();

    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
        if (!success)
            ShowErrorMessage("Failed to join the game");
    }

}

