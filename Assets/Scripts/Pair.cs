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
}

