using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BezierMove : NetworkBehaviour
{
    float thirdRowHeight = 3.4f;

    public Vector3 endPoint;
    Quaternion endRotation;

    Vector3 p0, p1, p3;

    int num = 1;

    public bool moving = false;
    public bool rotating = false;


    public float speed = 50f;

    List<Vector3> pos = new List<Vector3>();

    public Player owner;
    public bool check = false;

    //opens given tile to player
    public void OpenTile(float rotation)
    {
        endRotation= Quaternion.Euler(-90, rotation, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);
       
        rotating = true; 
    }

    //moves tile to the end position
    public void Move(Vector3 end,float rotation )
    {

        FillArray(transform.position,end,8.4f);

        //moving from pos[0] to pos[1]
        endPoint = pos[1];
        endRotation = Quaternion.Euler(0, rotation, 0);
        
        StartCoroutine(WaitForMoving(rotation));
    }

    //opens tile after it stops moving
    IEnumerator WaitForMoving(float rotation)
    {
        moving = true;
        rotating = true;

        if (check && owner!=null)
            owner.needToCheckMoving = true;

        while (moving)
            yield return new WaitForSeconds(0.1f);
        OpenTile(rotation);
    }

    //lies out tile in combination
    public void LieOut(Vector3 endPos,float rotation,bool closed)
    {

        if (closed)
        {
            endPos.y = 0.9f;
            endRotation = Quaternion.Euler(0, rotation, 0);
        }
        else
        {
            endPos.y += 0.1f;
            endRotation = Quaternion.Euler(-180, rotation, 0);
        }

        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
       
        FillArray(transform.position, endPos, 2.4f);

        endPoint = pos[1];

        moving = true;
        rotating = true;

        if (check && owner != null)
            owner.needToCheckMoving = true;
    }

    //calculates bezier curve point
    Vector3 GetBezierPosition(float t)
    {
        return p1 + (1 - t) * (1 - t) * (p0 - p1) + t * t * (p3 - p1);
    }

    //fills array with bezier curve points
    void FillArray(Vector3 x0,Vector3 x3,float height)
    {
        p0 = x0;
        p3 = x3;
        p1 = new Vector3((p0.x+3*p3.x)/4f, height, (p0.z + 3*p3.z) / 4f);

        for (float t = 0; t <= 1; t += 0.25f)
        {            
            pos.Add(GetBezierPosition(t));
        }
    }

    //starts moving free tiles
    public void StartMoveNewFreeTiles(Vector3 pos,bool isFirst,float r)
    {
        StartCoroutine(MoveNewFreeTiles(pos,isFirst,r));
    }

    //moves both free tiles
    IEnumerator MoveNewFreeTiles(Vector3 newPos,bool isFirst,float rotation)
    {

        if(!isFirst)
            yield return new WaitForSeconds(0.8f);
        MoveFreeTile(transform.position.x, thirdRowHeight, transform.position.z,rotation);
        yield return new WaitForSeconds(0.8f);

        MoveFreeTile(newPos.x, thirdRowHeight, newPos.z,rotation);
        yield return new WaitForSeconds(0.8f);

        
    }

    //moves one free tiles
    void MoveFreeTile(float x,float y,float z,float rotation)
    {
        endPoint = new Vector3(x,y,z);
        endRotation = Quaternion.Euler(0, rotation, 0);

        moving = true;
        rotating = true;
    }

    //"selects" tile after click
    public void SelectTile()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    //deselects tile
    public void DeselectTile()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
    }

    //opens tile at the end of the game
    public void ShowTile(float rotation)
    {
        transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        transform.rotation = Quaternion.Euler(-180, rotation, 0);
    }

    //moves and rotates tile
    private void FixedUpdate()
    {
        if (moving)
        {
            if (transform.position == endPoint)
            {
                num++;
                
                if (num >= pos.Count)
                {
                    moving = false;
                    pos = new List<Vector3>();
                    num = 1;

                    if (check)
                        check = false;

                    return;
                }
                endPoint = pos[num];
            }
            float step = speed * Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, endPoint, step);
        }
        if (rotating)
        {
            if (transform.rotation == endRotation)
            {
                rotating = false;
                return;
            }
           
            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, Time.deltaTime * speed);
        }
    }
}
