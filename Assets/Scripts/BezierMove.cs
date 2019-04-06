using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMove : MonoBehaviour
{
    float thirdRowHeight = 3.4f;

   // public Transform transformBegin;
    public Vector3 endPoint;
    Quaternion endRotation;

    Vector3 p0, p1, p3;

    int num = 1;

    public bool moving = false;
    public bool rotating = false;


    float speed = 20f;

    List<Vector3> pos = new List<Vector3>();

    public void OpenTile(float rotation)
    {
        endRotation= Quaternion.Euler(-90, rotation, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        rotating = true;
        
    }

    public void Move(Vector3 end,float rotation )
    {
        speed = 50f;
        FillArray(transform.position,end,8.4f);

        //moving from pos[0] to pos[1]

        endPoint = pos[1];
        endRotation = Quaternion.Euler(0, rotation, 0);
        //Wall.instance.isMoving = true;
        StartCoroutine(WaitForMoving(rotation));
        

    }

    IEnumerator WaitForMoving(float rotation)
    {
        //yield return new WaitForSeconds(0.1f);
        moving = true;
        rotating = true;
        while (moving)
            yield return new WaitForSeconds(0.1f);
        OpenTile(rotation);
    }

    public void LieOut(Vector3 endPos,float rotation)
    {

        transform.position = new Vector3(transform.position.x, 1, transform.position.z);

        FillArray(transform.position, endPos,2.4f);


        endPoint = pos[1];
        endRotation = Quaternion.Euler(-180, rotation, 0);

        speed = 38f;
        
        moving = true;
        rotating = true;

    }


    Vector3 GetBezierPosition(float t)
    {

        return p1 + (1 - t) * (1 - t) * (p0 - p1) + t * t * (p3 - p1);

        //return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }

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

    public void StartMoveNewFreeTiles(GameObject tile,bool isFirst)
    {
        StartCoroutine(MoveNewFreeTiles(tile,isFirst));
    }

    IEnumerator MoveNewFreeTiles(GameObject newTile,bool isFirst)
    {
        speed = 12f;
        if(!isFirst)
            yield return new WaitForSeconds(0.8f);
        MoveTile(transform.position.x, thirdRowHeight, transform.position.z);
        yield return new WaitForSeconds(0.8f);

        MoveTile(newTile.transform.position.x, thirdRowHeight, newTile.transform.position.z);
        yield return new WaitForSeconds(0.8f);

        
    }

    void MoveTile(float x,float y,float z)
    {
        endPoint = new Vector3(x,y,z);
        moving = true;
    }

    public void SelectTile()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    public void DeselectTile()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
    }


    private void Update()
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
            //Vector3 direction = (endPoint - transform.position).normalized;

            //create the rotation we need to be in to look at the target
            //Quaternion lookRotation = Quaternion.LookRotation(direction);

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, Time.deltaTime * speed);
            
        }



    }

}
