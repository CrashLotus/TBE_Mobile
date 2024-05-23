using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormTail : WormSection
{
    public GameObject m_poopEffect;
    public Transform m_poopSpot;
    public float m_eggSpeed = 1.0f;
    public int m_eggPower = 1;

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
                    Egg.Spawn(pos, m_eggPower, -m_eggSpeed * m_poopSpot.right, 0);  //mrwTODO increase generation over time?
                }
            }
        }
    }
}
