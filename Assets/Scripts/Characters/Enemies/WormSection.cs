using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSection : PooledObject, IHitPoints
{
    public Vector3 m_headJoint;
    public Vector3 m_tailJoint;
    const float s_minAng = -30.0f;
    const float s_maxAng = 30.0f;
    const int s_hitDamage = 1;
    protected SpriteRenderer m_sprite;
    protected int m_maxHitPoints = 20;
    protected int m_hitPoints = 20;
    protected Worm m_head;
    protected WormSection m_prevSection;
    protected WormSection m_nextSection;

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_hitPoints = m_maxHitPoints;
        m_sprite = GetComponent<SpriteRenderer>();
    }

    public void SetHead(Worm head)
    {
        m_head = head;
    }

    public void SetNext(WormSection next)
    {
        m_nextSection = next;
    }

    public void SetPrev(WormSection prev)
    {
        m_prevSection = prev;
    }

    public void Connect(WormSection prev, WormSection next)
    {
        m_prevSection = prev;
        m_nextSection = next;
    }

    Vector3 TransformPoint(Vector3 point)
    {
        if (null != m_sprite)
        {
            if (m_sprite.flipY)
                point.y = -point.y;
        }
        return transform.TransformPoint(point);
    }

    public Vector3 GetTailPos()
    {
        return TransformPoint(m_tailJoint);
    }

    public Vector3 GetHeadPos()
    {
        return TransformPoint(m_headJoint);
    }

    public SpriteRenderer GetSprite()
    {
        return m_sprite;
    }

    public void UpdatePosition(WormSection parent)
    {
        if (null != parent)
        {
            // rotate to aim at the parent's tail
            Vector3 targetPos = parent.GetTailPos();
            Vector3 tailPos = GetTailPos();
            Vector3 targetDelta = targetPos - tailPos;
            float targetAng = Mathf.Atan2(targetDelta.y, targetDelta.x);
            Vector3 rest = m_headJoint - m_tailJoint;
            float restAng = Mathf.Atan2(rest.y, rest.x);
            targetAng -= restAng;
            float angle = Mathf.Rad2Deg * targetAng - parent.transform.localEulerAngles.z;
            while (angle < -180.0f)
                angle += 360.0f;
            while (angle > 180.0f)
                angle -= 360.0f;
            angle = Mathf.Clamp(angle, s_minAng, s_maxAng);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, parent.transform.localEulerAngles.z + angle);
            Vector3 pos = transform.position;
            Vector3 headPos = GetHeadPos();
            Vector3 delta = targetPos - headPos;
            delta.z = 0.0f;
            pos += delta;
            transform.position = pos;

#if false   //mrwTODO make the body follow the head flip
            if (null != m_sprite)
            {
                if (targetDelta.x < -0.1f)
                    m_sprite.flipY = true;
                else if (targetDelta.x > 0.1f)
                    m_sprite.flipY = false;
            }
#endif
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IHitPoints hit = collision.gameObject.GetComponent<IHitPoints>();
        if (null != hit)
        {
            hit.Damage(s_hitDamage, IHitPoints.HitType.NONE);
        }
    }

    public IHitPoints.DamageReturn Damage(int damage, IHitPoints.HitType hitType)
    {
        if (null == m_prevSection)
        {   // head passes damage onto next section
            return m_head.HeadDamage(damage, hitType);
        }
        if (null == m_nextSection)
        {   // tail passes damage onto prev section
            return m_prevSection.Damage(damage, hitType);
        }
        if (m_hitPoints > 0)
        {
            m_hitPoints -= damage;
            if (m_hitPoints <= 0)
            {
                Explode();
                return IHitPoints.DamageReturn.KILLED;    // I've been killed
            }
            return IHitPoints.DamageReturn.DAMAGED;
        }

        return IHitPoints.DamageReturn.PASS_THROUGH;       // I'm already dead
    }

    protected virtual void Explode()
    {
        if (null != m_head)
        {
            m_head.SectionDestroyed(this);
        }
    }
}
