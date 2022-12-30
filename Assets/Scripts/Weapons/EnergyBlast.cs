using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBlast : Bullet
{
    public float m_accel = 1.1f;
    public float m_spinRate = -10.0f * Mathf.Rad2Deg;

    float m_rot = 0.0f;

    protected override void Update()
    {
        float dt = Time.deltaTime;
        m_curSpeed += m_accel * dt;
        m_vel = m_dir * m_curSpeed;
        m_rot = m_rot + m_spinRate * dt;
        if (m_rot < 0.0f)
            m_rot += 360.0f;
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, m_rot);
        base.Update();
    }
}
