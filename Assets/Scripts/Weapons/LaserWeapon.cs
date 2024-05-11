using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon
{
    public GameObject m_powerBullet;
    public Sound m_powerSound;

    GameObject m_baseBullet;
    Sound m_baseSound;
    const float s_coneAngle = 10.0f;

    static readonly float[] s_fireDelay = { 0.25f, 0.2f, 0.175f };

    protected override void Start()
    {
        base.Start();
        m_baseBullet = m_bulletPrefab;
        m_baseSound = m_fireSound;
    }

    bool FireAtAngle(float ang)
    {
        ObjectPool pool = ObjectPool.GetPool(m_bulletPrefab, 64);
        if (null == pool)
            return false;
        GameObject bulletObj = pool.Allocate(GetFirePos());
        if (null != bulletObj)
        {
            Vector2 dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * ang), Mathf.Sin(Mathf.Deg2Rad * ang));
            if (m_ownerSprite.flipX)
                dir.x = -dir.x;

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Fire(dir);

            return true;
        }
        return false;
    }

    protected override bool Fire()
    {
        int fireRate = Player.GetBulletSpeedLevel();
        m_fireDelay = s_fireDelay[fireRate];

        Player.EggBonus bonusMode = Player.GetBonusMode();
        if (bonusMode == Player.EggBonus.POWER_LASER
            || bonusMode == Player.EggBonus.MULTISHOT
            || bonusMode == Player.EggBonus.MEGA_LASER
            )
        {
            m_bulletPrefab = m_powerBullet;
            m_fireSound = m_powerSound;
        }
        else
        {
            m_bulletPrefab = m_baseBullet;
            m_fireSound = m_baseSound;
        }

        // fire the base projectile
        if (false == base.Fire())
        {
            return false;
        }

        // multi-shot
        if (bonusMode == Player.EggBonus.MULTISHOT
            || bonusMode == Player.EggBonus.MEGA_LASER
            )
        {
            FireAtAngle(s_coneAngle);
            FireAtAngle(-s_coneAngle);
        }
        return true;
    }
}
