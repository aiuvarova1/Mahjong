using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;
    BinaryFormatter bf;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }

    public void StartGame()
    {
        Debug.Log("start");

        BuildWall.instance.FillIndexes();

        //serializing to pass data(rpc can't take lists)

        var o = new MemoryStream(); //Create something to hold the data

        bf = new BinaryFormatter(); //Create a formatter
        bf.Serialize(o, BuildWall.indexes); //Save the list
        
        var data = Convert.ToBase64String(o.GetBuffer()); //Convert the data to a string

        RpcBuild(data);

        Wall.instance.Initialize(BuildWall.instance.tiles);

        List<Tile>[] tiles = Wall.instance.tiles;

        //for (int i = 0; i < tiles.Length; i++)
        //{
        //    for (int j = 0; j < tiles[i].Count; j++)
        //    {
        //       // NetworkServer.Spawn(tiles[i][j].tile);
        //    }
        //}
        //NetworkServer.Spawn(BuildWall.instance.tilePrefab);

    }

    [ClientRpc]
    public void RpcBuild(string data)
    {
        var ins = new MemoryStream(Convert.FromBase64String(data)); //Create an input stream from the string
                                                                    //Read back the data
        bf = new BinaryFormatter();
        List<int> indexes = (List<int>)bf.Deserialize(ins);

        BuildWall.instance.Build(indexes);
    }

}
