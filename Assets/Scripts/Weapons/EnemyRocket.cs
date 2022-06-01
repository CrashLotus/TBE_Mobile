using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocket : Bullet, IHitPoints
{
    public float m_timeOut = 2.0f;
    public float m_turnSpd = 0.5f;
    public float m_maxHitPoints = 2.0f;
    public Sound m_destroyedSound;

    float m_timer;
    float m_hitPoints;

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_hitPoints = m_maxHitPoints;
    }

    public IHitPoints.DamageReturn Damage(float damage, IHitPoints.HitType hitType)
    {
        if (m_hitPoints > 0.0f)
        {
            m_hitPoints -= damage;
            if (m_hitPoints <= 0.0f)
            {
                if (null != m_destroyedSound)
                    m_destroyedSound.Play();
                Explode();
                return IHitPoints.DamageReturn.KILLED;    // I've been killed
            }
            return IHitPoints.DamageReturn.DAMAGED;
        }

        return IHitPoints.DamageReturn.PASS_THROUGH;       // I'm already dead    }
    }

    public override void Fire(Vector3 dir)
    {
        base.Fire(dir);
        m_timer = m_timeOut;
    }

    protected override void Update()
    {
        float dt = Time.deltaTime;
        m_timer -= dt;
        if (m_timer <= 0.0f)
        {
            if (null != m_destroyedSound)
                m_destroyedSound.Play();
            Explode();
            return;
        }
        Player player = Player.Get();
        if (null != player)
        {
            Vector2 toPlayer = player.transform.position - transform.position;
            float ang = Mathf.Atan2(toPlayer.y, toPlayer.x);
            float rot = transform.localEulerAngles.z * Mathf.Deg2Rad;
            ang = ang - rot;
            if (ang < -Mathf.PI)
                ang += 2.0f * Mathf.PI;
            else if (ang > Mathf.PI)
                ang -= 2.0f * Mathf.PI;
            ang = Mathf.Clamp(ang, -m_turnSpd * dt, m_turnSpd * dt);
            rot += ang;
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Rad2Deg * rot);
            m_vel = m_speed * new Vector3(Mathf.Cos(rot), Mathf.Sin(rot), 0.0f);
        }

        base.Update();
    }
}
