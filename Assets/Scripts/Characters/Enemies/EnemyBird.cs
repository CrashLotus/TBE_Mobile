using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBird : Bird, IHitPoints
{
    public Weapon m_weapon;
    public float m_fireDelay = 5.0f;
    public float m_jukeFreq = 1.0f;
    public int m_eggPower = 1;
    public int m_score;
    public GameObject m_spawnEffect;

    const float s_chaseDist = 100.0f;
    const float s_minPlayerDist = 2.0f;
    const float s_repulseForce = 0.75f;
    const float s_repulseForceEgg = 2.25f;
    const float s_maxPush = 0.5f;
    protected const float s_wanderSpeed = 0.4f;
    protected const float s_centerForce = 0.4f;
    protected const float s_centerSpeed = 0.9f;
    const float s_fireDelayMin = 2.0f;
    const float s_fireDelayMax = 5.0f;
    protected const float s_wanderJuke = 1.0f;
    const float s_riseHeight = 0.2f;
    const float s_fallHeight = -3.0f;
    const float s_invTime = 1.0f;
    protected const float s_jukeAmp = 1.0f;
    static List<EnemyBird> s_theList = new List<EnemyBird>();
    static ObjectPool[] s_enemyPool = new ObjectPool[3];

    protected enum State
    {
        RISING,
        FALLING,
        FLYING,
        CUSTOM
    };
    protected Vector3 m_repulse = Vector3.zero;
    protected float m_jukeTimer = 0.0f;
    protected float m_invTimer;
    protected State m_state;

    static readonly string[] s_enemyName =
    {
        "Enemy01",
        "Enemy02",
        "Enemy03"
    };

    public static void WarmUp()
    {
        for (int i = 0; i < 3; ++i)
        {
            MakeEnemyPool(i + 1);
            EnemyBird bird = s_enemyPool[i].m_prefab.GetComponent<EnemyBird>();
            bird._WarmUp();
        }
    }

    protected override void _WarmUp()
    {
        base._WarmUp();
        ObjectPool.GetPool(m_spawnEffect, 16);
        if (null != m_weapon)
            m_weapon.WarmUp();
    }

    public static void MakeEnemyPool(int power)
    {
        int index = power - 1;
        if (null == s_enemyPool[index])
        {
            GameObject enemyPrefab = Resources.Load<GameObject>(s_enemyName[index]);
            if (null == enemyPrefab)
            {
                Debug.LogError("Unable to load enemy prefab " + s_enemyName[index]);
                return;
            }
            s_enemyPool[index] = ObjectPool.GetPool(enemyPrefab, 50);
        }
    }

    public static EnemyBird Spawn(Vector3 pos, int power)
    {
        int index = power - 1;
        if (null == s_enemyPool[index])
        {
            MakeEnemyPool(power);
        }
        if (null != s_enemyPool[index])
        {
            GameObject enemyObj = s_enemyPool[index].Allocate(pos);
            if (null != enemyObj)
            {
                EnemyBird enemy = enemyObj.GetComponent<EnemyBird>();
                if (null != enemy.m_spawnEffect)
                {
                    pos.z -= 1.0f;  // sort the flames to the front
                    ObjectPool pool = ObjectPool.GetPool(enemy.m_spawnEffect, 16);
                    if (null != pool)
                        pool.Allocate(pos);
                }
                return enemy;
            }
        }
        return null;
    }
    public static int GetCount()
    {
        return s_theList.Count;
    }

    public static void DeleteAll()
    {
        for (int i = s_theList.Count - 1; i >= 0; --i)
        {
            s_theList[i].Free();
        }
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_jukeFreq = Random.Range(0.6f, 1.4f) * m_jukeFreq;
        m_fireDelay = Random.Range(s_fireDelayMin, s_fireDelayMax);
        m_repulse = Vector3.zero;
        m_invTimer = s_invTime;
        GetComponent<Collider2D>().enabled = false;
        Vector3 pos = transform.position;
        pos = Camera.main.WorldToViewportPoint(pos);
        if (pos.y > 1.0f)
            m_state = State.FALLING;
        else
            m_state = State.RISING;
    }

    private void OnEnable()
    {
        s_theList.Add(this);
    }

    private void OnDisable()
    {
        s_theList.Remove(this);
    }

    protected override void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime(false);

        m_fireDelay -= dt;
        if (m_fireDelay <= 0.0f)
            m_fireDelay = 0.0f;

        if (m_invTimer > 0.0f)
        {
            m_invTimer -= Time.unscaledDeltaTime;
            if (m_invTimer <= 0.0f)
            {
                m_invTimer = 0.0f;
                GetComponent<Collider2D>().enabled = true;
            }
        }

        // find the player
        Player player = Player.Get();

        Vector3 pos = transform.position;

        if (State.CUSTOM > m_state)
        {
            Vector3 move = Vector3.zero;
            bool doWander = true;

            if (null != player)
            {
                Vector3 delta = player.transform.position + Juke(dt) - pos;
                if (Mathf.Abs(delta.x) <= s_chaseDist)
                    doWander = false;
                if (delta.x > 0.0f)
                {   // player is on my right
                    delta.x -= s_minPlayerDist;
                    m_sprite.flipX = false;
                }
                else
                {   // player is on my left
                    delta.x += s_minPlayerDist;
                    m_sprite.flipX = true;
                }

                if (State.FLYING == m_state)
                {
                    float length = delta.magnitude;
                    if (length > m_horizSpeed * dt)
                    {
                        move = delta / length;
                        move.x *= m_horizSpeed * dt;
                        move.y *= m_vertSpeed * dt;
                    }
                    else
                    {
                        move = delta;
                        move.y *= m_vertSpeed / m_horizSpeed;
                    }

                    float avoidLava = pos.y - GameManager.Get().GetLavaHeight();
                    avoidLava = s_riseHeight - avoidLava;
                    if (avoidLava > 0.0f)
                    {
                        avoidLava = avoidLava / s_riseHeight;
                        move.y += m_vertSpeed * avoidLava * dt;
                    }
                }
            }

            if (State.RISING == m_state)
            {
                move.y = m_vertSpeed * dt;
                if (pos.y > GameManager.Get().GetLavaHeight() + s_riseHeight)
                    m_state = State.FLYING;
            }
            else if (State.FALLING == m_state)
            {
                move.y = -m_vertSpeed * dt;
                if (pos.y < s_fallHeight)
                    m_state = State.FLYING;
            }

            pos += move;

            if (doWander)
            {
                pos = DoWander(pos, dt);
            }
            else
            {
                pos += m_repulse * dt;
                if (null != m_weapon && m_fireDelay <= 0.0f)
                    m_weapon.HoldTrigger();
            }
        }

        if (State.FLYING == m_state)
        {
            // constrain the enemy to the bottom of the screen
            Vector3 botRight = new Vector3(1.0f, 0.0f, 0.0f);   // bottom-right corner in view coords
            botRight = Camera.main.ViewportToWorldPoint(botRight);    // converted to world coords
            pos.y = Mathf.Max(pos.y, botRight.y);
        }

        transform.position = pos;
        m_repulse = Vector3.zero;

        base.Update();
    }

    protected Vector3 Juke(float dt)
    {
        m_jukeTimer += dt;
        // figure 8
        Vector3 juke = new Vector3((float)Mathf.Sin(2.0f * m_jukeFreq * m_jukeTimer),
            (float)Mathf.Sin(m_jukeFreq * m_jukeTimer), 0.0f);
        juke *= s_jukeAmp;
        return juke;
    }

    protected virtual Vector3 DoWander(Vector3 pos, float dt)
    {
        Vector3 push = m_repulse;
        if (push.sqrMagnitude > 0.0001f)
            push.Normalize();
        push *= s_wanderSpeed;
        float centerForce = (Camera.main.transform.position.y - pos.y) * s_centerForce;
        centerForce = Mathf.Clamp(centerForce, -s_centerSpeed, s_centerSpeed);
        push.y += centerForce;
        pos += push * dt;
        pos += Juke(dt) * s_wanderJuke * dt;
        return pos;
    }

    public static void DoRepulse(float dt)
    {
        int birdCount = s_theList.Count;
        for (int i = 0; i < birdCount - 1; ++i)
        {
            EnemyBird bird1 = s_theList[i];
            Vector3 pos1 = bird1.transform.position;
            for (int j = i + 1; j < birdCount; ++j)
            {
                EnemyBird bird2 = s_theList[j];
                Vector3 pos2 = bird2.transform.position;
                // get delta to each other bird
                Vector3 delta = pos1 - pos2;
                // get distance squared
                float distSq = delta.sqrMagnitude;
                if (distSq > 0.0001f)
                {
                    delta.Normalize();
                }
                else
                {   // he's right on top of you... just push on the x axis to separate
                    delta = Vector3.right;
                    distSq = 0.0001f;
                }
                float push = Mathf.Min(s_repulseForce / distSq, s_maxPush);
                bird1.m_repulse += delta * push;
                bird2.m_repulse -= delta * push;
            }

            foreach (Egg egg in Egg.GetList())
            {
                // get delta to each egg
                Vector3 delta = pos1 - egg.transform.position;
                // get distance squared
                float distSq = delta.sqrMagnitude;
                if (distSq > 0.0001f)
                {
                    delta.Normalize();
                }
                else
                {   // he's right on top of you... just push on the x axis to separate
                    delta = Vector3.right;
                    distSq = 0.0001f;
                }
                float push = Mathf.Min(s_repulseForceEgg / distSq, s_maxPush);
                bird1.m_repulse += delta * push;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_invTimer <= 0.0f)
        {
            IHitPoints hit = collision.gameObject.GetComponent<IHitPoints>();
            if (null != hit)
            {
                hit.Damage(m_hitPoints, IHitPoints.HitType.NONE);
                Damage(m_hitPoints, IHitPoints.HitType.NONE);
            }
        }
    }

    public IHitPoints.DamageReturn Damage(int damage, IHitPoints.HitType hitType)
    {
        if (m_invTimer > 0.0f)
            return IHitPoints.DamageReturn.NO_DAMAGE;
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

    void Explode()
    {
        ObjectPool.Allocate(m_deathEffect, 64, transform.position);
        if (null != m_deathSound)
            m_deathSound.Play();
        Player.AddScore(m_score);
        Egg.Spawn(transform.position, m_eggPower);
        TutorialManager.Get().EnemyKilled(transform.position);
        Free();
    }

    public static void ProcessAll(GameManager.ProcessObject process)
    {
        foreach (EnemyBird bird in s_theList)
            process(bird.gameObject);
    }
}
