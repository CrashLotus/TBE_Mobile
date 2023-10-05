//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IHitPoints;
using static UnityEditor.PlayerSettings;

public class Ninja : EnemyBird
{
    public GameObject m_poopEffect;
    public Transform m_poopSpot;
    public float m_eggSpeed = 1.0f;

    static ObjectPool s_ninjaPool = null;

    enum State_Ninja
    {
        SHELL = State.CUSTOM,
        SPIN,
        WOBBLE
    };
    float m_shellTimer;
    float m_spinTimer;
    float m_eggTimer;
    Vector3 m_vel;
    Rigidbody2D m_rigidBody;

    const float s_shellTime = 3.0f;
    const float s_gravity = 4.0f;

    const float s_underLavaOffset = 1.5f;
    const float s_popUpSpeed = 6.0f;
    const float s_popUpAng = 45.0f;
    const float s_spinTime = 0.5f;
    const float s_wobbleTime = 2.0f;
    const float s_eggDelayMin = 2.0f;
    const float s_eggDelayMax = 4.0f;
    static readonly int[] s_numEggExplode = { 5, 3, 1 };
    static readonly float[] s_eggExplodeSpeed = { 3.0f, 2.0f, 0.0f };
    static readonly float[] s_eggExplodeUp = { 4.0f, 4.0f, 4.0f };

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
        m_pushFactor = 0.05f;
        m_shellTimer = 0.0f;
        m_spinTimer = 0.0f;
        NextEggTime();
        m_vel = Vector3.zero;
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime();
        Vector3 pos = transform.position;
        Vector3 oldPos = pos;
        Vector3 rot = transform.localEulerAngles;

        switch ((State_Ninja)m_state)
        {
            case State_Ninja.SHELL:
                {
                    m_fireDelay = 1.0f;
                    m_vel.y -= s_gravity * dt;
                    pos += m_vel * dt;

                    m_shellTimer -= dt;
                    if (m_shellTimer <= 0.0f)
                    {
                        m_state = State.FLYING;
                        if (null != m_anim)
                            m_anim.Play("Fly", 0, 0.0f);
                        NextEggTime();
                        if (null != m_rigidBody)
                            m_rigidBody.isKinematic = false;
                    }
                    Debug.Log(m_vel);
                    UpdatePos(pos, rot);
                    break;
                }
            case State_Ninja.SPIN:
                {
                    m_fireDelay = 1.0f;
                    m_spinTimer -= dt;
                    m_vel.y -= s_gravity * dt;
                    pos += m_vel * dt;
                    rot.z = 360.0f * m_spinTimer / s_spinTime;
                    if (m_spinTimer <= 0.0f)
                    {
                        m_spinTimer = s_wobbleTime;
                        m_state = (State)State_Ninja.WOBBLE;
                    }
                    UpdatePos(pos, rot);
                    break;
                }
            case State_Ninja.WOBBLE:
                {
                    m_fireDelay = 1.0f;
                    m_vel.y -= s_gravity * dt;
                    pos += m_vel * dt;
                    rot.z = Mathf.Rad2Deg * m_spinTimer * Mathf.Sin(Mathf.Deg2Rad * 360.0f * m_spinTimer);
                    m_spinTimer -= dt;
                    if (m_spinTimer <= 0.0f)
                    {
                        m_state = (State)State_Ninja.SHELL;
                    }
                    UpdatePos(pos, rot);
                    break;
                }
            default:
                {
                    if (Missile.GetNumMissiles() > 0)
                    {
                        m_state = (State)State_Ninja.SHELL;
                        m_shellTimer = s_shellTime;
                        if (null != m_anim)
                            m_anim.Play("Hide");
                        if (null != m_rigidBody)
                            m_rigidBody.isKinematic = true;
                    }
                    else
                    {
                        if (pos.y > GameManager.Get().GetLavaHeight())
                        {
                            m_eggTimer -= dt;
                            if (m_eggTimer <= 0.0f && GameManager.State.GAME_OVER != GameManager.Get().GetState())
                            {
                                Poop();
                                NextEggTime();
                            }
                        }
                    }
                    break;
                }
        }

        base.Update();

        if (State.CUSTOM > m_state)
            m_vel = (transform.position - oldPos) / dt;
    }

    void UpdatePos(Vector3 pos, Vector3 rot)
    {
        float bottomHeight = GameManager.Get().GetLavaHeight() - s_underLavaOffset;
        if (pos.y < bottomHeight)
        {
            pos.y = bottomHeight;
            m_vel.y = 0.0f;
        }
        transform.position = pos;
        transform.localEulerAngles = rot;
    }

    protected override IHitPoints.DamageReturn DoDamage(int damage, IHitPoints.HitType hitType)
    {
        if (hitType == HitType.MISSILE)
        {   // ninja is immune to missiles... he hides in his shell
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
            return DamageReturn.NO_DAMAGE;
        }
        if (hitType == HitType.NONE)
        {   // ninja is immune to collision damage
            return DamageReturn.NO_DAMAGE;
        }
        return base.DoDamage(damage, hitType);
    }

    protected override void Explode()
    {
        Vector3 pos = transform.position;
        for (int eggPower = 0; eggPower < 3; ++eggPower)
        {
            float angPerEgg = 2.0f * Mathf.PI / s_numEggExplode[eggPower];
            float eggAng = 0.0f;
            for (int i = 0; i < s_numEggExplode[eggPower]; ++i)
            {
                Vector3 eggDir = s_eggExplodeSpeed[eggPower] * new Vector3(Mathf.Sin(eggAng), Mathf.Cos(eggAng), 0.0f);
                eggDir.y += s_eggExplodeUp[eggPower];
                Egg.Spawn(pos, eggPower + 1, eggDir);
                eggAng += angPerEgg;
            }
        }
        FollowCamera.Shake(10.0f, 1.0f);
        TimeCrystal.Spawn(pos);
        m_eggPower = 0;
        base.Explode();
    }

    void PopUp(float power)
    {
        float ang = Mathf.Deg2Rad * Random.Range(-s_popUpAng, s_popUpAng);
        m_vel += new Vector3(Mathf.Sin(ang), Mathf.Cos(ang), 0.0f) * power;
    }

    void NextEggTime()
    {
        m_eggTimer = Random.Range(s_eggDelayMin, s_eggDelayMax);
    }

    public void Poop()
    {
        if (null != m_poopEffect && null != m_poopSpot)
        {
            Vector3 pos = m_poopSpot.transform.localPosition;
            Vector3 dir = m_poopSpot.right;
            if (m_sprite.flipX)
            {
                pos.x = -pos.x;
                dir.x = -dir.x;
            }
            pos = m_poopSpot.transform.parent.TransformPoint(pos);
            if (pos.y > GameManager.Get().GetLavaHeight())
            {
                if (GameManager.Get().GetScreenBounds().Contains(pos))
                {
                    GameObject poop = ObjectPool.Allocate(m_poopEffect, 16, pos);
                    if (null != poop)
                    {
                        poop.transform.rotation = m_poopSpot.rotation;
                        SpriteRenderer poopSprite = poop.GetComponent<SpriteRenderer>();
                        if (null != poopSprite)
                            poopSprite.flipX = m_sprite.flipX;
                    }
                    Egg.Spawn(pos, m_eggPower, -m_eggSpeed * dir);
                }
            }
        }
    }
}
