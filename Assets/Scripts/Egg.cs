using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : PooledObject, IHitPoints
{
    public Sprite[] m_sprites;
    public Sound m_collectSound;
    public Sound m_spawnSound;

    static ObjectPool s_eggPool;
    static List<Egg> s_theList = new List<Egg>();
    static int s_numHitLava = 0;
    static int s_numJuggle = 0;
    const float s_animSpeedMin = 20.0f;
    const float s_animSpeedMax = 60.0f;
    static float s_spawnSpeed = 1.8f;
    const float s_spawnAng = 45.0f;
    static float s_gravity = 1.7f;
    const float s_minPickUpDelay = 0.5f;
    const int s_score = 50;
    const float s_spinTime = 0.5f;
    const float s_wobbleTime = 2.0f;
    const float s_hudFlyTime = 0.3f;
    static readonly Vector2 s_magnetOffset = new Vector2(85.0f, 87.5f);

    enum State
    {
        IDLE,
        SPIN,
        WOBBLE,
        FLY_TO_HUD,
    }

    Vector3 m_vel = Vector3.zero;
    float m_pickUpTimer;
    int m_power;
    State m_state = State.IDLE;
    float m_stateTimer = 0.0f;
    Vector3 m_startPos;
    Vector3 m_hudPos;
    float m_startRot;
    int m_numJuggle = 0;
    //mrwTODO add magnet sprite
    float m_magnetPower = 0.0f;

    public static void MakeEggPool()
    {
        if (null == s_eggPool)
        {
            GameObject eggPrefab = Resources.Load<GameObject>("Egg");
            s_eggPool = ObjectPool.GetPool(eggPrefab, 100);
        }
    }

    static Egg MakeEgg(Vector3 pos, int power)
    {
        if (null == s_eggPool)
        {
            MakeEggPool();
        }
        if (null != s_eggPool)
        {
            GameObject eggObj = s_eggPool.Allocate(pos);
            if (null != eggObj)
            {
                Egg egg = eggObj.GetComponent<Egg>();
                egg.m_power = power;
                SpriteRenderer sprite = egg.GetComponent<SpriteRenderer>();
                sprite.sprite = egg.m_sprites[power - 1];
                if (null != egg.m_spawnSound)
                    egg.m_spawnSound.Play();
                return egg;
            }
        }
        return null;
    }

    public static void Spawn(Vector3 pos, int power, float upSpeed)
    {
        while (power > 3)
        {
            Egg egg = MakeEgg(pos, 3);
            egg.PopUp(upSpeed);
            power -= 3;
        }
        if (power > 0)
        {
            Egg egg = MakeEgg(pos, power);
            egg.PopUp(upSpeed);
        }
    }

    public static void Spawn(Vector3 pos, int power)
    {
        Spawn(pos, power, s_spawnSpeed);
    }

    public static void Spawn(Vector2 pos, int power, Vector2 vel)
    {
        Egg egg = MakeEgg(pos, power);
        if (null != egg)
            egg.m_vel = vel;
    }

    public static int GetNumHitLava()
    {
        return s_numHitLava;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        m_magnetPower = 0.0f;
        if (State.FLY_TO_HUD == m_state)
        {
            UpdateFlyToHud(Time.unscaledDeltaTime);
        }
        else
        {
            Player player = Player.Get();
            m_pickUpTimer -= Time.unscaledDeltaTime;
            Vector3 pos = transform.position;
            //mrwTODO
            //if (m_pickUpTimer <= 0.0f)
            //{
            //    if (null != player && Overlaps(player))
            //    {
            //        Player.AddScore(s_score);
            //        player.AddEgg();
            //        m_hudPos = EggIcon.GetPos();
            //        AudioComponent.Get().PlaySound("eggCollect");
            //        SetState(State.FLY_TO_HUD);
            //        return;
            //    }
            //    m_pickUpTimer = 0.0f;
            //}

            //mrwTODO
            //if (null != player)
            //{
            //    int magnetLevel = Player.GetEggMagnetLevel();
            //    if (magnetLevel > 0)
            //    {
            //        float[] s_magnetRange = { 0.0f, 200.0f, 400.0f };
            //        float[] s_magnetPower = { 0.0f, 500.0f, 1000.0f };
            //        Vector2 delta = m_pos - player.GetPos();
            //        float dist = delta.Length();
            //        if (dist < s_magnetRange[magnetLevel] && dist > 1.0f)
            //        {
            //            m_magnetPower = 1.0f - dist / s_magnetRange[magnetLevel];
            //            float magnetSpd = m_magnetPower * s_magnetPower[magnetLevel];
            //            float damp = m_magnetPower * 0.05f * dt * 60.0f;
            //            m_vel -= m_vel * damp;
            //            m_vel -= magnetSpd * delta / dist * dt;
            //        }
            //    }
            //}
            m_vel.y -= s_gravity * dt;
            pos += m_vel * dt;

            if (pos.y < GameManager.Get().GetLavaHeight())
            {
                Free();
                HitLava();
            }
            else
            {
                switch (m_state)
                {
                    case State.SPIN:
                        m_stateTimer -= dt;
                        transform.localEulerAngles = 
                            new Vector3(0.0f, 0.0f, 360.0f * m_stateTimer / s_spinTime);
                        if (m_stateTimer <= 0.0f)
                            SetState(State.WOBBLE);
                        break;
                    case State.WOBBLE:
                        transform.localEulerAngles = 
                            new Vector3(0.0f, 0.0f, Mathf.Rad2Deg * m_stateTimer * Mathf.Sin(Mathf.Deg2Rad * 360.0f * m_stateTimer));
                        m_stateTimer -= dt;
                        if (m_stateTimer <= 0.0f)
                            SetState(State.IDLE);
                        break;
                }
            }

            transform.position = pos;
        }

        //m_magnet.Update(dt);
    }

    void UpdateFlyToHud(float dt)
    {
        m_stateTimer -= dt;
        float lerp = m_stateTimer / s_hudFlyTime;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(m_hudPos);
        transform.position = Vector3.Lerp(targetPos, m_startPos, lerp);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(0.0f, m_startRot, lerp));
        //        m_scale = MathHelper.Lerp(EggIcon.GetScale(), 1.0f, lerp);    //mrwTODO
        if (m_stateTimer <= 0.0f)
        {
            Free();
        }
    }

    public static int GetCount()
    {
        return s_theList.Count;
    }

    public static List<Egg> GetList()
    {
        return s_theList;
    }

    protected void PopUp(float power)
    {
        float ang = Mathf.Deg2Rad * Random.Range(-s_spawnAng, s_spawnAng);
        m_vel = new Vector3(Mathf.Sin(ang), Mathf.Cos(ang), 0.0f) * power;
    }

    void SetState(State newState)
    {
        switch (newState)
        {
            case State.SPIN:
                m_stateTimer = s_spinTime;
                break;
            case State.WOBBLE:
                m_stateTimer = s_wobbleTime;
                break;
            case State.FLY_TO_HUD:
                {
                    m_stateTimer = s_hudFlyTime;
                    m_startPos = transform.position;
                    m_startRot = transform.localEulerAngles.z;
                }
                break;
        }

        m_state = newState;
    }

    protected virtual void HitLava()
    {
        EnemyBird enemy = EnemyBird.Spawn(transform.position, m_power);
        if (null != enemy)
        {
            ++s_numHitLava;
        }
    }

    public IHitPoints.DamageReturn Damage(float damage, IHitPoints.HitType hitType)
    {
        if (State.SPIN != m_state)
        {
            ++m_numJuggle;
            ++s_numJuggle;
            PopUp(s_spawnSpeed);
            SetState(State.SPIN);
            return IHitPoints.DamageReturn.DAMAGED;
        }
        return IHitPoints.DamageReturn.PASS_THROUGH;
    }

    private void OnEnable()
    {
        s_theList.Add(this);
    }

    private void OnDisable()
    {
        s_theList.Remove(this);
    }

    override public void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_pickUpTimer = s_minPickUpDelay;
        m_vel = Vector3.zero;
        m_state = State.IDLE;
        m_stateTimer = 0.0f;
        m_numJuggle = 0;
        m_magnetPower = 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State.FLY_TO_HUD != m_state)
        {
            if (m_pickUpTimer <= 0.0f)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (null != player && player == Player.Get())
                {
                    if (null != m_collectSound)
                        m_collectSound.Play();
                    Player.AddScore(s_score);
                    m_hudPos = HitPoint_UI.Get().GetEggPos();
                    player.AddEgg();
                    SetState(State.FLY_TO_HUD);
                }
            }
        }
    }
}
