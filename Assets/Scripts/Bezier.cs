using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    public Vector3 a, b, c, d;

    public void SetStart(Vector3 start, Vector3 startVel)
    {
        a = start;
        b = start + startVel;
    }

    public void SetEnd(Vector3 end, Vector3 endVel)
    {
        d = end;
        c = end - endVel;
    }

    public Vector3 Evaluate(float t)
    {
        float t1 = (1.0f - t);
        float t12 = t1 * t1;
        float t13 = t12 * t1;
        float t2 = t * t;
        float t3 = t2 * t;
        Vector3 pos = t13 * a
            + 3.0f * t12 * t * b
            + 3.0f * t1 * t2 * c
            + t3 * d;
        return pos;
    }
}
