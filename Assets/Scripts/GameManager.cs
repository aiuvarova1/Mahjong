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
        bf.Serialize(o, BuildWall.instance.indexes); //Save the list
        
        var data = Convert.ToBase64String(o.GetBuffer()); //Convert the data to a string

        GameMaster.instance.DarkenScreens();

        RpcBuild(data);

        StartCoroutine(Lighten());

        Wall.instance.Initialize(BuildWall.instance.tiles);

        List<Tile>[] tiles = Wall.instance.tiles;


    }

    IEnumerator Lighten()
    {
        yield return new WaitForSeconds(0.9f);
        GameMaster.instance.LightenScreens();
        
    }

    [ClientRpc]
    public void RpcBuild(string data)
    {
        
        var ins = new MemoryStream(Convert.FromBase64String(data)); //Create an input stream from the string
                                                                    //Read back the data
        bf = new BinaryFormatter();
        List<int> indexes = (List<int>)bf.Deserialize(ins);

        StartCoroutine(Build(indexes));

        
    }

    IEnumerator Build(List<int> indexes)
    {
        yield return new WaitForSeconds(0.8f);
        BuildWall.instance.Build(indexes);
    }

}
