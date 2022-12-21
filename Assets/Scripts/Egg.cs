using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : PickUp
{
    public Sprite[] m_sprites;

    static ObjectPool s_eggPool;
    static List<Egg> s_theList = new List<Egg>();
    static int s_numHitLava = 0;
    const int s_score = 50;
    const float s_hudFlyTime = 0.3f;
    static readonly Vector2 s_magnetOffset = new Vector2(85.0f, 87.5f);

    int m_power;
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

    public static void Spawn(Vector3 pos, int power, Vector3 vel)
    {
        Egg egg = MakeEgg(pos, power);
        if (null != egg)
            egg.m_vel = vel;
    }

    public static int GetNumHitLava()
    {
        return s_numHitLava;
    }

    protected override void Update()
    {
        base.Update();
        if (State.FLY_TO_HUD != m_state)
        {
            m_magnetPower = 0.0f;
            //mrwTODO
//            Vector3 pos = transform.position;
            //            float dt = Time.deltaTime;
            //            Player player = Player.Get();
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
//            transform.position = pos;
        }

        //m_magnet.Update(dt);
    }

    protected override void UpdateFlyToHud(float dt)
    {
        base.UpdateFlyToHud(dt);
        m_stateTimer -= dt;
        float lerp = m_stateTimer / s_hudFlyTime;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(m_hudPos);
        transform.position = Vector3.Lerp(targetPos, m_startPos, lerp);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(0.0f, m_startRot, lerp));
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

    protected override void SetState(State newState)
    {
        base.SetState(newState);
        switch (newState)
        {
            case State.FLY_TO_HUD:
                {
                    m_stateTimer = s_hudFlyTime;
                }
                break;
        }
    }

    protected override void HitLava()
    {
        base.HitLava();
        EnemyBird enemy = EnemyBird.Spawn(transform.position, m_power);
        if (null != enemy)
        {
            ++s_numHitLava;
        }
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

    override public void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_magnetPower = 0.0f;
    }

    protected override void PickedUp(Player player)
    {
        base.PickedUp(player);
        Player.AddScore(s_score);
        m_hudPos = HitPoint_UI.Get().GetEggPos();
        player.AddEgg();
    }
}
