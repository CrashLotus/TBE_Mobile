using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour
{
    static BulletTime s_theManager;

    public Sound m_timeWarpOver;
    public Sound m_timeWarpBegin;

    float m_bulletTimeTimer = 0.0f;
    float m_bulletTimeFactor = 1.0f;

    const float s_bulletTimeRamp = 0.5f;
    static float[] s_timeWarpFactor = { 0.1f, 0.05f };
    static float[] s_timeWarpTime = { 3.0f, 5.0f };


    public static BulletTime Get()
    {
        if (null == s_theManager)
        {   // just add this as a component to the existing GameManager object
            GameManager gm = GameManager.Get();
            s_theManager = gm.gameObject.AddComponent<BulletTime>();
            s_theManager.Initialize();
        }
        return s_theManager;
    }

    void Initialize()
    {
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        float bulletTime = m_bulletTimeFactor;
        bool wasBulletTime = m_bulletTimeTimer > s_bulletTimeRamp;
        m_bulletTimeTimer -= dt;
        if (m_bulletTimeTimer <= 0.0f)
        {
            bulletTime = 1.0f;
            m_bulletTimeFactor = 1.0f;
            m_bulletTimeTimer = 0.0f;
        }
        else if (m_bulletTimeTimer <= s_bulletTimeRamp)
        {
            float lerp = m_bulletTimeTimer / s_bulletTimeRamp;
            bulletTime = Mathf.Lerp(1.0f, m_bulletTimeFactor, lerp);
            if (wasBulletTime)
            {
                if (null != m_timeWarpOver)
                    m_timeWarpOver.Play();
            }
        }
        Time.timeScale = bulletTime;
    }

    public static void Begin()
    {
        BulletTime bt = Get();
        int bulletTimeLevel = GetBulletTimeLevel();
        if (bulletTimeLevel >= 0)
        {
            bt.m_bulletTimeFactor = s_timeWarpFactor[bulletTimeLevel];
            bt.m_bulletTimeTimer = s_timeWarpTime[bulletTimeLevel];
            if (null != bt.m_timeWarpBegin)
                bt.m_timeWarpBegin.Play();
        }
    }

    public static int GetBulletTimeLevel()
    {
        SaveData data = SaveData.Get();
        if (data.HasUpgrade("BULLETTIME"))
        {
            if (data.HasUpgrade("BULLETTIME2"))
                return 1;
            return 0;
        }
        return -1;
    }
}
