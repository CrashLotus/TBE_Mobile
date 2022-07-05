using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon
{
    public GameObject m_powerBullet;
    public Sound m_powerSound;

    GameObject m_baseBullet;
    Sound m_baseSound;

    protected override void Start()
    {
        base.Start();
        m_baseBullet = m_bulletPrefab;
        m_baseSound = m_fireSound;
    }

    protected override bool Fire()
    {
        Player.EggBonus bonusMode = Player.Get().GetBonusMode();
        if (bonusMode == Player.EggBonus.POWER_LASER
            || bonusMode == Player.EggBonus.MULTISHOT
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
        return base.Fire();
    }
}
