using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PooledObject
{
    public float m_speed = 14.0f;
    public int m_damage = 1;
    public float m_force = 0.75f;
    public IHitPoints.HitType m_hitType = IHitPoints.HitType.BULLET;
    public GameObject m_impactPow;
    public Sound m_impactSound;

    static List<Bullet> s_theList = new List<Bullet>();
    protected Vector3 m_vel;
    protected Vector3 m_dir;
    Renderer m_renderer;

    public void WarmUp()
    {
        if (null != m_impactPow)
            ObjectPool.GetPool(m_impactPow, 32);
    }

    protected virtual void Start()
    {
        m_renderer = GetComponent<Renderer>();
    }

    public virtual void Fire(Vector3 dir)
    {
        m_dir = dir;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
        m_vel = m_dir * m_speed;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Vector3 pos = transform.position;
        pos += m_vel * Time.deltaTime;
        transform.position = pos;

        if (null != m_renderer)
        {
            Bounds bounds = m_renderer.bounds;
            Bounds screenBounds = GameManager.Get().GetScreenBounds();
            if (false == bounds.Intersects(screenBounds))
            {
                Free();
            }
        }
    }

    public virtual void Hit(GameObject other)
    {
        IHitPoints hp = other.GetComponent<IHitPoints>();
        if (null != hp)
        {
            IHitPoints.DamageReturn damageReturn = hp.Damage(m_damage, m_hitType);
            if (IHitPoints.DamageReturn.PASS_THROUGH != damageReturn)
            {
                Impact(other, damageReturn);
                Explode();
            }
        }
    }

    public virtual void Explode()
    {
        if (null != m_impactPow)
        {
            Vector3 pos = transform.position;
            pos.z -= 1.0f;  // sort the pows to the front
            ObjectPool pool = ObjectPool.GetPool(m_impactPow, 32);
            if (null != pool)
                pool.Allocate(pos);
        }
        if (null != m_impactSound)
            m_impactSound.Play();
        Free();
    }

    protected virtual void Impact(GameObject other, IHitPoints.DamageReturn damageReturn)
    {
        Bird bird = other.GetComponent<Bird>();
        if (null != bird)
            bird.Push(m_vel * m_force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Hit(collision.gameObject);
    }

    private void OnEnable()
    {
        s_theList.Add(this);
    }

    private void OnDisable()
    {
        s_theList.Remove(this);
    }

    public static void DeleteAll()
    {
        for (int i = s_theList.Count - 1; i >= 0; --i)
        {
            s_theList[i].Free();
        }
    }
}
