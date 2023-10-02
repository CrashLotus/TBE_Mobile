using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Bullet
{
    const float s_timeOut = 1.0f;
    const float s_launchSpeed = 5.6f;
    const float s_finalSpeed = 2.2f;

    GameObject m_target;
    Bezier m_bezier;
    Vector3 m_oldPos;
    Vector3 m_finalDir;
    Vector3 m_targetPos;
    float m_timer;

    static int s_numMissile = 0;

    public void Launch(Vector3 pos, Vector3 dir, Vector3 finalDir, GameObject target)
    {
        Fire(dir);
        m_finalDir = finalDir;
        m_timer = s_timeOut;
        m_vel = dir * s_launchSpeed;
        m_target = target;
        m_bezier = new Bezier();
        m_bezier.SetStart(pos, m_vel);
        CalcBezier();
        m_oldPos = pos;
        ++s_numMissile;
    }

    protected override void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime(true);
        m_timer -= dt;
        if (m_timer <= 0.0f)
        {
            if (null != m_target && m_target.activeSelf)
            {
                Hit(m_target);
            }
            --s_numMissile;
            Free();
        }
        else
        {
            CalcBezier();
            Vector3 pos = transform.position;
            Vector3 dir = pos - m_oldPos;
            transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
            m_oldPos = pos;
        }
    }

    void CalcBezier()
    {
        if (null != m_target && m_target.activeSelf)
            m_targetPos = m_target.transform.position;
        Vector3 dir = m_targetPos - transform.position;
        dir = Vector3.Normalize(m_finalDir) * s_finalSpeed;
        m_bezier.SetEnd(m_targetPos, dir);
        float time = 1.0f - m_timer / s_timeOut;
        transform.position = m_bezier.Evaluate(time);
    }

    public static int GetNumMissiles()
    {
        return s_numMissile;
    }
}
