using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/PauseWave")]
public class PauseWave : Wave
{
    public float m_pauseTime = 1.0f;
    float m_timer;

    public override void Start()
    {
        m_timer = m_pauseTime;
        base.Start();
    }

    public override void Update()
    {
        if (m_timer > 0.0f)
            m_timer -= BulletTime.Get().GetDeltaTime();
        base.Update();
    }

    public override bool IsDone()
    {
        if (m_timer <= 0.0f)
            return true;
        return base.IsDone();
    }

    public override bool IsWait()
    {
        return true;
    }
}
