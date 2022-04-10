using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PooledObject
{
    public float m_speed = 14.0f;
    public float m_damage = 1.0f;
    public float m_force = 0.75f;
    public HitPoints.HitType m_hitType = HitPoints.HitType.BULLET;
    protected Vector3 m_vel;
    Renderer m_renderer;

    protected virtual void Start()
    {
        m_renderer = GetComponent<Renderer>();
    }

    public virtual void Fire(Vector3 dir)
    {
        transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x));
        m_vel = dir * m_speed;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.flipX = dir.x < 0.0f;
    }

    // Update is called once per frame
    void Update()
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
        HitPoints hp = other.GetComponent<HitPoints>();
        if (null != hp)
        {
            HitPoints.DamageReturn damageReturn = hp.Damage(m_damage, m_hitType);
            if (HitPoints.DamageReturn.PASS_THROUGH != damageReturn)
            {
                Impact(other, damageReturn);
                Explode();
            }
        }
    }

    public virtual void Explode()
    {
        Free();
    }

    protected virtual void Impact(GameObject other, HitPoints.DamageReturn damageReturn)
    {
        Bird bird = other.GetComponent<Bird>();
        if (null != bird)
            bird.Push(m_vel * m_force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision.gameObject);
    }
}
