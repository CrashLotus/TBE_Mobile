using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitPoints
{
    public enum DamageReturn
    {
        PASS_THROUGH,       // just pass right through with no effect
        NO_DAMAGE,          // hits me, but does no damage
        DAMAGED,            // hits me and does damage
        KILLED              // kills me
    }
    public enum HitType
    {
        NONE,
        BULLET,
        MISSILE,
        LASER,
        METEOR,

        TOTAL
    }

    public DamageReturn Damage(int damage, HitType hitType);
}
