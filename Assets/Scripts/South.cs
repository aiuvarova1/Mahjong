using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class South:Wind
{
    const int num = 2;
    
    //public Vector3 
    public Vector3  startOpenTilePosition = new Vector3(-15, 0, -27);
    public Vector3  startFlowerPosition = new Vector3(-15, 0, -23);

    public South(Player p) : base(p)
    {
        startPosition = new Vector3(-11, 1f, -30);
        rotation = 180;
         freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
        Name = "South";
    }



    public override void MoveRightFreePosition(ref Vector3 pos)
    {
        pos.x += 2;
    }

    public override void MoveLeftFreePosition(ref Vector3 pos)
    {
        pos.x -= 2;
    }

    public South()
    {
        startPosition = new Vector3(-11, 1f, -30);
        rotation = 180;
        freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
        Name = "South";
    }
}

