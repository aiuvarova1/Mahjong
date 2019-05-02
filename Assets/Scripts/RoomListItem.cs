using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine;

public class RoomListItem : NetworkBehaviour {

    MatchInfoSnapshot match;
    public Text roomInfo;
    public delegate void JoinRoomDelegate(MatchInfoSnapshot match);
    JoinRoomDelegate joinRoomDelegate;

    public void Setup(MatchInfoSnapshot myMatch,JoinRoomDelegate joinRoomCallback)
    {
        match = myMatch;
        joinRoomDelegate = joinRoomCallback;
        roomInfo.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")" ;
    }

    public void JoinRoom()
    {
        Debug.Log("join");
        joinRoomDelegate.Invoke(match);
    }
}
