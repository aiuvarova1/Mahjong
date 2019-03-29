using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class WallPair
{
    public Tile upperTile;
    public Tile lowerTile;

    public WallPair(Tile up, Tile down)
    {
        upperTile = up;
        lowerTile = down;
    }
    public WallPair()
    {

    }
}

