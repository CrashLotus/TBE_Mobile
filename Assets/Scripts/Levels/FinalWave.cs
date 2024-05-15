using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/FinalWave")]
public class FinalWave : Wave
{
    bool firstWave = true;

    enum State
    {
        EGGS,
        BOSS,
    }
    State m_state;

    // egg stuff
    int m_numEgg1 = 0;
    int m_numEgg2 = 0;
    int m_numEgg3 = 0;
    protected int m_numEgg;
    List<int> m_eggList;
    int m_eggsLeft = 0;
    float m_timePerEgg;
    float m_eggTimer;
    float m_totalTime = 10.0f;
    float m_spawnCenterX;     // the player's posision at the beginning of the wave marks the center of the spawn

    // meteor stuff
    int m_meteorNumSpawn;
    int m_meteorSpawnLeft;
    float m_meteorTimePerSpawn;
    float m_meteorSpawnTimer;

    const float s_spawnPosY = 4.5f;
    const float s_eggSpacing = 9.0f;  // +/- this dist from player's pos
    const float s_eggPosY = 5.0f;
    const float s_eggSpeedY = -2.2f;

    // boss stuff
    enum BossStage
    {
        THREE_WORMS,
        TWO_NINJA,
        TWO_SUPER_WORM,
        SUPER_PLUS_NINJA,
        TWO_MECHA_WORM,

        TOTAL
    }
    BossStage m_bossStage;
    int m_bossNumCircuit;

    public override void Start()
    {
        base.Start();
        m_state = State.EGGS;
        m_bossStage = BossStage.THREE_WORMS;
        Advance();
    }

    public override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;
        if (m_meteorSpawnLeft > 0)
            m_meteorSpawnTimer -= dt;
        while (m_meteorSpawnTimer <= 0.0f && m_meteorSpawnLeft > 0)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-GameManager.Get().m_worldWidth, GameManager.Get().m_worldWidth),
                s_spawnPosY,
                0.0f
                ); ;
            Meteor.Spawn(spawnPos);
            --m_meteorSpawnLeft;
            m_meteorSpawnTimer += m_meteorTimePerSpawn;
        }

        bool advance = false;
        switch (m_state)
        {
            case State.EGGS:
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
                    m_eggTimer += m_timePerEgg;
                }

                if (m_eggsLeft <= 0)
                {   // wait for all eggs to spawn
                    if (EnemyBird.GetCount() <= 0 && Egg.GetCount() <= 0)
                    {   // wait for all enemies to be cleared out
                        advance = true;
                    }
                }
                break;
            case State.BOSS:
                if (Worm.GetCount() <= 0)
                {   // wait for all the worms to be dead
                    if (EnemyBird.GetCount() <= 0 && Egg.GetCount() <= 0)
                    {   // wait for all enemies to be cleared out
                        advance = true;
                    }
                }
                break;
        }

        if (advance)
        {
            switch (m_state)
            {
                case State.EGGS:
                    m_state = State.BOSS;
                    SpawnBoss();
                    break;
                case State.BOSS:
                    m_bossStage++;
                    if (m_bossStage >= BossStage.TOTAL)
                    {
                        m_bossStage = (BossStage)0;
                        ++m_bossNumCircuit;
                    }
                    m_state = State.EGGS;
                    break;
            }
            Advance();
        }
    }

    public override bool IsDone()
    {
        return false;
    }

    void Advance()
    {
        bool doEggs = (m_state == State.EGGS) || m_bossNumCircuit > 0;
        bool doMeteors = (m_state == State.EGGS) || m_bossNumCircuit > 1;

        if (doEggs)
        {
            m_numEgg1 += 6;
            if (m_numEgg1 > 48)
            {
                m_numEgg1 -= 48;
                m_numEgg2 += 6;
            }
            m_numEgg2 += 3;
            if (m_numEgg2 > 48)
            {
                m_numEgg2 -= 48;
                m_numEgg3 += 4;
            }
            m_numEgg3 += 1;
            m_numEgg = m_numEgg1 + m_numEgg2 + m_numEgg3;
            m_totalTime = 3.0f + 0.3f * m_eggsLeft;
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

            m_timePerEgg = m_totalTime / m_numEgg;
            m_eggTimer = m_timePerEgg;

            if (firstWave)
            {
                firstWave = false;
            }
            else
            {
                if (m_eggsLeft > 45)
                    SetText("Huevos Rancheros");
                else if (m_eggsLeft > 30)
                    SetText("Egg Tsunami");
                else if (m_eggsLeft > 15)
                    SetText("Grade A Extra Large Egg Wave");
                else if (m_eggsLeft > 9)
                    SetText("Big Egg Wave");
                else
                    SetText("Egg Wave");
            }
        }

        if (doMeteors)
        {
            m_meteorSpawnLeft = m_meteorNumSpawn;
            m_meteorNumSpawn += 8;
            m_meteorTimePerSpawn = m_totalTime / m_meteorNumSpawn;
            m_meteorSpawnTimer = m_meteorTimePerSpawn;
        }
    }

    void SpawnBoss()
    {
        Vector3 pos = Player.Get().transform.position;

        switch (m_bossStage)
        {
            case BossStage.THREE_WORMS:
                Worm.Spawn(pos, Worm.WormType.WORM, Worm.Pattern.ARC_LEFT);
                pos.x += 6.0f;
                Worm.Spawn(pos, Worm.WormType.WORM, Worm.Pattern.ARC_LEFT);
                pos.x += 6.0f;
                Worm.Spawn(pos, Worm.WormType.WORM, Worm.Pattern.ARC_LEFT);
                SetText("Eat the Worm");
                break;
            case BossStage.TWO_NINJA:
                pos.y = GameManager.Get().GetScreenBounds().max.y + 2.0f;
                pos.x -= 6.0f;
                Ninja.Spawn(pos);
                pos.x += 12.0f;
                Ninja.Spawn(pos);
                SetText("Turtle Soup");
                break;
            case BossStage.TWO_SUPER_WORM:
                pos.x += 6.0f;
                Worm.Spawn(pos, Worm.WormType.SUPER, Worm.Pattern.ARC_LEFT);
                pos.x -= 12.0f;
                Worm.Spawn(pos, Worm.WormType.SUPER, Worm.Pattern.ARC_RIGHT);
                SetText("Watch Out! They Bite!");
                break;
            case BossStage.SUPER_PLUS_NINJA:
                pos.x -= 6.0f;
                Worm.Spawn(pos, Worm.WormType.SUPER, Worm.Pattern.ARC_RIGHT);
                pos.x += 12.0f;
                pos.y = GameManager.Get().GetScreenBounds().max.y + 2.0f;
                Ninja.Spawn(pos);
                SetText("Combo Platter");
                break;
            case BossStage.TWO_MECHA_WORM:
                pos.x += 6.0f;
                Worm.Spawn(pos, Worm.WormType.MECHA, Worm.Pattern.ARC_LEFT);
                pos.x -= 12.0f;
                Worm.Spawn(pos, Worm.WormType.MECHA, Worm.Pattern.ARC_RIGHT);
                SetText("Double Trouble");
                break;
        }
    }
}
