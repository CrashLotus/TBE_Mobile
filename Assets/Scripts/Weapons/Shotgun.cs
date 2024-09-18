//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public GameObject m_blastPrefab;
    const float s_coneAngle = 30.0f;
    const int s_numStars = 5;
    const float s_preFireDelay = 0.5f;

    public override void WarmUp()
    {
        base.WarmUp();
        if (null != m_bulletPrefab)
        {
            ObjectPool.GetPool(m_blastPrefab, 4);
        }
    }

    IEnumerator DelayedFire()
    {
        yield return new WaitForSeconds(s_preFireDelay);
        DoFire();
    }

    protected override bool Fire()
    {
        StartCoroutine(DelayedFire());
        return true;
    }

    bool DoFire()
    { 
        ObjectPool pool = ObjectPool.GetPool(m_bulletPrefab, 64);
        if (null == pool)
            return false;

        Vector3 pos = GetFirePos();
        Vector3 dir = Aim();

        bool fired = false;
        for (int i = 0; i < s_numStars; ++i)
        {
            GameObject bulletObj = pool.Allocate(pos);
            if (null != bulletObj)
            {
                fired = true;

                float ang = ((float)i / (float)s_numStars - 0.5f) * s_coneAngle;
                ang = Mathf.Deg2Rad * ang;
                Vector3 tempDir = new Vector3(dir.x * Mathf.Cos(ang), Mathf.Sin(ang));
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.Fire(tempDir);
            }
        }

        if (false == fired)
            return false;

        if (null != m_blastPrefab)
        {
            var blastPool = ObjectPool.GetPool(m_blastPrefab, 4);
            GameObject blast = blastPool.Allocate(pos);
            if (null != blast)
            {
                SpriteRenderer blastSprite = blast.GetComponent<SpriteRenderer>();
                if (null != blastSprite)
                    blastSprite.flipX = m_ownerSprite.flipX;
            }
        }

        if (null != m_fireSound)
            m_fireSound.Play();

        Bird bird = m_owner.GetComponent<Bird>();
        if (null != bird)
            bird.Push(-dir * m_recoil);

        return true;
    }
}
