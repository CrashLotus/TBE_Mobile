using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnim : PooledObject
{
    public bool m_randomRotate = true;
    public float m_minSpeed = 0.8f;
    public float m_maxSpeed = 1.2f;
    public float m_minScale = 0.8f;
    public float m_maxScale = 1.2f;
    public Sound m_sound;

    Animator m_anim;

    IEnumerator DeleteWhenDone()
    {
        if (null != m_anim)
        {
            var state = m_anim.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length / m_anim.speed);
        }
        Free();
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        SetUp();
        if (null != m_anim)
            m_anim.Play(0, -1, 0.0f);
        if (null != m_sound)
            m_sound.Play();
    }

    void SetUp()
    {
        m_anim = GetComponent<Animator>();
        if (null != m_anim)
        {
            m_anim.speed = Random.Range(m_minSpeed, m_maxSpeed);
        }
        transform.localScale = Random.Range(m_minScale, m_maxScale) * Vector3.one;
        if (m_randomRotate)
        {
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        }
        StartCoroutine(DeleteWhenDone());
    }
}
