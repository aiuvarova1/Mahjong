using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Chow : Combination
{
    public Chow(Tile t1, Tile t2, Tile t3) : base(t1, t2, t3)
    {
        Name = "Chow";
    }

    //returns points for chow - always 0
    public override int CalculatePoints(string wind)
    {
        Debug.Log("chow" + tileList[0].name[0]);
        return 0;
    }
}

