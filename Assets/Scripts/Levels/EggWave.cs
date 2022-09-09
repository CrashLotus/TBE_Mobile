using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/EggWave")]
public class EggWave : Wave
{
    public int m_numEgg1 = 0;
    public int m_numEgg2 = 0;
    public int m_numEgg3 = 0;
    public bool m_addTimeCrytal = false;
    public float m_duration = 0.0f;   //zero time means use the default calculation
    public bool m_waitOnEggs = true;
    public bool m_waitOnEnemies = true;

    protected const float s_eggSpacing = 9.0f;  // +/- this dist from player's pos
    protected const float s_eggPosY = 5.0f;
    protected const float s_eggSpeedY = -2.2f;

    protected int m_numEgg;
    protected List<int> m_eggList;
    protected int m_eggsLeft;
    protected float m_totalTime;
    protected float m_timePerEgg;
    protected float m_eggTimer;
    protected float m_spawnCenterX;     // the player's posision at the beginning of the wave marks the center of the spawn

    public override void Start()
    {
        Wave insertWave = this;
        if (m_waitOnEggs)
        {   // wait for the eggs to be done
            WaitOnWave wait = ScriptableObject.CreateInstance<WaitOnWave>();
            Level.Get().AddWave(wait, this);
            insertWave = wait;
        }
        if (m_waitOnEnemies)
        {   // wait for the enemies to be done
            EnemyWave enemy = ScriptableObject.CreateInstance<EnemyWave>();
            Level.Get().AddWave(enemy, insertWave);
        }

        m_numEgg = m_numEgg1 + m_numEgg2 + m_numEgg3;
        if (m_duration <= 0.0f)
            m_totalTime = 3.0f + 0.25f * m_numEgg;
        else
            m_totalTime = m_duration;
        if (m_text.Length == 0)
        {   // default text
            if (m_numEgg > 60)
                m_text = "Huevos Rancheros";
            else if (m_numEgg > 40)
                m_text = "Egg Tsunami";
            else if (m_numEgg > 20)
                m_text = "Grade A Extra Large Egg Wave";
            else if (m_numEgg > 13)
                m_text = "Big Egg Wave";
            else
                m_text = "Egg Wave";
        }
        m_eggsLeft = 0;
        m_timePerEgg = m_totalTime / m_numEgg;
        m_eggTimer = 0.0f;
        m_spawnCenterX = 0.0f;

        Player player = Player.Get();
        if (null != player)
            m_spawnCenterX = player.transform.position.x;

        m_eggsLeft = m_numEgg;

        m_eggList = new List<int>();
        for (int i = 0; i < m_numEgg1; ++i)
            m_eggList.Add(1);
        for (int i = 0; i < m_numEgg2; ++i)
            m_eggList.Add(2);
        for (int i = 0; i < m_numEgg3; ++i)
            m_eggList.Add(3);
        int swap;
        for (int i = m_eggList.Count - 1; i > 0; --i)
        {
            int index = Random.Range(0, i - 1);
            swap = m_eggList[i];
            m_eggList[i] = m_eggList[index];
            m_eggList[index] = swap;
        }

        if (m_addTimeCrytal)
        {
            m_eggList.Insert(0, -1);
            ++m_numEgg;
            ++m_eggsLeft;
        }

        m_timePerEgg = m_totalTime / m_numEgg;
        m_eggTimer = m_timePerEgg;

        base.Start();
    }

    public override void Update()
    {
        float dt = Time.deltaTime;
        m_eggTimer -= dt;
        while (m_eggTimer <= 0.0f && m_eggsLeft > 0)
        {
            Player player = Player.Get();
            if (null != player)
                m_spawnCenterX = player.transform.position.x;

            Vector2 spawnPos = new Vector2(this.m_spawnCenterX, s_eggPosY);
            spawnPos.x += Random.Range(-s_eggSpacing, s_eggSpacing);
            --m_eggsLeft;
            if (m_eggList[m_eggsLeft] > 0)
                Egg.Spawn(spawnPos, m_eggList[m_eggsLeft], s_eggSpeedY);
            else
                TimeCrystal.Spawn(spawnPos, s_eggSpeedY);
            m_eggTimer += m_timePerEgg;
        }
        base.Update();
    }

    public override bool IsDone()
    {
        if (m_isDone)
            return true;

        if (m_eggsLeft <= 0)
            return true;

        return false;
    }
}
