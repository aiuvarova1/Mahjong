using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class North:Wind
{
    const int num = 4;

    public Vector3 startPosition = new Vector3(11, 1f, 30);

    public Vector3 startOpenTilePosition = new Vector3(11, 0f, 27);
    public Vector3 startFlowerPosition = new Vector3(11, 0, 23);

    public North(Player p) : base(p)
    {
        rotation = 0;
        freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
    }



    public override void MoveRightFreePosition(ref Vector3 pos)
    {
        pos.x -= 2f;
    }

    public override void MoveLeftFreePosition(ref Vector3 pos)
    {
        pos.x += 2f;
    }

    public North()
    {
        rotation = 0;
        freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
    }
}

