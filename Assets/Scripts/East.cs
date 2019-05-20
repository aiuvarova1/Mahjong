using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 class East:Wind
{
    public East(Player p) : base(p)
    {
        startPosition = new Vector3(-31, 1f, 11);
        startOpenTilePosition = new Vector3(-27, 0, 15);
        startFlowerPosition = new Vector3(-23, 0, 15);

        rotation = -90;
        Refresh();
        Name = "East";  
    }

    // moves right free position (from player's point of view)
    public override void MoveRightFreePosition(ref Vector3 pos)
    {
        pos.z -= 2f;
    }

    // moves left free position (from player's point of view)
    public override void MoveLeftFreePosition(ref Vector3 pos)
    {
        pos.z += 2f;
    }

    // moves free position closer to player
    public override void MoveForwardPosition(ref Vector3 pos)
    {
        pos.x -= 2f;
    }


    public East()
    {
        startPosition = new Vector3(-30, 1f, 11);
        startOpenTilePosition = new Vector3(-27, 0, 15);
        startFlowerPosition = new Vector3(-23, 0, 15);

        rotation = -90;
        Refresh();
        Name = "East";
        
    }
}

