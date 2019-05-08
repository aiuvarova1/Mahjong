using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class North:Wind
{
    const int num = 4;

    //public Vector3 

     

    public North(Player p) : base(p)
    {
        startPosition = new Vector3(11, 1f, 31);
        startOpenTilePosition = new Vector3(15, 0f, 27);
        startFlowerPosition = new Vector3(15, 0, 23);

        rotation = 0;
        Refresh();
        Name = "North";
    }



    public override void MoveRightFreePosition(ref Vector3 pos)
    {
        pos.x -= 2f;
    }

    public override void MoveLeftFreePosition(ref Vector3 pos)
    {
        pos.x += 2f;
    }

    public override void MoveForwardPosition(ref Vector3 pos)
    {
        pos.z += 2f;
    }

    public North()
    {
        startPosition = new Vector3(11, 1f, 30);
        startOpenTilePosition = new Vector3(15, 0f, 27);
        startFlowerPosition = new Vector3(15, 0, 23);

        rotation = 0;
        Refresh();
        Name = "North";
    }
}

