using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Kong : Combination
{
    public Kong(Tile t1, Tile t2, Tile t3, int ind) : base(t1, t2, t3, ind) { }

    public Kong(Tile t1, Tile t2, Tile t3,Tile t4, int ind): base(t1, t2, t3, ind)
    {
        tileList.Add(t4);
    }
}

