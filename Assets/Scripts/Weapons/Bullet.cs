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
    public bool m_isPlayerBullet = false;

    static List<Bullet> s_theList = new List<Bullet>();
    protected Vector3 m_vel;
    protected Vector3 m_dir;
    protected float m_curSpeed;
    bool m_isOnScreen;
    Renderer m_renderer;
    float m_offScreenTimeOut;
    const float s_offScreenTimeOut = 1.0f;

    public void WarmUp()
    {
        if (null != m_impactPow)
            ObjectPool.GetPool(m_impactPow, 32);
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_curSpeed = m_speed;
        m_isOnScreen = false;
        m_offScreenTimeOut = s_offScreenTimeOut;
    }

    protected virtual void Start()
    {
        m_renderer = GetComponent<Renderer>();
        if (null == m_renderer)
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();
            Bounds biggest = new Bounds();
            foreach (Renderer render in renders)
            {
                Bounds bound = render.bounds;
                if (bound.size.sqrMagnitude > biggest.size.sqrMagnitude)
                {
                    biggest = bound;
                    m_renderer = render;
                }
            }
        }
    }

    public virtual void Fire(Vector3 dir)
    {
        m_dir = dir;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
        m_vel = m_dir * m_curSpeed;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime(m_isPlayerBullet);
        Vector3 pos = transform.position;
        pos += m_vel * dt;
        transform.position = pos;

        if (null != m_renderer)
        {   // once the bullet has appeared on screen, kill it as soon as it leaves the screen
            Bounds bounds = m_renderer.bounds;
            Bounds screenBounds = GameManager.Get().GetScreenBounds();
            if (bounds.Intersects(screenBounds))
            {   // wait for the bullet to appear on screen (in case fired from off-screen)
                m_isOnScreen = true;
            }
            else
            {
                if (m_isOnScreen)
                {   // then once it has left the screen, delete it
                    Free();
                }
                else
                {   // if a bullet is off screen for over a second, delete it
                    m_offScreenTimeOut -= dt;
                    if (m_offScreenTimeOut <= 0.0f)
                        Free();
                }
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
        if (m_isPlayerBullet)
        {
            if (damageReturn == IHitPoints.DamageReturn.KILLED)
            {
                if (m_hitType == IHitPoints.HitType.BULLET || m_hitType == IHitPoints.HitType.LASER)
                {
                    Player.ComboKill(other.transform.position);
                }
            }
        }
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
