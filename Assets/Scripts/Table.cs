using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Table
{
    public Tile lastTile;
    private Vector3 startPosition;
    private Vector3 currentPosition;

    public float rotation;

    public Vector3 CurrentPosition
    {
        get { return currentPosition; }
        set
        {
            currentPosition = value;
            if (currentPosition.z < -12)
                currentPosition = new Vector3(currentPosition.x - 3, currentPosition.y, startPosition.z);

        }
    }


    public Table()
    {
        startPosition = new Vector3(10, GameManager.instance.winds[0].startPosition.y, GameManager.instance.winds[0].startPosition.z);
        rotation = -90;
        CurrentPosition = startPosition;
    }

    //moves right free table position
    public void MoveRightStartPosition()
    {
        Vector3 curPos = CurrentPosition;
        curPos.z -= 2f;
        CurrentPosition = curPos;
    }

    //moves left free table position
    public void MoveLeftStartPosition()
    {
        Vector3 curPos = CurrentPosition;
        curPos.z += 2f;
        if (curPos.z > startPosition.z)
        {
            curPos.z = -11f;
            curPos.x += 3f;
        }
        CurrentPosition = curPos;
    }
}

