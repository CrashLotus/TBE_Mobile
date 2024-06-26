//----------------------------------------------------------------------------------------
//	Copyright � 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : PooledObject
{
    public float m_vertSpeed = 2.0f;
    public float m_horizSpeed = 2.0f;
    public int m_maxHitPoints = 1;
    public GameObject m_deathEffect;
    public Sound m_deathSound;

    protected const float s_pushTime = 0.15f;
    protected Vector3 m_push = Vector3.zero;
    protected float m_pushTimer = 0.0f;
    protected float m_pushFactor;
    protected float m_hitPoints;
    protected SpriteRenderer m_sprite;
    protected Animator m_anim;
    protected Vector3 m_oldPos;

    const float s_animSpeedMod = 5.4f;
    const float s_animSpeedMax = 120.0f;
    const float s_animSpeedMin = 0.0f;
    const float s_animSpeedBase = 15.0f;

    protected virtual void _WarmUp()
    {
        if (null != m_deathEffect)
            ObjectPool.GetPool(m_deathEffect, 64);
    }

    private void Start()
    {
        Init(null); //mrwTODO remove this when no more enemies are hand-placed
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_push = Vector3.zero;
        m_pushTimer = 0.0f;
        m_sprite = GetComponent<SpriteRenderer>();
        m_anim = GetComponent<Animator>();
        m_pushFactor = Mathf.Sqrt(m_horizSpeed / 100.0f);
        m_hitPoints = m_maxHitPoints;
        m_oldPos = transform.position;
        Utility.HitFlashReset(gameObject);
    }

    protected virtual void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime(this is Player);
        UpdatePush(dt);

        if (null != m_anim)
        {
            if (dt <= 0.0f)
            {
                m_anim.speed = 0.0f;
            }
            else if (m_anim.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
            {
                if (dt > 0.0f)
                {
                    Vector3 move = transform.position - m_oldPos;
                    float animSpeed = s_animSpeedBase + s_animSpeedMod * move.y / dt;
                    animSpeed = Mathf.Clamp(animSpeed, s_animSpeedMin, s_animSpeedMax);
                    m_anim.speed = animSpeed / 15.0f;
                }
            }
            else
            {
                m_anim.speed = 1.0f;
            }
        }

        m_oldPos = transform.position;
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

    void OnFireWeapon()
    {
        if (null != m_anim)
        {
            m_anim.Play("Fire", 0, 0.0f);
            m_anim.speed = 1.0f;
        }
    }
}
