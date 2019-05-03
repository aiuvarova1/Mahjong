using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 class East:Wind
{
    const int num = 1;
   // public Vector3 

    public Vector3  startOpenTilePosition = new Vector3(-27, 0, 15);
    public Vector3  startFlowerPosition = new Vector3(-23, 0, 15);

    public East(Player p) : base(p)
    {
        startPosition = new Vector3(-30, 1f, 11);
        rotation = -90;
        freePosition = startPosition;
        
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
        Name = "East";
        
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
        startPosition = new Vector3(-30, 1f, 11);
        rotation = -90;
        freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
        Name = "East";
        
    }
}

