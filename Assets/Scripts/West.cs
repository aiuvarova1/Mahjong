using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 class West:Wind
{
    const int num = 3;
 
    public Vector3 startPosition = new Vector3(30, 1f, -11);

    public Vector3 startOpenTilePosition = new Vector3(27, 0f, -15);
    public Vector3 startFlowerPosition = new Vector3(23, 0, -15);

    public West(Player p) : base(p)
    {

        rotation = 90;
        freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
        Name = "West";

    }

    public override void MoveRightFreePosition(ref Vector3 pos)
    {
        pos.z += 2;
    }

    public override void MoveLeftFreePosition(ref Vector3 pos)
    {
        pos.z -= 2;
    }

    public West()
    {
        rotation = 90;
        freePosition = startPosition;
        freeOpenPosition = startOpenTilePosition;
        freeFlowerPosition = startFlowerPosition;
        Name = "West";
    }
}

