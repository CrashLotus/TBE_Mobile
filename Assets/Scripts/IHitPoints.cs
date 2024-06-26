//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

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

    public DamageReturn Damage(float damage, HitType hitType);
}
