using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 class West:Wind
{
    const int num = 3;
 
    
    public static Vector3 startPosition = new Vector3(28, 1f, -11);

    public West(Player p) : base(p)
    {
        rotation = 90;
        freePosition = startPosition;
    }

    public override void RefreshFreePosition()
    {
        freePosition.z += 2;
    }

    public West()
    {
        rotation = 90;
        freePosition = startPosition;
    }
}

