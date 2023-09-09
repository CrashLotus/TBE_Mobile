using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : WormSection
{
    public enum WormType
    {
        WORM,
        SUPER,
        MECHA,

        TOTAL
    }
    public enum Pattern
    {
        ARC_RIGHT,
        ARC_LEFT,
        S_UP_RIGHT,
        S_UP_LEFT,
        S_DOWN_RIGHT,
        S_DOWN_LEFT,
        UP,
        DOWN,
        SKIM_LOW_RIGHT,
        SKIM_LOW_LEFT,

        DEBUG_CHASE,     // chase the player - just for debugging
    }

    public WormType m_type = WormType.WORM;
    public int m_numSection = 10;
    public float m_speed = 2.0f;
    public float m_turnSpeed = 180.0f;
    public GameObject m_midPrefab;
    public GameObject m_tailPrefab;
    public GameObject m_warning;
    public GameObject m_explosion;
    public int m_numExplode = 5;
    public float m_explodeRadius = 1.0f;
    public GameObject m_explodeBullet;
    public int m_numExplodeBullet = 6;
    public int m_score = 500;

    List<WormSection> m_sections;
    Pattern m_pattern;
    float m_tailYMin = 0.0f;
    float m_tailYMax = 0.0f;
    bool m_tailOnScreen = false;
    Vector3 m_arcCenter;
    const float s_arcWidth = 6.0f;
    const float s_arcHeight = 4.0f;

    static List<Worm> s_theList = new List<Worm>();
    static ObjectPool[] s_wormPool = new ObjectPool[(int)WormType.TOTAL];

    static readonly string[] s_wormName =
    {
        "Worm_Head",
        "SuperWorm_Head",
        "MechaWorm_Head"
    };

    public static void WarmUp()
    {
        for (int i = 0; i < (int)WormType.TOTAL; ++i)
        {
            MakeWormPool(i);
            if (null != s_wormPool[i])
            {
                Worm worm = s_wormPool[i].m_prefab.GetComponent<Worm>();
                worm._WarmUp();
            }
        }
    }

    public static Worm Spawn(Vector3 pos, WormType power, Pattern pattern)
    {
        int index = (int)power;
        if (null == s_wormPool)
        {
            MakeWormPool(index);
        }
        if (null != s_wormPool)
        {
            GameObject enemyObj = s_wormPool[index].Allocate(pos);
            if (null != enemyObj)
            {
                Worm worm = enemyObj.GetComponent<Worm>();
                worm.SpawnSections();
                switch (pattern)
                {
                    case Pattern.ARC_LEFT:
                        worm.m_arcCenter = pos;
                        worm.m_arcCenter.x -= s_arcWidth;
                        worm.m_arcCenter.y = 0.0f;
                        break;
                    case Pattern.ARC_RIGHT:
                        worm.m_arcCenter = pos;
                        worm.m_arcCenter.x += s_arcWidth;
                        worm.m_arcCenter.y = 0.0f;
                        break;
                }
                worm.BeginPattern(pattern);
                return worm;
            }
        }
        return null;
    }

    public static void MakeWormPool(int index)
    {
        if (null == s_wormPool[index])
        {
            GameObject wormPrefab = Resources.Load<GameObject>(s_wormName[index]);
            if (null == wormPrefab)
            {
                Debug.LogError("Unable to load enemy prefab " + s_wormName[index]);
                return;
            }
            s_wormPool[index] = ObjectPool.GetPool(wormPrefab, 4);
        }
    }

    public static int GetCount()
    {
        return s_theList.Count;
    }

    public static void DeleteAll()
    {
        for (int i = s_theList.Count - 1; i >= 0; --i)
        {
            s_theList[i].DeleteWorm();
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

    protected void _WarmUp()
    {
        if (null != m_midPrefab)
        {
            ObjectPool.GetPool(m_midPrefab, 40);
        }
        if (null != m_tailPrefab)
        {
            ObjectPool.GetPool(m_tailPrefab, 4);
            WormTail tail = m_tailPrefab.GetComponent<WormTail>();
            tail._WarmUp();
        }
        if (null != m_warning)
        {
            ObjectPool.GetPool(m_warning, 12);
        }
        if (null != m_explosion)
        {
            ObjectPool.GetPool(m_explosion, 64);
        }
        if (null != m_explodeBullet)
        {
            ObjectPool.GetPool(m_explodeBullet, 32);
        }
    }

    void DeleteWorm()
    {
        foreach (WormSection section in m_sections)
        {
            section.Free();
        }
        m_sections.Clear();
        Free();
    }

    void SpawnSections()
    { 
        WormSection parent = this;
        m_sections = new List<WormSection>();
        float animTime = 0.0f;
        for (int i = 0; i < m_numSection; ++i)
        {
            ObjectPool pool;
            if (i < m_numSection - 1)
                pool = ObjectPool.GetPool(m_midPrefab, 64);
            else
                pool = ObjectPool.GetPool(m_tailPrefab, 64);
            if (null != pool)
            {
                GameObject section = pool.Allocate(transform.position);
                WormSection worm = section.GetComponent<WormSection>();
                Vector3 pos = parent.GetTailPos() - transform.TransformDirection(worm.m_headJoint);
                pos.z += 0.1f;
                section.transform.position = pos;
                Animator anim = section.GetComponent<Animator>();
                if (null != anim)
                {
                    anim.Play("Loop", -1, animTime);
                }
                worm.Init(null);
                m_sections.Add(worm);
                parent = worm;
                animTime += 0.2f;
            }
        }

        // connect the sections
        parent = this;
        SetHead(this);       
        SetNext(m_sections[0]);
        SetPrev(null);
        WormSection next;
        for (int i = 0; i < m_sections.Count; ++i)
        { 
            WormSection worm = m_sections[i];
            if (i < m_sections.Count - 1)
                next = m_sections[i + 1];
            else
                next = null;
            worm.SetHead(this);
            worm.SetNext(next);
            worm.SetPrev(parent);
            parent = worm;
        }
    }

    void BeginPattern(Pattern pattern)
    {
        m_arcCenter = WrapAround.WrapPosition(m_arcCenter);
        bool flipY = false;
        float ang = 0.0f;
        switch (pattern)
        {
            case Pattern.ARC_LEFT:
                {
                    ang = 90.0f;
                    flipY = true;
                    Vector3 pos = m_arcCenter;
                    pos.x += s_arcWidth;
                    pos.y = GameManager.Get().GetLavaHeight() - 3.0f;
                    pos.z = 0.1f;
                    transform.position = pos;
                    Warning(pos);
                }
                break;
            case Pattern.ARC_RIGHT:
                {
                    ang = 90.0f;
                    flipY = false;
                    Vector3 pos = m_arcCenter;
                    pos.x -= s_arcWidth;
                    pos.y = GameManager.Get().GetLavaHeight() - 3.0f;
                    pos.z = 0.1f;
                    transform.position = pos;
                    Warning(pos);
                }
                break;
            case Pattern.UP:
                {
                    ang = 90.0f;
                    flipY = false;
                    Vector3 pos = m_arcCenter;
                    pos.y = GameManager.Get().GetLavaHeight() - 3.0f;
                    pos.z = 0.1f;
                    transform.position = pos;
                    Warning(pos);
                }
                break;
            case Pattern.DOWN:
                {
                    ang = -90.0f;
                    flipY = false;
                    Vector3 pos = m_arcCenter;
                    pos.y = GameManager.Get().GetScreenBounds().max.y + 1.0f;
                    pos.z = 0.1f;
                    transform.position = pos;
                }
                break;
        }

        // update the positions of the sections
        WormSection parent = this;
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
        if (null != m_sprite)
            m_sprite.flipY = flipY;
        foreach (WormSection worm in m_sections)
        {
            Vector3 pos = parent.GetTailPos() - parent.transform.TransformDirection(worm.m_headJoint);
            pos.z += 0.1f;
            worm.transform.position = pos;
            worm.transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
            SpriteRenderer wormSprite = worm.GetComponent<SpriteRenderer>();
            if (null != wormSprite)
                wormSprite.flipY = flipY;
            parent = worm;
        }

        m_pattern = pattern;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime(false);

        Pattern doPattern = m_pattern;

        Vector3 pos = transform.position;
        float ang = transform.localEulerAngles.z;
        float targetAng = ang;
        Vector3 dir = transform.right;
        bool isDone = false;

        switch (doPattern)
        {
            case Pattern.ARC_LEFT:
                {
                    Vector3 forecast = pos + 0.25f * m_speed * dir;
                    Vector3 offset = forecast - m_arcCenter;
                    targetAng = Mathf.Atan2(offset.y * s_arcWidth / s_arcHeight, offset.x);
                    Vector3 targetPos = m_arcCenter;
                    if (targetAng < -0.5f * Mathf.PI)
                    {
                        targetPos.x -= s_arcWidth;
                        targetPos.y = pos.y - 0.25f * m_speed;
                        if (false == m_tailOnScreen)
                            isDone = true;
                    }
                    else if (targetAng < 0.0f)
                    {
                        targetPos.x += s_arcWidth;
                        targetPos.y = pos.y + 0.25f * m_speed;
                    }
                    else
                    {
                        targetPos.x += s_arcWidth * Mathf.Cos(targetAng);
                        targetPos.y += s_arcHeight * Mathf.Sin(targetAng);
                    }
                    offset = targetPos - pos;
                    targetAng = Mathf.Atan2(offset.y, offset.x);
                    targetAng = Mathf.Rad2Deg * targetAng;
                }
                break;
            case Pattern.ARC_RIGHT:
                {
                    Vector3 forecast = pos + 0.25f * m_speed * dir;
                    Vector3 offset = forecast - m_arcCenter;
                    targetAng = Mathf.Atan2(offset.y * s_arcWidth / s_arcHeight, offset.x);
                    Vector3 targetPos = m_arcCenter;
                    if (targetAng < -0.5f * Mathf.PI)
                    {
                        targetPos.x -= s_arcWidth;
                        targetPos.y = pos.y + 0.25f * m_speed;
                    }
                    else if (targetAng < 0.0f)
                    {
                        targetPos.x += s_arcWidth;
                        targetPos.y = pos.y - 0.25f * m_speed;
                        if (false == m_tailOnScreen)
                            isDone = true;
                    }
                    else
                    {
                        targetPos.x += s_arcWidth * Mathf.Cos(targetAng);
                        targetPos.y += s_arcHeight * Mathf.Sin(targetAng);
                    }
                    offset = targetPos - pos;
                    targetAng = Mathf.Atan2(offset.y, offset.x);
                    targetAng = Mathf.Rad2Deg * targetAng;
                }
                break;
            case Pattern.UP:
                {
                    targetAng = 90.0f;
                    if (m_tailYMin > 1.1f)
                        isDone = true;
                }
                break;
            case Pattern.DOWN:
                {
                    targetAng = -90.0f;
                    if (m_tailYMax < -0.1f)
                        isDone = true;
                }
                break;
            case Pattern.DEBUG_CHASE:
                {
                    Player player = Player.Get();
                    if (null != player)
                    {
                        Vector3 diff = player.transform.position - transform.position;
                        targetAng = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);
                    }
                }
                break;
        }

        // steer towards target angle
        targetAng -= ang;
        if (targetAng < -180.0f)
            targetAng += 360.0f;
        if (targetAng > 180.0f)
            targetAng -= 360.0f;
        targetAng = Mathf.Clamp(targetAng, -m_turnSpeed * dt, m_turnSpeed * dt);
        ang += targetAng;
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);

        // move forward
        dir = transform.right;
        pos += m_speed * dir * dt;
        transform.position = pos;

        // Update the sections
        WormSection parent = this;
        foreach (WormSection section in m_sections)
        {
            section.UpdatePosition(parent);
            parent = section;
        }

        if (isDone)
        {
            switch (m_pattern)
            {
                case Pattern.ARC_RIGHT:
                    if (WormType.SUPER == m_type)
                        BeginPattern(Pattern.UP);
                    else
                        BeginPattern(Pattern.ARC_LEFT);
                    break;
                case Pattern.ARC_LEFT:
                    BeginPattern(Pattern.ARC_RIGHT);
                    break;
                case Pattern.UP:
                    BeginPattern(Pattern.DOWN);
                    break;
                case Pattern.DOWN:
                    BeginPattern(Pattern.ARC_RIGHT);
                    break;
            }
        }

        // is the tail on screen?
        WormSection tail = m_sections[m_sections.Count - 1];
        SpriteRenderer tailSprite = tail.GetSprite();
        Vector3 tailMin = Camera.main.WorldToViewportPoint(tailSprite.bounds.min);
        m_tailYMin = tailMin.y;
        Vector3 tailMax = Camera.main.WorldToViewportPoint(tailSprite.bounds.max);
        m_tailYMax = tailMax.y;
        if (m_tailYMax < 0.0f)
            m_tailOnScreen = false;
        else if (m_tailYMin > 1.0f)
            m_tailOnScreen = false;
        else
            m_tailOnScreen = true;
    }

    public IHitPoints.DamageReturn HeadDamage(int damage, IHitPoints.HitType hitType)
    {
        if (m_sections.Count > 1)
        {   // pass the damage down to the next section
            return m_sections[0].Damage(damage, hitType);
        }
        // we're out of sections...
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

    protected override void Explode()
    {
        foreach (WormSection section in m_sections)
        {
            ExplodeEffect(section.transform.position, section.transform.eulerAngles.z);
            Player.AddScore(m_score);
            section.Free();
        }
        ExplodeEffect(transform.position, transform.eulerAngles.z);
        TimeCrystal.Spawn(transform.position);
        Player.AddScore(m_score);
        FollowCamera.Shake(10.0f, 1.0f);
        Free();
    }

    protected void ExplodeEffect(Vector3 pos, float rot = 0.0f)
    {
        for (int i = 0; i < m_numExplode; ++i)
        {
            Vector2 offset = m_explodeRadius * Random.insideUnitCircle;
            Vector3 explodePos = new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z - 0.1f);
            ObjectPool.Allocate(m_explosion, 64, explodePos);
        }
        if (null != m_explodeBullet)
        {
            float s_angPer = 2.0f * Mathf.PI / m_numExplodeBullet;
            float ang = Mathf.Deg2Rad * (rot + 90.0f) + 0.5f * s_angPer;
            for (int i = 0; i < m_numExplodeBullet; ++i)
            {
                Vector3 dir = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0.0f);
                Vector3 objPos = pos + 0.4f * dir;
                GameObject obj = ObjectPool.Allocate(m_explodeBullet, 32, objPos);
                if (null != obj)
                {
                    Bullet bullet = obj.GetComponent<Bullet>();
                    if (null != bullet)
                    {
                        bullet.Fire(dir);
                    }
                }
                ang += s_angPer;
            }
        }
    }

    protected void Warning(Vector3 pos)
    {
        FollowCamera.Shake(8.0f, 2.0f);
        if (null != m_warning)
        {
            pos.y = GameManager.Get().GetLavaHeight();
            pos.z = -1.1f;
            ObjectPool pool = ObjectPool.GetPool(m_warning, 12);
            if (null != pool)
                pool.Allocate(pos);
        }
    }

    public void SectionDestroyed(WormSection section)
    {
        WormSection prev = this;
        for (int i = 0; i < m_sections.Count; i++)
        {
            WormSection worm = m_sections[i];
            if (section == worm)
            {   // this is it
                WormSection next = null;
                if (i < m_sections.Count - 1)
                    next = m_sections[i + 1];
                prev.SetNext(next);
                if (null != next)
                    next.SetPrev(prev);
                m_sections.RemoveAt(i);
                ExplodeEffect(section.transform.position, section.transform.eulerAngles.z);
                Player.AddScore(m_score);
                section.Free();
                return;
            }
            prev = worm;
        }
    }

    public static void ProcessEachSegment(GameManager.ProcessObject process)
    {
        foreach (Worm worm in s_theList)
        {
            foreach (WormSection section in worm.m_sections)
                process(section.gameObject);
            process(worm.gameObject);
        }
    }
}
