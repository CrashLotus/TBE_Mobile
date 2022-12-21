using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormTail : WormSection
{
    public GameObject m_poopEffect;
    public Transform m_poopSpot;
    public float m_eggSpeed = 3.0f;

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
                ObjectPool pool = ObjectPool.GetPool(m_poopEffect, 16);
                if (null != pool)
                {
                    GameObject poop = pool.Allocate(m_poopSpot.position);
                    if (null != poop)
                        poop.transform.rotation = m_poopSpot.rotation;
                }
                Egg.Spawn(pos, 1, -m_eggSpeed * m_poopSpot.right);
            }
        }
    }
}
