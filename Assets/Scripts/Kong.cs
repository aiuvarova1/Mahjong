using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Kong : Combination
{
    public Kong(Tile t1, Tile t2, Tile t3,Tile t4,  bool openedComb) : base(t1, t2, t3)
    {
        tileList.Add(t4);
        opened = openedComb;
    }

    public Kong(Pung pung,Tile t4, bool openedComb) :base(pung.tileList[0],pung.tileList[1],pung.tileList[2])
    {
        tileList.Add(t4);
        opened = openedComb;
    }

    public override int CalculatePoints(string wind)
    {
        int score = 0;

        Debug.Log("kong " + tileList[0].name);

        if (tileList[0].name[1] == '1' || tileList[0].name[1] == '9' ||
            char.IsUpper(tileList[0].name[0]))
        {

            score = 16;

        }
        else score = 8;

        if (!opened) score *= 2;

        Debug.Log(score);

        //your wind
        if (wind == tileList[0].name) doubling *= 2;

        if (tileList[0].name == GameManager.instance.majorWind) doubling *= 2;

        if (tileList[0].name == "Green" || tileList[0].name == "White" || tileList[0].name == "Red") doubling *= 2;

        Debug.Log("double " + doubling);

        return score;

    }
}

