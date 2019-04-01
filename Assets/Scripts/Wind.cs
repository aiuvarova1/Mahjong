using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class Wind:IComparable<Wind>
{
    public Player player { get;  set; }

    public Wind(Player p)
    {
        this.player = p;
    }

    public Wind() { }

    public  Vector3 freePosition;
    public float rotation;

    public int CompareTo(Wind wind)
    {
        if (this is East) return 1;
        else if (this is North) return -1;
        else if(this is South)
        {
            if (wind is East) return -1;
            return 1;
        }
        else
        {
            if (wind is North) return 1;
            return -1;
        }
    }

    public abstract void RefreshFreePosition();

}

