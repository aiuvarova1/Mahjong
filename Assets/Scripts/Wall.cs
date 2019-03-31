using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wall : NetworkBehaviour
{
    //public static Wall instance;

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else if (instance != this) Destroy(gameObject);
    //}

    public List<List<WallPair>> tiles;
    public List<Tile> freeTiles = new List<Tile>();

    public void Initialize(List<List<WallPair>> tiles)
    {
        this.tiles = tiles;
        
    }

    public void AssighFreeTiles()
    {
        int wallNum = Random.Range(0, tiles.Count);
        int restNum = Random.Range(2, 13);

        Debug.Log(tiles.Count);
        //Debug.Log(tiles[wallNum].Count);

        //freeTiles.Add(tiles[wallNum][restNum].upperTile);
        //freeTiles.Add(tiles[wallNum][restNum].lowerTile);

        //RpcLieFreeTiles(freeTiles[0].tile,freeTiles[1].tile,
           // tiles[wallNum][restNum-2].upperTile.tile, tiles[wallNum][restNum - 1].upperTile.tile);

    }

    [ClientRpc]
    void RpcLieFreeTiles(GameObject tile1, GameObject tile2,
        GameObject newTile1, GameObject newTile2)
    {
        tile1.GetComponent<BezierMove>().StartMoveNewFreeTiles(newTile1);
        tile2.GetComponent<BezierMove>().StartMoveNewFreeTiles(newTile2);

    }

    public void Update()
    {
        if (!isServer) return;

        if(GameMaster.instance.gameState=="playing"
            && freeTiles.Count == 0)
        {

        }
    }
}
