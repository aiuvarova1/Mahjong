using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class North:Wind
{
    const int num = 4;

    public static Vector3 startPosition = new Vector3(11, 1f, 27);

    public North(Player p) : base(p)
    {
        rotation = 0;
        freePosition = startPosition;
    }

    public override void RefreshFreePosition()
    {
        freePosition.x -= 2f;
    }

    public North()
    {
        rotation = 0;
        freePosition = startPosition;
    }
}

