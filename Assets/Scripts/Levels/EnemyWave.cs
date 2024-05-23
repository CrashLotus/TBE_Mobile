using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/EnemyWave")]
public class EnemyWave : Wave
{
    public int m_numEnemy1;
    public int m_numEnemy2;
    public int m_numEnemy3;
    public bool m_waitOnEnemies = true;

    class Spawn
    {
        public string m_spawnType;
        public Vector2 m_spawnPos;
        public float m_delay;

        public Spawn(string spawnType, Vector2 pos, float delay)
        {
            m_spawnType = spawnType;
            m_spawnPos = pos;
            m_delay = delay;
        }
    }

    List<Spawn> m_spawnList;
    int m_spawnIndex;
    float m_enemyTimer;
    float m_spawnCenterX;     // the player's posision at the beginning of the wave marks the center of the spawn

    public void AddSpawn(string spawnType, Vector2 pos, float delay)
    {
        m_spawnList.Add(new Spawn(spawnType, pos, delay));
    }

    public override void Start()
    {
        if (m_waitOnEnemies)
        {   // wait for the enemies to be done
            WaitOnWave wait = ScriptableObject.CreateInstance<WaitOnWave>();
            Level.Get().AddWave(wait, this);
        }

        m_spawnList = new List<Spawn>();
        m_spawnIndex = 0;
        m_enemyTimer = 0.0f;
        m_spawnCenterX = 0.0f;

        Player player = Player.Get();
        if (null != player)
            m_spawnCenterX = player.transform.position.x;

        m_spawnIndex = 0;

        float groundY = GameManager.Get().GetLavaHeight();
        List<string> enemyShuffle = new List<string>();
        for (int i = 0; i < m_numEnemy1; ++i)
            enemyShuffle.Add("Enemy_01");
        for (int i = 0; i < m_numEnemy2; ++i)
            enemyShuffle.Add("Enemy_02");
        for (int i = 0; i < m_numEnemy3; ++i)
            enemyShuffle.Add("Enemy_03");
        string swap;
        for (int i = enemyShuffle.Count - 1; i > 0; --i)
        {
            int index = Random.Range(0, i - 1);
            swap = enemyShuffle[i];
            enemyShuffle[i] = enemyShuffle[index];
            enemyShuffle[index] = swap;
        }
        foreach (string enemy in enemyShuffle)
        {
            Vector3 enemyPos = new Vector3(Random.Range(-6.7f, 6.7f), groundY, 0.0f);
            AddSpawn(enemy, enemyPos, 1.0f);
        }

        if (m_spawnList.Count > 0)
            m_enemyTimer = m_spawnList[0].m_delay;

        base.Start();
    }

    public override void Update()
    {
        float dt = Time.deltaTime;
        m_enemyTimer -= dt;
        while (m_spawnIndex < m_spawnList.Count && m_enemyTimer <= 0.0f)
        {
            Vector2 spawnPos = m_spawnList[m_spawnIndex].m_spawnPos;
            spawnPos.x += m_spawnCenterX;
#if false   //mrwTODO ???
            SpawnFactory.Spawn(m_spawnList[m_spawnIndex].m_spawnType, spawnPos);
#else
            string spawnType = m_spawnList[m_spawnIndex].m_spawnType;
            if ("Enemy_01" == spawnType)
                EnemyBird.Spawn(spawnPos, 1, -1);
            else if ("Enemy_02" == spawnType)
                EnemyBird.Spawn(spawnPos, 2, -1);
            else if ("Enemy_03" == spawnType)
                EnemyBird.Spawn(spawnPos, 3, -1);
            else
                Debug.LogWarning("Unknown Spawn Type: " + spawnType);
#endif
            ++m_spawnIndex;
            if (m_spawnIndex < m_spawnList.Count)
                m_enemyTimer += m_spawnList[m_spawnIndex].m_delay;
        }
        base.Update();
    }

    public override bool IsDone()
    {
        if (m_isDone)
            return true;

        if (m_spawnIndex < m_spawnList.Count)
            return false;

        if (EnemyBird.GetCount() > 0 || Egg.GetCount() > 0)
            return false;

        return true;
    }
}
