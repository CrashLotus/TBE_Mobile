using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : PooledObject, IHitPoints
{
    public Sound m_collectSound;
    public Sound m_spawnSound;

    protected enum State
    {
        IDLE,
        SPIN,
        WOBBLE,
        FLY_TO_HUD,
    }
    protected Vector3 m_vel = Vector3.zero;
    protected float m_pickUpTimer;
    protected State m_state = State.IDLE;
    protected float m_stateTimer = 0.0f;
    protected Vector3 m_startPos;
    protected Vector3 m_hudPos;
    protected float m_startRot;

    protected const float s_spawnSpeed = 1.8f;
    protected const float s_spawnAng = 45.0f;
    protected const float s_gravity = 1.7f;
    protected const float s_spinTime = 0.5f;
    protected const float s_wobbleTime = 2.0f;
    protected const float s_minPickUpDelay = 0.5f;

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_pickUpTimer = s_minPickUpDelay;
        GetComponent<Collider2D>().enabled = false;
        transform.localRotation = Quaternion.identity;
        m_vel = Vector3.zero;
        m_state = State.IDLE;
        m_stateTimer = 0.0f;
        SetState(State.IDLE);
        Utility.HitFlashReset(gameObject);
    }

    protected virtual void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime();

        if (State.FLY_TO_HUD == m_state)
        {
            UpdateFlyToHud(Time.unscaledDeltaTime);
        }
        else
        {
            if (m_pickUpTimer > 0.0f)
            {
                m_pickUpTimer -= Time.unscaledDeltaTime;
                if (m_pickUpTimer <= 0.0f)
                {
                    GetComponent<Collider2D>().enabled = true;
                    m_pickUpTimer = 0.0f;
                }
            }
            Vector3 pos = transform.position;
            m_vel.y -= s_gravity * dt;
            pos += m_vel * dt;

            if (pos.y < GameManager.Get().GetLavaHeight())
            {
                Free();
                HitLava();
            }
            else
            {
                switch (m_state)
                {
                    case State.SPIN:
                        m_stateTimer -= dt;
                        transform.localEulerAngles =
                            new Vector3(0.0f, 0.0f, 360.0f * m_stateTimer / s_spinTime);
                        if (m_stateTimer <= 0.0f)
                            SetState(State.WOBBLE);
                        break;
                    case State.WOBBLE:
                        transform.localEulerAngles =
                            new Vector3(0.0f, 0.0f, Mathf.Rad2Deg * m_stateTimer * Mathf.Sin(Mathf.Deg2Rad * 360.0f * m_stateTimer));
                        m_stateTimer -= dt;
                        if (m_stateTimer <= 0.0f)
                            SetState(State.IDLE);
                        break;
                }
            }

            transform.position = pos;
        }
    }

    protected void PopUp(float power)
    {
        float ang = Mathf.Deg2Rad * Random.Range(-s_spawnAng, s_spawnAng);
        m_vel = new Vector3(Mathf.Sin(ang), Mathf.Cos(ang), 0.0f) * power;
    }

    protected virtual void SetState(State newState)
    {
        switch (newState)
        {
            case State.SPIN:
                m_stateTimer = s_spinTime;
                break;
            case State.WOBBLE:
                m_stateTimer = s_wobbleTime;
                break;
            case State.FLY_TO_HUD:
                {
                    GetComponent<Collider2D>().enabled = false;
                    m_startPos = transform.position;
                    m_startRot = transform.localEulerAngles.z;
                }
                break;
        }

        m_state = newState;
    }

    protected virtual void HitLava()
    {
        // Does Nothing By Default
    }

    protected virtual void UpdateFlyToHud(float dt)
    {
        // Does Nothing By Default
    }

    public IHitPoints.DamageReturn Damage(float damage, IHitPoints.HitType hitType)
    {
        if (State.SPIN != m_state)
        {
            PopUp(s_spawnSpeed);
            SetState(State.SPIN);
            Utility.HitFlash(gameObject);
            return IHitPoints.DamageReturn.DAMAGED;
        }
        return IHitPoints.DamageReturn.PASS_THROUGH;
    }

    protected virtual void PickedUp(Player player)
    {
        if (null != m_collectSound)
            m_collectSound.Play();
        SetState(State.FLY_TO_HUD);
    }

    private void CheckPickup(Collision2D collision)
    {
        if (State.FLY_TO_HUD != m_state)
        {
            if (m_pickUpTimer <= 0.0f)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (null != player && player == Player.Get())
                {
                    PickedUp(player);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckPickup(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckPickup(collision);
    }
}
