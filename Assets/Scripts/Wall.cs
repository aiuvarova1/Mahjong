using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wall : NetworkBehaviour
{

    public List<List<WallPair>> tiles;
    public List<Tile> freeTiles = new List<Tile>();
    public static Wall instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);

    }

    public void Initialize(List<List<WallPair>> tiles)
    {
        this.tiles = tiles;
        
    }

    public void AssighFreeTiles()
    {
        //int wallNum = Random.Range(0, tiles.Count);
        //int restNum = Random.Range(2, 13);
        int wallNum = 1;
        int restNum = 5;

        Debug.Log(wallNum);
        Debug.Log(restNum);

        RpcLieFreeTiles(wallNum,restNum);

    }

    [ClientRpc]
    void RpcLieFreeTiles(int wallNum,int restNum)

    {
        freeTiles.Add(tiles[wallNum][restNum].upperTile);
        freeTiles.Add(tiles[wallNum][restNum].lowerTile);

        freeTiles[0].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][restNum-2].upperTile.tile,true);
        freeTiles[1].tile.GetComponent<BezierMove>().StartMoveNewFreeTiles(tiles[wallNum][restNum - 1].upperTile.tile,false);

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
