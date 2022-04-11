using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggHitPoints : HitPoints
{
    Egg m_owner;

    protected override void Start()
    {
        base.Start();
        m_owner = gameObject.GetComponent<Egg>();
    }

    public override DamageReturn Damage(float damage, HitType hitType)
    {
        return m_owner.Damage(damage, hitType);
    }

    public override void Explode()
    {
        // do nothing
    }

}
