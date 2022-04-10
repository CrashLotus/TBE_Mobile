using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject m_bulletPrefab;
    public float m_fireDelay = 0.25f;   //time between bullets (in seconds)
    public float m_recoil = 8.0f;

    protected float m_fireTimer;        //countdown timer for next bullet
    protected bool m_triggerHold;
    protected GameObject m_owner;
    protected SpriteRenderer m_ownerSprite;
    protected Vector3 m_offset;

    // Start is called before the first frame update
    void Start()
    {
        m_fireTimer = 0.0f;
        m_triggerHold = false;
        m_owner = transform.parent.gameObject;
        m_ownerSprite = m_owner.GetComponent<SpriteRenderer>();
        m_offset = transform.localPosition;
    }

    public void HitTrigger()
    {
        m_fireTimer = 0.0f;
        m_triggerHold = true;
    }

    public void HoldTrigger()
    {
        m_triggerHold = true;
    }

    /// <summary>
    /// Normally you should not need to call this explicitly.
    /// Only if you want to make sure the trigger has been released and just maybe you didn't call Update() yet this frame.
    /// </summary>
    public void ReleaseTrigger()
    {
        m_triggerHold = false;
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        m_fireTimer -= dt;
        while (m_fireTimer < 0.0f)
        {
            if (m_triggerHold && Fire())
            {
                m_fireTimer += m_fireDelay;
            }
            else
            {
                m_fireTimer = 0.0f;
            }
        }

        m_triggerHold = false;
    }

    public float TimeToNextFire()
    {
        return m_fireTimer;
    }

    protected virtual bool Fire()
    {
        ObjectPool pool = ObjectPool.GetPool(m_bulletPrefab, 64);
        GameObject bulletObj = pool.Allocate(GetFirePos());
        if (null != bulletObj)
        {
            Vector3 dir = Vector3.right;
            if (m_ownerSprite.flipX)
                dir = -dir;

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Fire(dir);

            Bird bird = m_owner.GetComponent<Bird>();
            if (null != bird)
                bird.Push(-dir * m_recoil);

            return true;
        }
        return false;
    }

    protected Vector3 GetFirePos()
    {
        Vector3 pos = m_owner.transform.position;
        if (m_ownerSprite.flipX)
            pos.x -= m_offset.x;
        else
            pos.x += m_offset.x;
        pos.y += m_offset.y;
        return pos;
    }
}
