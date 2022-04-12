using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitPoints : HitPoints
{
    const int s_maxEggStart = 10;
    const int s_maxEggFinish = 20;

    public override DamageReturn Damage(float damage, HitType hitType)
    {
#if false   //mrwTODO
        if (m_eggShieldOn)
        {
            m_eggShieldOn = false;
            AudioComponent.Get().PlaySound("BubbleBurst");
            return DamageReturn.NO_DAMAGE;
        }

        m_comboTimer = 0.0f;
        m_comboPoints = 0;

        int shake = Math.Min((int)damage, s_hitShake.Length - 1);
        Camera.Shake(s_hitShake[shake].X, s_hitShake[shake].Y);

#if CHEAT_INVULNERABLE
        return DamageReturn.NO_DAMAGE;
#endif
#endif
//        EggBonus startBonus = GetBonusMode(); //mrwTODO

        int maxEgg = MaxEgg();
        if (m_hitPoints > maxEgg)   // you can have more than max eggs, but you can't have more than max hit points
            m_hitPoints = maxEgg;
        float eggDamage = Mathf.Min(damage, m_hitPoints);
        int numEgg = (int)eggDamage;
        Vector3 pos = transform.position;
        while (numEgg >= 3)
        {
            Egg.Spawn(pos, 3);
            numEgg -= 3;
        }
        while (numEgg >= 2)
        {
            Egg.Spawn(pos, 2);
            numEgg -= 2;
        }
        while (numEgg >= 1)
        {
            Egg.Spawn(pos, 1);
            numEgg -= 1;
        }

//        AudioComponent.Get().PlaySound("Punch");  //mrwTODO
        DamageReturn ret = base.Damage(damage, hitType);
#if false   //mrwTODO
        if (DamageReturn.KILLED == ret)
        {
            // shut off the laser - if it is firing
            m_megaLaser.ReleaseTrigger();
            m_megaLaser.Update(0.0f);
            Game1.SetState(Game1.State.GAME_OVER);
        }
        else if (GetBonusMode() != startBonus)
        {
            AudioComponent.Get().PlaySound("PowerDown");
        }
#endif
        return ret;
    }

    public static int MaxEgg()
    {
#if false   //mrwTODO
        if (IsAbilityUnlocked(Upgrade.Type.EGG5))
            return s_maxEggStart + 5;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG4))
            return s_maxEggStart + 4;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG3))
            return s_maxEggStart + 3;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG2))
            return s_maxEggStart + 2;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG1))
            return s_maxEggStart + 1;
#endif
        return s_maxEggStart;
    }
}
