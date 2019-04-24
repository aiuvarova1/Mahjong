using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Kong : Combination
{

    bool opened;

    public Kong(Tile t1, Tile t2, Tile t3,bool openedComb) : base(t1, t2, t3)
    {
        Name = "Kong";
        opened = openedComb;
    }

    public Kong(Tile t1, Tile t2, Tile t3,Tile t4,  bool openedComb) : base(t1, t2, t3)
    {
        tileList.Add(t4);
        opened = openedComb;
    }

    public Kong(Pung pung,Tile t4, bool openedComb) :base(pung.tileList[0],pung.tileList[1],pung.tileList[2])
    {
        opened = openedComb;
    }
}

