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

    public List<Tile>[] tiles;

    public void Initialize(List<Tile>[] tiles)
    {
        this.tiles = tiles;
        
    }
}
