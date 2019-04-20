using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Pung : Combination
{
    public Vector3 additionalPosition;

    public Pung(Tile t1, Tile t2, Tile t3, int ind) : base(t1, t2, t3, ind) { }
}

