using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Pair : Combination
{
    public Pair(Tile tile1, Tile tile2)
    {
        tileList.Add(tile1);
        tileList.Add(tile2);
    }

    public override int CalculatePoints(string wind)
    {
        int score = 0;

        if (wind == tileList[0].name) score = 2 ;

        if (tileList[0].name == GameManager.instance.majorWind) score= 2;

        if (tileList[0].name == "Green" || tileList[0].name == "White" || tileList[0].name == "Red") score= 2;

        return score;

    }
}

