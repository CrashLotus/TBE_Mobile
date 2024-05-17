using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    public GameObject m_baseBullet;
    GameObject m_passThru;

    protected override void Impact(GameObject other, IHitPoints.DamageReturn damageReturn)
    {
        if (other == m_passThru)
            return;
        base.Impact(other, damageReturn);
        if (m_baseBullet != null)
        {
            SaveData data = SaveData.Get();
            if (false == data.HasUpgrade("BULLETBOUNCE"))
                return;
            float ang = Random.Range(-Mathf.PI, Mathf.PI);
            Vector3 dir = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0.0f);
            ObjectPool pool = ObjectPool.GetPool(m_baseBullet, 64);
            if (null == pool)
                return;
            GameObject bulletObj = pool.Allocate(transform.position);
            if (null != bulletObj)
            {
                // mrwTODO bounce Sound

                PlayerBullet bullet = bulletObj.GetComponent<PlayerBullet>();
                if (null != bullet)
                {
                    bullet.Fire(dir);
                    bullet.m_passThru = other;
                }
            }
        }
    }
}
