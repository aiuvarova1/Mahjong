using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 class East:Wind
{
    const int num = 1;
    public Vector3 startPosition = new Vector3(-30, 1f, 11);

    public Vector3  startOpenTilePosition = new Vector3(-27, 1f, 11);
    public Vector3  startFlowerPosition = new Vector3(-24, 0, 11);

    public East(Player p) : base(p)
    {
        rotation = -90;
        freePosition = startPosition;
        
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
    }


    public override void MoveRightFreePosition(ref Vector3 pos)
    {
        pos.z -= 2f;
    }

    public override void MoveLeftFreePosition(ref Vector3 pos)
    {
        pos.z += 2f;
    }


    public East()
    {
        rotation = -90;
        freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
    }
}

