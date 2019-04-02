using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public List<Tile> playerTiles = new List<Tile>();
    public Vector3 startPosition;

    public Camera Camera { get; set; }
    string name;
    public string wind;
    public int order;

    public void SortTiles()
    {
        //Tile[] temp = new Tile[playerTiles.Count];
        //playerTiles.CopyTo(temp);

        List<Vector3> positions = CreatePositionList();

        playerTiles.Sort();
        Debug.Log("sorted");

        //playerTiles[0].tile.transform.position = startPosition;
        for (int i = 0; i < playerTiles.Count; i++)
        {
            Debug.Log(playerTiles[i]);
            playerTiles[i].tile.transform.position = positions[i];
        }
    }

    List<Vector3> CreatePositionList()
    {
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < playerTiles.Count; i++)
        {
            pos.Add(playerTiles[i].tile.transform.position);
        }
        return pos;
    }

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
