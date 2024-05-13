using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaWorm : Worm
{
    public GameObject m_rocketPrefab;
    public float m_fireRate = 1.0f;

    float m_fireTimer = 0.0f;

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_fireTimer = 0.0f;
    }

    protected override void _WarmUp()
    {
        base._WarmUp();
        ObjectPool.GetPool(m_rocketPrefab, 16);
    }

    protected override void Update()
    {
        base.Update();

        float dt = BulletTime.Get().GetDeltaTime();
        int sectionIndex = (int)m_fireTimer;
        m_fireTimer += m_fireRate * dt;
        if ((int)m_fireTimer != sectionIndex)
        {
            if (sectionIndex < m_sections.Count)
            {
                WormSection section = m_sections[sectionIndex];
                Vector3 sectionPos = Camera.main.WorldToViewportPoint(section.transform.position);
                if (sectionPos.y > 0.0f && sectionPos.y < 1.0f)
                {
                    Fire(section);
                }
                else
                {
                    if (sectionIndex == 0)
                    {   // wait for the 1st section to get on screen
                        m_fireTimer = 0.0f;
                    }
                }
            }
            if (sectionIndex > m_sections.Count)
            {
                m_fireTimer = 0.0f;
            }
        }
    }

    void Fire(WormSection section)
    {
        Animator anim = section.gameObject.GetComponent<Animator>();
        if (anim)
            anim.SetTrigger("Fire");
    }

    public void LaunchRocket(WormSection section)
    { 
        ObjectPool pool = ObjectPool.GetPool(m_rocketPrefab, 16);
        if (null == pool)
            return;
        WormTail tail = section as WormTail;
        int numRocket = section.transform.childCount;
        for (int i = 0; i < numRocket; ++i)
        {
            Transform xform = section.transform.GetChild(i);
            if (tail && tail.m_poopSpot == xform)
                continue;   // don't fire a rocket from the poop spot
            GameObject obj = pool.Allocate(xform.position);
            if (null == obj)
                return;
            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet)
            {
                Vector3 dir = xform.right;
                if (m_sprite.flipY)
                    dir = -dir;
                bullet.Fire(dir);
            }
        }
    }
}
