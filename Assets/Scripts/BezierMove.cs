using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMove : MonoBehaviour
{

    public Transform transformBegin;
    public Transform transformEnd;

    int num = 1;
    bool moving = false;

    List<Vector3> pos = new List<Vector3>();

    Vector3 GetBezierPosition(float t)
    {
        Vector3 p0 = transformBegin.position;
        Vector3 p1 = p0 + transformBegin.forward;

        Vector3 p3 = transformEnd.position;
        Vector3 p2 = p3 - transformEnd.forward * (-1);
        p1 = new Vector3(1.5f, 2.5f, 1.5f);
        // here is where the magic happens!
        return p1 + (1 - t) * (1 - t) * (p0 - p1) + t * t * (p3 - p1);
        //return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }

    void FillArray()
    {
        for (float t = 0; t <= 1; t += 0.02f)
        {
            pos.Add(GetBezierPosition(t));
        }
    }

    void Move()
    {
        for (int i = 0; i < pos.Count - 1; i++)
        {
            MoveStep(pos[i]);
        }
    }

    void MoveStep(Vector3 end)
    {
        while (transform.position != end)
        {
            float step = 1.0f * Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, end, step);
        }
    }

    private void Start()
    {
        FillArray();
        transformEnd.position = pos[1];
        moving = true;
        //Move();
    }

    private void Update()
    {
        if (!moving) return;
        if (transform.position == transformEnd.position)
        {
            num++;
            if (num >= pos.Count)
            {
                moving = false;
                return;
            }
            transformEnd.position = pos[num];
        }
        float step = 1.0f * Time.deltaTime;

        // Move our position a step closer to the target.
        transform.position = Vector3.MoveTowards(transform.position, transformEnd.position, step);

    }

}
