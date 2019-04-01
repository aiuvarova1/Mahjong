using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class South:Wind
{
    const int num = 2;
    
    public static Vector3 startPosition = new Vector3(-11, 1f, -27);


    public South(Player p) : base(p)
    {
          rotation = 180;
         freePosition = startPosition;
    }

    public override void RefreshFreePosition()
    {
        freePosition.x += 2;
    }

    public South()
    {
        rotation = 180;
        freePosition = startPosition;
    }
}

