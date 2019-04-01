using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public List<Tile> playerTiles = new List<Tile>();

    public Camera Camera { get; set; }
    string name;
    public string wind;
    public int order;

    [Command]
    public void CmdAddWind()
    {
        switch (wind)
        {
            case "East":
                GameManager.instance.winds[0].player=this;
                return;
            case "South":
                GameManager.instance.winds[1].player = this ;
                return;
            case "West":
                GameManager.instance.winds[2].player=this;
                return;
            case "North":
                GameManager.instance.winds[3].player=this;
                return;
            default:

                return;

        }
    }

}
