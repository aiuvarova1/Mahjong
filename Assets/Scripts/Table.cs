using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Table
{
    // public List<Tile> tableTiles { get; set; }

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
        //tableTiles = new List<Tile>();
        startPosition = new Vector3(10, East.startPosition.y, East.startPosition.z);
        rotation = -90;
        CurrentPosition = startPosition;
    }

    public void MoveRightStartPosition()
    {
        currentPosition.z -= 2f;
    }

    public void MoveLeftStartPosition()
    {
        currentPosition.z += 2f;
    }


}

