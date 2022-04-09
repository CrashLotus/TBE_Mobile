using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoints : MonoBehaviour
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

    public float m_hitPoints = 1.0f;

    protected int[] m_hitByType;
    protected HitType m_lastHit;

    protected virtual void Start()
    {
        m_hitByType = new int[(int)HitType.TOTAL];
        m_lastHit = HitType.NONE;
    }

    public virtual DamageReturn Damage(float damage, HitType hitType)
    {
        ++m_hitByType[(int)hitType];
        m_lastHit = hitType;
        if (m_hitPoints > 0.0f)
        {
            m_hitPoints -= damage;
            if (m_hitPoints <= 0.0f)
            {
                Explode();
                return DamageReturn.KILLED;    // I've been killed
            }
            return DamageReturn.DAMAGED;
        }

        return DamageReturn.PASS_THROUGH;       // I'm already dead
    }

    public virtual void Explode()
    {
        Destroy(gameObject);
    }
}
