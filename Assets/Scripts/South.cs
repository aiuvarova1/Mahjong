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
    

    public South(Player p) : base(p)
    {
        startPosition = new Vector3(-11, 1f, -30);
        startOpenTilePosition = new Vector3(-15, 0, -27);
        startFlowerPosition = new Vector3(-15, 0, -23);

        rotation = 180;
        // freePosition = startPosition;
        //freeOpenPosition = startOpenTilePosition;
        //freeFlowerPosition = startFlowerPosition;
        base.Refresh();
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
        startOpenTilePosition = new Vector3(-15, 0, -27);
        startFlowerPosition = new Vector3(-15, 0, -23);

        rotation = 180;
        //freePosition = startPosition;
        //freeOpenPosition = startOpenTilePosition;
        //freeFlowerPosition = startFlowerPosition;
        base.Refresh();
        Name = "South";
    }

    
}

