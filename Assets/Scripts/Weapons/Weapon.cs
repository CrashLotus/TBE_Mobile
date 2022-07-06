using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject m_bulletPrefab;
    public float m_fireDelay = 0.25f;   //time between bullets (in seconds)
    public float m_recoil = 8.0f;
    public Sound m_fireSound;

    protected float m_fireTimer;        //countdown timer for next bullet
    protected bool m_triggerHold;
    protected GameObject m_owner;
    protected SpriteRenderer m_ownerSprite;
    protected Vector3 m_offset;

    public virtual void WarmUp()
    {
        if (null != m_bulletPrefab)
        {
            ObjectPool.GetPool(m_bulletPrefab, 64);
            Bullet bullet = m_bulletPrefab.GetComponent<Bullet>();
            if (null != bullet)
                bullet.WarmUp();
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_fireTimer = 0.0f;
        m_triggerHold = false;
        m_owner = transform.parent.gameObject;
        m_ownerSprite = m_owner.GetComponent<SpriteRenderer>();
        m_offset = transform.localPosition;
    }

    public virtual void HitTrigger()
    {
        m_fireTimer = 0.0f;
        m_triggerHold = true;
    }

    public virtual void HoldTrigger()
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

    public virtual void Update()
    {
        float dt = Time.deltaTime;

        m_fireTimer -= dt;
        while (m_fireTimer < 0.0f)
        {
            if (m_triggerHold && Fire())
            {
                m_owner.SendMessage("OnFireWeapon", SendMessageOptions.DontRequireReceiver);
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
        if (null == pool)
            return false;
        GameObject bulletObj = pool.Allocate(GetFirePos());
        if (null != bulletObj)
        {
            if (null != m_fireSound)
                m_fireSound.Play();

            Vector3 dir = Aim();

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

    protected Vector3 Dir2Player(Vector3 pos)
    {
        Vector3 dir = new Vector3(1.0f, 0.0f, 0.0f);
        Player player = Player.Get();
        if (null != player)
        {
            dir = player.transform.position - pos;
            dir.Normalize();
        }
        else
        {
            float ang = Random.Range(-0.3f, 0.3f);
            dir = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0.0f);
            if (m_ownerSprite.flipX)
                dir.x = -dir.x;
        }

        return dir;
    }

    protected virtual Vector3 Aim()
    {
        Vector3 dir = Vector3.right;
        if (m_ownerSprite.flipX)
            dir = -dir;
        return dir;
    }
}
