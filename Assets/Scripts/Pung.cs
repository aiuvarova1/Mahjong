using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Pung : Combination
{
    public Vector3 additionalPosition;

    public Pung(Tile t1, Tile t2, Tile t3) : base(t1, t2, t3)
    {
        Name = "Pung";
    }
    public Pung(Tile t1, Tile t2, Tile t3, bool opened) : base(t1, t2, t3)
    {
        Name = "Pung";
        this.opened = opened;
    }

    //returns oung points
    public override int CalculatePoints(string wind)
    {
        int score = 0;

        if (tileList[0].name[1] == '1' || tileList[0].name[1] == '9' ||
            char.IsUpper(tileList[0].name[0]))
        {
            score = 4;
        }
        else score = 2;
        if (!opened) score *= 2;

        //your wind
        if (wind == tileList[0].name) doubling *= 2;

        if (tileList[0].name == GameManager.instance.majorWind) doubling *= 2;

        if (tileList[0].name == "Green" || tileList[0].name == "White" || tileList[0].name == "Red") doubling *= 2;

        return score;
    }
}

