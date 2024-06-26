//----------------------------------------------------------------------------------------
//	Copyright � 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/NinjaWave")]
public class NinjaWave : Wave
{
    public float m_xOffset = 0.0f;
    public bool m_waitOnEnemies = true;

    public override void Start()
    {
        Wave insertWave = this;
        if (m_waitOnEnemies)
        {   // wait for the enemies to be done
            EnemyWave enemy = ScriptableObject.CreateInstance<EnemyWave>();
            Level.Get().AddWave(enemy, insertWave);
        }

        base.Start();

        Vector3 pos = Player.Get().transform.position;
        pos.x += m_xOffset;
        pos.y = GameManager.Get().GetScreenBounds().max.y + 2.0f;
        
        Ninja.Spawn(pos);
    }

    public override bool IsDone()
    {
        return true;
    }
}
