using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : WormSection
{
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

    public int m_numSection = 10;
    public float m_speed = 2.0f;
    public float m_turnSpeed = 180.0f;
    public GameObject m_midPrefab;
    public GameObject m_tailPrefab;

    List<WormSection> m_sections;
    Pattern m_pattern;
    bool m_tailOnScreen = false;
    Vector3 m_arcCenter;
    const float s_arcWidth = 6.5f;
    const float s_arcHeight = 4.5f;

    // Start is called before the first frame update
    void Start()
    {
        Init(null);
        SpawnSections();
#if false   //ARC_LEFT
        m_arcCenter = transform.position;
        m_arcCenter.x -= s_arcWidth;
        m_arcCenter.y = 0.0f;
        BeginPattern(Pattern.ARC_LEFT);
#else       //ARC_RIGHT
        m_arcCenter = transform.position;
        m_arcCenter.x += s_arcWidth;
        m_arcCenter.y = 0.0f;
        BeginPattern(Pattern.ARC_RIGHT);
#endif
    }

    void SpawnSections()
    { 
        WormSection parent = this;
        m_sections = new List<WormSection>();
        float animTime = 0.0f;
        for (int i = 0; i < m_numSection; ++i)
        {
            GameObject section;
            if (i < m_numSection - 1)
                section = Instantiate(m_midPrefab);
            else
                section = Instantiate(m_tailPrefab);
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

    void BeginPattern(Pattern pattern)
    {
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
        float dt = Time.deltaTime;

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
                    BeginPattern(Pattern.ARC_LEFT);
                    break;
                case Pattern.ARC_LEFT:
                    BeginPattern(Pattern.ARC_RIGHT);
                    break;
            }
        }

        // is the tail on screen?
        WormSection tail = m_sections[m_sections.Count - 1];
        SpriteRenderer tailSprite = tail.GetSprite();
        Vector3 tailMin = Camera.main.WorldToViewportPoint(tailSprite.bounds.min);
        Vector3 tailMax = Camera.main.WorldToViewportPoint(tailSprite.bounds.max);
        if (tailMax.y < 0.0f)
            m_tailOnScreen = false;
        else if (tailMin.y > 1.0f)
            m_tailOnScreen = false;
        else
            m_tailOnScreen = true;
    }
}
