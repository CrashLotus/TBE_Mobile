using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public List<Wave> m_waves;

    int m_currentWave;
    bool m_isDone;
    bool m_isActive;
    protected bool m_doTransition;

    static Level s_currentLevel = null;

    public static Level Get()
    {
        return s_currentLevel;
    }

    public Wave FindWave(string label)
    {
        foreach (Wave wave in m_waves)
        {
            if (wave.GetLabel() == label)
                return wave;
        }
        return null;
    }

    public void Start()
    {
        s_currentLevel = this;

        m_currentWave = -1;
        m_isDone = false;
        m_isActive = false;
        m_doTransition = false;

        for (int i = 0; i < m_waves.Count; ++i)
        {
            m_waves[i] = Instantiate(m_waves[i]);
            m_waves[i].Reset();
        }
        if (m_waves.Count > 0)
        {
            m_currentWave = -1;
            StartNextWave();
        }
    }

    public bool IsDone()
    {
        return m_isDone;
    }

    public bool IsActive()
    {
        return m_isActive;
    }

    public bool DoTransition()
    {
        return m_doTransition;
    }

    public bool GoTo(string label)
    {
        for (int i = 0; i < m_waves.Count; ++i)
        {
            if (label == m_waves[i].GetLabel())
            {
                m_currentWave = i - 1;
                StartNextWave();
                return true;
            }
        }
        return false;
    }

    public void Update()
    {
        // update each active wave
        m_isActive = false;
        for (int i = 0; i <= m_currentWave; ++i)
        {
            if (i < m_waves.Count && m_waves[i].IsActive())
            {
                m_waves[i].Update();
                if (m_waves[i].IsDone())
                {
                    m_waves[i].Stop();
                    if (i == m_currentWave)
                        StartNextWave();
                }
                else
                    m_isActive = true;
            }
        }

        if (m_currentWave >= m_waves.Count)
            m_isDone = !m_isActive;
    }

    void StartNextWave()
    {
        m_currentWave++;
        while (m_currentWave < m_waves.Count)
        {
            m_isActive = true;
            m_waves[m_currentWave].Start();
            if (m_waves[m_currentWave].IsDone())
                m_waves[m_currentWave].Stop();
            else if (m_waves[m_currentWave].IsWait())
                return;
            ++m_currentWave;
        }
    }

    public void AddWave(Wave newWave, Wave afterWave)
    {
        int insertAt = m_waves.Count;
        if (null != afterWave)
            insertAt = m_waves.IndexOf(afterWave) + 1;
        m_waves.Insert(insertAt, newWave);
    }

#if false   //mrwTODO
    protected Wave DoEnemyWave(int numEnemy1, int numEnemy2, int numEnemy3)
    {
        DoEnemyWave_NoWait(numEnemy1, numEnemy2, numEnemy3);
        Wave wave = new WaitOnWave(this, null, null);
        m_waves.Add(wave);      // wait for enemies to be cleared
        return wave;
    }

    protected void DoEnemyWave_Meteors(int numEnemy1, int numEnemy2, int numEnemy3, int numMeteor)
    {
        DoEnemyWave_NoWait(numEnemy1, numEnemy2, numEnemy3);
        m_waves.Add(new PauseWave(this, null, 1.0f));       // pause a moment
        // add some meteors
        m_waves.Add(new MeteorWave(this, null, numMeteor, MeteorWave.DefaultTime(numMeteor)));
        m_waves.Add(new WaitOnWave(this, null, null));      // wait for enemies to be cleared
    }

    protected void DoEggWave_Enemies(int numEgg1, int numEgg2, int numEgg3, int numEnemy1, int numEnemy2, int numEnemy3)
    {
        DoEggWave_NoWait(numEgg1, numEgg2, numEgg3);
        m_waves.Add(new PauseWave(this, null, 1.0f));       // pause a moment
        DoEnemyWave(numEnemy1, numEnemy2, numEnemy3);
    }

    protected void DoEggWave_EnemiesMeteors(int numEgg1, int numEgg2, int numEgg3, int numEnemy1, int numEnemy2, int numEnemy3, int numMeteor)
    {
        DoEggWave_NoWait(numEgg1, numEgg2, numEgg3);
        m_waves.Add(new PauseWave(this, null, 1.0f));       // pause a moment
        DoEnemyWave_NoWait(numEnemy1, numEnemy2, numEnemy3);
        m_waves.Add(new PauseWave(this, null, 1.0f));       // pause a moment
        m_waves.Add(new MeteorWave(this, null, numMeteor, MeteorWave.DefaultTime(numMeteor)));
        m_waves.Add(new WaitOnWave(this, null, null));      // wait for enemies to be cleared
    }

    protected void DoEnemyWave_NoWait(int numEnemy1, int numEnemy2, int numEnemy3, string label = null)
    {
        float groundY = Game1.GetLavaHeight();
        EnemyWave enemyWave = new EnemyWave(this, label);
        List<string> enemyShuffle = new List<string>();
        for (int i = 0; i < numEnemy1; ++i)
            enemyShuffle.Add("Enemy_01");
        for (int i = 0; i < numEnemy2; ++i)
            enemyShuffle.Add("Enemy_02");
        for (int i = 0; i < numEnemy3; ++i)
            enemyShuffle.Add("Enemy_03");
        string swap;
        for (int i = enemyShuffle.Count - 1; i > 0; --i)
        {
            int index = RandomHelper.Range(0, i - 1);
            swap = enemyShuffle[i];
            enemyShuffle[i] = enemyShuffle[index];
            enemyShuffle[index] = swap;
        }
        foreach (string enemy in enemyShuffle)
        {
            Vector2 enemyPos = new Vector2(RandomHelper.Range(-600.0f, 600.0f), groundY);
            enemyWave.AddSpawn("Flame", enemyPos, 1.0f);
            enemyWave.AddSpawn(enemy, enemyPos, 0.0f);
        }
        m_waves.Add(enemyWave);
    }

    protected Wave DoEggWave(int numEgg1, int numEgg2, int numEgg3)
    {
        Wave eggWave = DoEggWave_NoWait(numEgg1, numEgg2, numEgg3);
        m_waves.Add(new WaitOnWave(this, null, null));  // wait for the eggs to spawn
        m_waves.Add(new EnemyWave(this, null));         // the empty enemy wave will cause us to wait for the eggs (and their enemies)
        m_waves.Add(new WaitOnWave(this, null, null));
        return eggWave;
    }

    protected Wave DoEggWave_Meteors(int numEgg1, int numEgg2, int numEgg3, int numMeteor)
    {
        Wave eggWave = DoEggWave_NoWait(numEgg1, numEgg2, numEgg3);
        m_waves.Add(new PauseWave(this, null, 1.0f));       // pause a moment
        // add some meteors
        m_waves.Add(new MeteorWave(this, null, numMeteor, MeteorWave.DefaultTime(numMeteor)));
        m_waves.Add(new WaitOnWave(this, null, null));  // wait for the eggs to spawn
        m_waves.Add(new EnemyWave(this, null));         // the empty enemy wave will cause us to wait for the eggs (and their enemies)
        m_waves.Add(new WaitOnWave(this, null, null));
        return eggWave;
    }

    protected Wave DoEggWave_NoWait(int numEgg1, int numEgg2, int numEgg3, string label = null)
    {
        int numEgg = numEgg1 + numEgg2 + numEgg3;
        Wave eggWave = new EggWave(this, label, numEgg1, numEgg2, numEgg3, 3.0f + 0.25f * numEgg);
        if (numEgg > 60)
            eggWave.SetText("Huevos Rancheros");
        else if (numEgg > 40)
            eggWave.SetText("Egg Tsunami");
        else if (numEgg > 20)
            eggWave.SetText("Grade A Extra Large Egg Wave");
        else if (numEgg > 13)
            eggWave.SetText("Big Egg Wave");
        else
            eggWave.SetText("Egg Wave");
        m_waves.Add(eggWave);
        return eggWave;
    }

    protected void DoMeteorWave(int numMeteor)
    {
        DoMeteorWave_NoWait(numMeteor);
        m_waves.Add(new WaitOnWave(this, null, null));
        m_waves.Add(new PauseWave(this, null, 3.0f));
    }

    protected void DoMeteorWave_NoWait(int numMeteor, string label = null)
    {
        float power = (numMeteor - 6) / 2.5f;
        float time = 5.0f + 0.4f * power;
        Wave meteorWave = new MeteorWave(this, label, numMeteor, time);
        if (numMeteor > 70)
            meteorWave.SetText("The Sky is Falling!");
        else if (numMeteor > 60)
            meteorWave.SetText("Major Meteor Storm");
        else if (numMeteor > 45)
            meteorWave.SetText("Big Meteor Storm");
        else
            meteorWave.SetText("Meteor Storm");
        m_waves.Add(meteorWave);
    }
#endif
}
