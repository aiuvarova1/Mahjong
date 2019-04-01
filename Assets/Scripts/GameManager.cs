using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;


public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;
    BinaryFormatter bf;

    public bool wallIsBuilt = false;

   public  List<Wind> winds = new List<Wind>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);

        winds.Add(new East());
        winds.Add(new South());
        winds.Add(new West());
        winds.Add(new North());
        
    }

    public void StartGame()
    {
        
        BuildWall.instance.FillIndexes();

        var o = new MemoryStream(); //Create something to hold the data

        bf = new BinaryFormatter(); //Create a formatter
        bf.Serialize(o, BuildWall.instance.indexes); //Save the list

        var data = Convert.ToBase64String(o.GetBuffer()); //Convert the data to a string
        BuildWall.instance.Build(BuildWall.instance.indexes);

        RpcBuildOnAllClients(data);
        //for server
        

        GameMaster.instance.DarkenScreens();

        StartCoroutine(Build());

        StartCoroutine(Lighten());

        Debug.Log(BuildWall.instance.tiles.Count + "tiles");

        //winds.Sort();
        //winds.Reverse();



    }

    public void DistributeTiles()
    {
        Wall.instance.AssighFreeTiles();

        Wall.instance.DistributeTiles();
    }

    [ClientRpc]
    void RpcBuildOnAllClients(string data)
    {

       

        if (isServer) return;

        var ins = new MemoryStream(Convert.FromBase64String(data)); //Create an input stream from the string
                                                                    //Read back the data
        bf = new BinaryFormatter();
        List<int> indexes = (List<int>)bf.Deserialize(ins);
        BuildWall.instance.Build(indexes);
    }


    IEnumerator Lighten()
    {
        yield return new WaitForSeconds(1.9f);
        GameMaster.instance.LightenScreens();
    }


    IEnumerator Build()
    {

        yield return new WaitForSeconds(1.8f);

        RpcMakeTilesVisible();

    }

    [ClientRpc]
    void RpcMakeTilesVisible()
    {
       // NetworkManager.
        foreach (List<WallPair> lst in BuildWall.instance.tiles)
        {
            foreach (WallPair pair in lst)
            {
                pair.upperTile.MakeVisible();
                pair.lowerTile.MakeVisible();
            }
        }
    }










}
