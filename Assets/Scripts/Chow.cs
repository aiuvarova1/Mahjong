﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Chow : Combination
{
    public Chow(Tile t1, Tile t2, Tile t3) : base(t1, t2, t3)
    {
        Name = "Chow";
    }

    public override int CalculatePoints(string wind)
    {
        return 0;
    }
}

