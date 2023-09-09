using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/NinjaWave")]
public class NinjaWave : Wave
{
    public float m_xOffset = 0.0f;
    public bool m_waitOnNinja = true;
    public bool m_waitOnEnemies = true;

    public override void Start()
    {
        Wave insertWave = this;
        if (m_waitOnNinja)
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
        //mrwTODO insert Ninja here
        //Worm.Spawn(pos, m_wormType, m_pattern);
        Debug.LogError("Ninja Goes Here");
    }

    public override bool IsDone()
    {
        if (m_isDone)
            return true;

        //mrwTODO insert Ninja here
        //if (Worm.GetCount() > 0)
        //    return false;

        return true;
    }
}
