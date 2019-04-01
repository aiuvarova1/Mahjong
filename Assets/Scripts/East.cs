using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 class East:Wind
{
    const int num = 1;
    public static Vector3 startPosition = new Vector3(-28, 1f, 11);


    public East(Player p) : base(p)
    {
        rotation = -90;
        freePosition = startPosition;
    }

    public override void RefreshFreePosition()
    {
        freePosition.z -= 2f;
    }

    public East()
    {
        rotation = -90;
        freePosition = startPosition;
    }
}

