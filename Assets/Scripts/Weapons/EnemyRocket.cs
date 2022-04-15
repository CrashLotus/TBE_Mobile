using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocket : Bullet
{
    public float m_timeOut = 2.0f;
    public float m_turnSpd = 0.5f;
    float m_timer;

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
