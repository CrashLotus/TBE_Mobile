using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : EnemyWeapon
{
    public GameObject m_blastPrefab;

    public override void WarmUp()
    {
        base.WarmUp();
        if (null != m_bulletPrefab)
        {
            ObjectPool.GetPool(m_blastPrefab, 4);
        }
    }

    protected override bool Fire()
    {
        if (null != m_blastPrefab)
        {
            var pool = ObjectPool.GetPool(m_blastPrefab, 4);
            pool.Allocate(GetFirePos());
        }
        return base.Fire();
    }
}
