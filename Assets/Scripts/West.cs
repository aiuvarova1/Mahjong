using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class West : Wind
{
    public West(Player p) : base(p)
    {
        startPosition = new Vector3(31, 1f, -11);
        startOpenTilePosition = new Vector3(27, 0f, -15);
        startFlowerPosition = new Vector3(23, 0, -15);
        rotation = 90;
        base.Refresh();
        Name = "West";

    }

    //moves position closer to player
    public override void MoveForwardPosition(ref Vector3 pos)
    {
        pos.x += 2f;
    }

    //moves position righter
    public override void MoveRightFreePosition(ref Vector3 pos)
    {
        pos.z += 2;
    }

    //moves position lefter
    public override void MoveLeftFreePosition(ref Vector3 pos)
    {
        pos.z -= 2;
    }

    public West()
    {
        startPosition = new Vector3(30, 1f, -11);
        startOpenTilePosition = new Vector3(27, 0f, -15);
        startFlowerPosition = new Vector3(23, 0, -15);

        rotation = 90;

        base.Refresh();
        Name = "West";
    }
}

