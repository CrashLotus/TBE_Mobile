//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/MeteorWave")]
public class MeteorWave : Wave
{
    public int m_numSpawn = 0;
    public float m_duration = 0.0f;   //zero time means use the default calculation
    public bool m_waitOnMeteors = true;

    int m_spawnLeft;
    float m_totalTime;
    float m_timePerSpawn;
    float m_spawnTimer;

    const float s_spawnPosY = 4.5f;

    public override void Start()
    {
        Wave insertWave = this;
        if (m_waitOnMeteors)
        {   // wait for the eggs to be done
            WaitOnWave wait = ScriptableObject.CreateInstance<WaitOnWave>();
            Level.Get().AddWave(wait, this);
            insertWave = wait;
        }

        if (m_duration <= 0.0f)
            m_totalTime = 2.0f + 0.4f * m_numSpawn;
        else
            m_totalTime = m_duration;

        m_spawnLeft = m_numSpawn;
        m_timePerSpawn = m_totalTime / m_numSpawn;
        m_spawnTimer = m_timePerSpawn;

        if (m_text.Length == 0)
        {   // default text
            if (m_numSpawn > 70)
                m_text ="The Sky is Falling!";
            else if (m_numSpawn > 60)
                m_text = "Major Meteor Storm";
            else if (m_numSpawn > 45)
                m_text = "Big Meteor Storm";
            else
                m_text = "Meteor Storm";
        }

        base.Start();
    }

    public override void Update()
    {
        float dt = Time.deltaTime;
        m_spawnTimer -= dt;
        while (m_spawnTimer <= 0.0f && m_spawnLeft > 0)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-GameManager.Get().m_worldWidth, GameManager.Get().m_worldWidth),
                s_spawnPosY,
                0.0f
                ); ;
            Meteor.Spawn(spawnPos);
            --m_spawnLeft;
            m_spawnTimer += m_timePerSpawn;
        }
        base.Update();
    }

    public override bool IsDone()
    {
        if (m_isDone)
            return true;

        if (m_spawnLeft <= 0)
        {
            if (Meteor.GetCount() <= 0)
                return true;
        }

        return false;
    }
}
