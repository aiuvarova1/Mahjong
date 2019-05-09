﻿using System.Collections;
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

    public void OpenTile(float rotation)
    {
        endRotation= Quaternion.Euler(-90, rotation, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);
       
        rotating = true;
        
    }

    public void Move(Vector3 end,float rotation )
    {

        FillArray(transform.position,end,8.4f);

        //moving from pos[0] to pos[1]
        endPoint = pos[1];
        endRotation = Quaternion.Euler(0, rotation, 0);
        
        StartCoroutine(WaitForMoving(rotation));
    }

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

    public void LieOut(Vector3 endPos,float rotation,bool closed)
    {

        if (closed)
        {
            Debug.Log("closed");
            //transform.position = new Vector3(transform.position.x, 2.8f, transform.position.z);
            endPos.y = 1.8f;
            endRotation = Quaternion.Euler(0, rotation, 0);
        }
        else
        {
            Debug.Log("opened");
            //transform.position = new Vector3(transform.position.x, 1, transform.position.z);
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

    public void StartMoveNewFreeTiles(Vector3 pos,bool isFirst,float r)
    {
        StartCoroutine(MoveNewFreeTiles(pos,isFirst,r));
    }

    IEnumerator MoveNewFreeTiles(Vector3 newPos,bool isFirst,float rotation)
    {
        Debug.Log(rotating);
        //speed = 40f;
        if(!isFirst)
            yield return new WaitForSeconds(0.8f);
        MoveFreeTile(transform.position.x, thirdRowHeight, transform.position.z,rotation);
        yield return new WaitForSeconds(0.8f);

        MoveFreeTile(newPos.x, thirdRowHeight, newPos.z,rotation);
        yield return new WaitForSeconds(0.8f);

        
    }

    void MoveFreeTile(float x,float y,float z,float rotation)
    {
        endPoint = new Vector3(x,y,z);
        endRotation = Quaternion.Euler(0, rotation, 0);

        moving = true;
        rotating = true;

    }

    public void SelectTile()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    public void DeselectTile()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
    }

    public void ShowTile(float rotation)
    {
        Debug.Log(rotation);
        Debug.Log(transform.rotation);
        transform.rotation=Quaternion.Euler(-180, rotation, 0);
        Debug.Log(transform.rotation);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }


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

            //transform.position = Vector3.Lerp(transform.position, endPoint, 0.5f*Time.deltaTime);
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
