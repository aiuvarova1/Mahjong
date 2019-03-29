using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public static Wall instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }

    public List<List<WallPair>> tiles;
    public List<Tile> freeTiles = new List<Tile>(2);

    public void Initialize(List<List<WallPair>> tiles)
    {
        this.tiles = tiles;
        
    }

    public void AssighFreeTiles()
    {
        int wallNum = Random.Range(0, tiles.Count);
        int restNum = Random.Range(2, 13);
    }
}
