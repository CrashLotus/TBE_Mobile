//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pow : PooledObject
{
    public float m_timeMin = 1.8f;
    public float m_timeMax = 2.2f;
    public float m_randOffset = 0.1f;
    public float m_scaleMin = 0.4f;
    public float m_scaleMax = 0.8f;
    public float m_randAng = 24.0f;

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        float scale = Random.Range(m_scaleMin, m_scaleMax);
        float rot = Random.Range(-m_randAng, m_randAng);
        Vector3 pos = transform.position;
        pos.x += Random.Range(-m_randOffset, m_randOffset);
        pos.y += Random.Range(-m_randOffset, m_randOffset);
        transform.position = pos;
        transform.localScale = scale * Vector3.one;
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, rot);
        StartCoroutine(CountDown(Random.Range(m_timeMin, m_timeMax)));
    }

    IEnumerator CountDown(float time)
    {
        yield return new WaitForSeconds(time);
        Free();
    }
}
