//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormRocket : Bullet, IHitPoints
{
    public Sound m_destroyedSound;

    public IHitPoints.DamageReturn Damage(float damage, IHitPoints.HitType hitType)
    {
        if (damage > 0.0f)
        {
            if (null != m_destroyedSound)
                m_destroyedSound.Play();
            Explode();
            return IHitPoints.DamageReturn.KILLED;    // I've been killed
        }

        return IHitPoints.DamageReturn.PASS_THROUGH;       // I'm already dead    }
    }
}
