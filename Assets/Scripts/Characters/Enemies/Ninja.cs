//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IHitPoints;

public class Ninja : EnemyBird
{
    static ObjectPool s_ninjaPool = null;

    enum State_Ninja
    {
        SHELL = State.CUSTOM,
        SPIN,
        WOBBLE
    };
    float m_shellTimer;
    float m_damageTimer;
    float m_spinTimer;
    float m_eggTimer;
    Vector3 m_vel;

    const float s_underLavaOffset = 0.0f;
    const float s_popUpSpeed = 2.9f;
    const float s_popUpAng = 45.0f;
    const float s_spinTime = 0.5f;
    const float s_wobbleTime = 2.0f;
    const float s_eggDelayMin = 1.0f;
    const float s_eggDelayMax = 2.5f;

    public static new void WarmUp()
    {
        if (null == s_ninjaPool)
        {
            MakePool();
            if (null != s_ninjaPool)
            {
                Ninja bird = s_ninjaPool.m_prefab.GetComponent<Ninja>();
                bird._WarmUp();
            }
        }
    }

    public static Ninja Spawn(Vector3 pos)
    {
        MakePool();
        if (null != s_ninjaPool)
        {
            GameObject enemyObj = s_ninjaPool.Allocate(pos);
            Ninja ninja = enemyObj.GetComponent<Ninja>();
            return ninja;
        }
        return null;
    }

    public static void MakePool()
    {
        if (null == s_ninjaPool)
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("Ninja");
            if (null == enemyPrefab)
            {
                Debug.LogError("Unable to load enemy prefab Ninja");
                return;
            }
            s_ninjaPool = ObjectPool.GetPool(enemyPrefab, 4);
        }
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_shellTimer = 0.0f;
        m_damageTimer = 0.0f;
        m_spinTimer = 0.0f;
        NextEggTime();
        m_vel = Vector3.zero;
    }

    protected override IHitPoints.DamageReturn DoDamage(int damage, IHitPoints.HitType hitType)
    {
        if (hitType == IHitPoints.HitType.MISSILE)
        {
            switch ((State_Ninja)m_state)
            {
                case State_Ninja.SHELL:
                case State_Ninja.WOBBLE:
                    PopUp(s_popUpSpeed);
                    m_state = (State)State_Ninja.SPIN;
                    m_spinTimer = s_spinTime;
                    break;
                case State_Ninja.SPIN:
                    break;
            }
            return IHitPoints.DamageReturn.NO_DAMAGE;
        }
        return base.DoDamage(damage, hitType);
    }

    void PopUp(float power)
    {
        float ang = Mathf.Deg2Rad * Random.Range(-s_popUpAng, s_popUpAng);
        m_vel = new Vector2(Mathf.Sin(ang), -Mathf.Cos(ang)) * power;
    }

    void NextEggTime()
    {
        m_eggTimer = Random.Range(s_eggDelayMin, s_eggDelayMax);
    }
}
