using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    

    public static List<Player> players = new List<Player>();
    private void Start()
    {
        
        

    }

    public static void AddPlayer(Player myPlayer)
    {
        players.Add(myPlayer);
    }
    //// Update is called once per frame
    //void Update()
    //{

    //}
}
