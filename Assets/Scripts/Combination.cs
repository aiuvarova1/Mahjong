using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class Combination
{
    public int points;
    public int doubling=1;

    public bool opened = true;

    public string type;
   // public int firstIndex;

    public string Name { get; protected set; }

    public List<Tile >tileList = new List<Tile>();

    public Combination(Tile tile1,Tile tile2,Tile tile3)
    {
        //firstIndex = ind;
        tileList.Add(tile1);
        tileList.Add(tile2);
        tileList.Add(tile3);

    }

     public Combination()
    {

    }

    public abstract int CalculatePoints(string wind);


}

