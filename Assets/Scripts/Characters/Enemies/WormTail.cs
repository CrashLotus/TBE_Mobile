//----------------------------------------------------------------------------------------
//	Copyright � 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormTail : WormSection
{
    public GameObject m_poopEffect;
    public Transform m_poopSpot;
    public float m_eggSpeed = 1.0f;
    public int m_eggPower = 1;

    int m_generation = 0;

    public void _WarmUp()
    {
        if (null != m_poopEffect)
            ObjectPool.GetPool(m_poopEffect, 16);
    }

    public void Poop()
    {
        if (null != m_poopEffect && null != m_poopSpot)
        {
            Vector3 pos = m_poopSpot.transform.position;
            if (pos.y > GameManager.Get().GetLavaHeight())
            {
                if (GameManager.Get().GetScreenBounds().Contains(pos))
                {
                    GameObject poop = ObjectPool.Allocate(m_poopEffect, 16, pos);
                    if (null != poop)
                        poop.transform.rotation = m_poopSpot.rotation;
                    Egg.Spawn(pos, m_eggPower, -m_eggSpeed * m_poopSpot.right, m_generation);
                }
            }
        }
    }

    public void SetGeneration(int generation)
    {
        m_generation = generation;
    }
}
