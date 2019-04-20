using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class Combination
{
    public int points;
    public string type;
    public int firstIndex;
    

    public List<Tile >tileList = new List<Tile>();

    public Combination(Tile tile1,Tile tile2,Tile tile3,int ind)
    {
        firstIndex = ind;
        tileList.Add(tile1);
        tileList.Add(tile2);
        tileList.Add(tile3);

    }
}

