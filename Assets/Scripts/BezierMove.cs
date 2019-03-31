using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMove : MonoBehaviour
{
    float thirdRowHeight = 3.4f;

    public Transform transformBegin;
    public Transform transformEnd;

    Vector3 p0, p1, p3;

    int num = 1;
    bool moving = false;
    float speed = 3f;

    List<Vector3> pos = new List<Vector3>();

    public void Move()
    {
       // FillArray();

        //moving from pos[0] to pos[1]

        transformEnd.position = pos[1];

        moving = true;
    }


    Vector3 GetBezierPosition(float t)
    {

        return p1 + (1 - t) * (1 - t) * (p0 - p1) + t * t * (p3 - p1);

        //return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }

    void FillArray(Vector3 x0,Vector3 x3)
    {
        p0 = x0;
        p3 = x3;
        p1 = new Vector3(1.5f, 2.5f, 1.5f);

        for (float t = 0; t <= 1; t += 0.02f)
        {
            pos.Add(GetBezierPosition(t));
        }
    }

    public void StartMoveNewFreeTiles(GameObject tile)
    {
        StartCoroutine(MoveNewFreeTiles(tile));
    }

    IEnumerator MoveNewFreeTiles(GameObject newTile)
    {
        MoveTile(transform.position.x, thirdRowHeight, transform.position.z);
        yield return new WaitForSeconds(0.8f);

        MoveTile(newTile.transform.position.x, thirdRowHeight, newTile.transform.position.z);
        yield return new WaitForSeconds(0.8f);

        
    }

    void MoveTile(float x,float y,float z)
    {
        transformEnd.position = new Vector3(x,y,z);
        moving = true;
    }

    //void Move()
    //{
    //    for (int i = 0; i < pos.Count - 1; i++)
    //    {
    //        MoveStep(pos[i]);
    //    }
    //}

    //void MoveStep(Vector3 end)
    //{
    //    while (transform.position != end)
    //    {
    //        float step = 1.0f * Time.deltaTime;

    //        // Move our position a step closer to the target.
    //        transform.position = Vector3.MoveTowards(transform.position, end, step);
    //    }
    //}

    

    private void Update()
    {
        if (!moving) return;

        if (transform.position == transformEnd.position)
        {
            num++;
            if (num >= pos.Count)
            {
                moving = false;
                pos.Clear();
                return;
            }
            transformEnd.position = pos[num];
        }
        float step = speed * Time.deltaTime;

        // Move our position a step closer to the target.
        transform.position = Vector3.MoveTowards(transform.position, transformEnd.position, step);

    }

}
