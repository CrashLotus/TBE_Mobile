using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSection : PooledObject, IHitPoints
{
    public Vector3 m_headJoint;
    public Vector3 m_tailJoint;
    protected SpriteRenderer m_sprite;
    public int m_maxHitPoints = 20;
    protected float m_hitPoints;
    protected Worm m_head;
    protected WormSection m_prevSection;
    protected WormSection m_nextSection;

    const float s_minAng = -30.0f;  // I currently have these disabled
    const float s_maxAng = 30.0f;
    const int s_hitDamage = 1;
    const float s_counterSteer = 0.5f;

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_hitPoints = m_maxHitPoints;
        m_sprite = GetComponent<SpriteRenderer>();
        Utility.HitFlashReset(gameObject);
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
#if true
            // modeling this as a tram-car with 2 axels: front and back
            // first get the current angle from the back axel to the front
            Vector3 headPos = GetHeadPos(); // front axel rotates here
            Vector3 tailPos = GetTailPos(); // back axel rotates here
            Vector3 delta = headPos - tailPos;
            float curAng = Mathf.Atan2(delta.y, delta.x);
            // the front axel steers straight into where the parent car is pulling it
            Vector3 headTarget = parent.GetTailPos();
            delta = headTarget - headPos;
            delta.z = 0.0f;
            if (Mathf.Abs(delta.x) > 0.0f || Mathf.Abs(delta.y) > 0.0f)
            {   // if the game is paused, the motion will be zero and we'll get 0 angles
                float headAng = Mathf.Atan2(delta.y, delta.x);
                float angDelta = headAng - curAng;
                while (angDelta < -Mathf.PI)
                    angDelta += 2.0f * Mathf.PI;
                while (angDelta > Mathf.PI)
                    angDelta -= 2.0f * Mathf.PI;
                // the back axel counter-steers a bit in the opposite direction to help the tram-car make the turn
                float tailAng = curAng - s_counterSteer * angDelta;
                // Each axel rolls forward in its own direction, and from that, we'll extract the final angle of the car itself
                float dist = delta.magnitude;
                Vector3 newTail = new Vector3(Mathf.Cos(tailAng), Mathf.Sin(tailAng), 0.0f);
                newTail = tailPos + dist * newTail;
                delta = headTarget - newTail;
                float finalAng = Mathf.Atan2(delta.y, delta.x);
                float angDiff = finalAng - curAng;
                angDiff *= Mathf.Rad2Deg;
                while (angDiff < -180.0f)
                    angDiff += 360.0f;
                while (angDiff > 180.0f)
                    angDiff -= 360.0f;
                float ang = transform.localEulerAngles.z;
                ang += angDiff;
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
            }

            // finally move the car so the front axel is attached to the parent's hitch point
            Vector3 pos = transform.position;
            headPos = GetHeadPos();
            delta = headTarget - headPos;
            delta.z = 0.0f;
            pos += delta;
            transform.position = pos;
#else
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
#endif

#if false
            headPos += delta;
            Vector3 newHeadPos = GetHeadPos();
            delta = headPos - newHeadPos;
            delta.z = 0.0f;
            pos += delta;
            transform.position = pos;
#endif

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

    public IHitPoints.DamageReturn Damage(float damage, IHitPoints.HitType hitType)
    {
        if (null == m_prevSection)
        {   // head passes damage onto next section
            Utility.HitFlash(gameObject);
            return m_head.HeadDamage(damage, hitType);
        }
        if (null == m_nextSection)
        {   // tail passes damage onto prev section
            Utility.HitFlash(gameObject);
            return m_prevSection.Damage(damage, hitType);
        }
        if (m_hitPoints > 0.0f)
        {
            m_hitPoints -= damage;
            if (m_hitPoints <= 0.0f)
            {
                Explode();
                return IHitPoints.DamageReturn.KILLED;    // I've been killed
            }
            Utility.HitFlash(gameObject);
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

    public void OnLaunchRocket()
    {
        MechaWorm mecha = m_head as MechaWorm;
        if (mecha)
        {
            mecha.LaunchRocket(this);
        }
    }
}
