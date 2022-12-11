using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSection : PooledObject
{
    public Vector3 m_headJoint;
    public Vector3 m_tailJoint;
    const float m_minAng = -30.0f;
    const float m_maxAng = 30.0f;
    protected SpriteRenderer m_sprite;

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_sprite = GetComponent<SpriteRenderer>();
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
            angle = Mathf.Clamp(angle, m_minAng, m_maxAng);
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
}
