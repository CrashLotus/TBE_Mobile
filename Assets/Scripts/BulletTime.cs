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
    public const int s_timeWarpPoints = 200;


    public static BulletTime Get()
    {
        if (null == s_theManager)
        {
            GameObject timeObj = Resources.Load<GameObject>("BulletTime");
            if (null == timeObj)
            {
                Debug.LogError("Unable to load BulletTime prefab");
                return null;
            }
            GameObject newObj = Instantiate(timeObj);
            newObj.name = "BulletTime";
            DontDestroyOnLoad(newObj);
            s_theManager = newObj.GetComponent<BulletTime>();
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
        float invert = 1.0f;
        if (m_bulletTimeTimer <= 0.0f)
        {
            bulletTime = 1.0f;
            m_bulletTimeFactor = 1.0f;
            m_bulletTimeTimer = 0.0f;
            invert = 0.0f;
        }
        else if (m_bulletTimeTimer <= s_bulletTimeRamp)
        {
            float lerp = m_bulletTimeTimer / s_bulletTimeRamp;
            invert = lerp;
            bulletTime = Mathf.Lerp(1.0f, m_bulletTimeFactor, lerp);
            if (wasBulletTime)
            {
                if (null != m_timeWarpOver)
                    m_timeWarpOver.Play();
            }
        }
        Time.timeScale = bulletTime;
        Shader.SetGlobalFloat("_Invert", invert);
    }

    public void Begin()
    {
        int bulletTimeLevel = GetBulletTimeLevel();
        if (bulletTimeLevel >= 0)
        {
            m_bulletTimeFactor = s_timeWarpFactor[bulletTimeLevel];
            m_bulletTimeTimer = s_timeWarpTime[bulletTimeLevel];
            if (null != m_timeWarpBegin)
                m_timeWarpBegin.Play();
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

    public bool IsReady()
    {
        if (GetBulletTimeLevel() >= 0)
            return SaveData.Get().GetTimeWarp() >= s_timeWarpPoints;
        return false;
    }

    public int GetTimeBarValue()
    {
        if (GetBulletTimeLevel() >= 0)
            return SaveData.Get().GetTimeWarp();
        return 0;
    }

    public void AddPoints(int points)
    {
        SaveData data = SaveData.Get();
        int timePoints = data.GetTimeWarp();
        timePoints += points;
        if (timePoints > s_timeWarpPoints)
            timePoints = s_timeWarpPoints;
        data.SetTimeWarp(timePoints);
    }

    public void Empty()
    {
        SaveData data = SaveData.Get();
        data.SetTimeWarp(0);
    }
}
