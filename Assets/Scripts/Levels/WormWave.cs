using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/WormWave")]
public class WormWave : Wave
{
    public Worm.WormType m_wormType = Worm.WormType.WORM;
    public Worm.Pattern m_pattern = Worm.Pattern.ARC_RIGHT;
    public float m_xOffset = 0.0f;
    public bool m_waitOnWorms = true;
    public bool m_waitOnEnemies = true;

    public override void Start()
    {
        Wave insertWave = this;
        if (m_waitOnWorms)
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

        base.Start();

        Vector3 pos = Player.Get().transform.position;
        pos.x += m_xOffset;
        Worm.Spawn(pos, m_wormType, m_pattern);

        m_isDone = true;
    }
}
