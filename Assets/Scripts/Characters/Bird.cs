using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : PooledObject
{
    public float m_vertSpeed = 2.0f;
    public float m_horizSpeed = 2.0f;
    public float m_maxHitPoints = 1.0f;
    
    protected const float s_pushTime = 0.15f;
    protected Vector3 m_push = Vector3.zero;
    protected float m_pushTimer = 0.0f;
    protected float m_pushFactor;
    protected float m_hitPoints = 1.0f;
    protected int[] m_hitByType;
    protected IHitPoints.HitType m_lastHit;
    protected SpriteRenderer m_sprite;

    private void Start()
    {
        Init(null); //mrwTODO remove this when no more enemies are hand-placed
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_sprite = GetComponent<SpriteRenderer>();
        m_pushFactor = Mathf.Sqrt(m_horizSpeed / 100.0f);
        m_hitPoints = m_maxHitPoints;
        m_hitByType = new int[(int)IHitPoints.HitType.TOTAL];
        m_lastHit = IHitPoints.HitType.NONE;
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
        {
            m_pushTimer = 0.0f;
        }
        else
        {
            float lerp = m_pushTimer / s_pushTime;
            Vector3 pos = transform.position;
            pos += m_push * lerp * dt;
            transform.position = pos;
        }
    }
}
