using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float m_vertSpeed = 2.0f;
    public float m_horizSpeed = 2.0f;

    protected const float s_pushTime = 0.15f;
    protected Vector3 m_push = Vector3.zero;
    protected float m_pushTimer = 0.0f;
    protected float m_pushFactor;
    protected SpriteRenderer m_sprite;

    protected virtual void Start()
    {
        m_sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        float dt = Time.deltaTime;
        UpdatePush(dt);
    }

    public virtual void Push(Vector3 force)
    {
        m_push = force * m_pushFactor;
        m_pushTimer = s_pushTime;
    }

    protected void UpdatePush(float dt)
    {
        // update push
        m_pushTimer -= dt;
        if (m_pushTimer <= 0.0f)
            m_pushTimer = 0.0f;
        else
        {
            float lerp = m_pushTimer / s_pushTime;
            Vector3 pos = transform.position;
            pos += m_push * lerp * dt;
            transform.position = pos;
        }
    }
}
