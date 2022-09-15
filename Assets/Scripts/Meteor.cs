using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : PooledObject
{
    public GameObject m_ball;
    public GameObject m_flame;
    public GameObject m_explode;
    public Sound m_spawnSound;

    static ObjectPool s_meteorPool;
    static List<Meteor> s_theList = new List<Meteor>();

    const float s_spawnSpeed = 4.9f;
    const float s_spawnAng = 20.0f;
    const float s_ballScaleMin = 0.9f;
    const float s_ballScaleMax = 1.3f;
    const float s_damage = 3.0f;
    const float s_ballRotSpd = 360.0f;
    const float s_lavaHitOffset = 0.12f;
    const float s_animSpeedMin = 10.0f;
    const float s_animSpeedMax = 20.0f;

    Vector3 m_vel;
    bool m_hitPlayer;   //true if I've already hit the player
    float m_ballRot;
    float m_ballRotSpd;

    public static void MakeMeteorPool()
    {
        if (null == s_meteorPool)
        {
            GameObject meteorPrefab = Resources.Load<GameObject>("Meteor");
            s_meteorPool = ObjectPool.GetPool(meteorPrefab, 64);
            Meteor meteor = meteorPrefab.GetComponent<Meteor>();
            if (null != meteor && null != meteor.m_explode)
                ObjectPool.GetPool(meteor.m_explode, 64);
        }
    }

    public static void Spawn(Vector3 pos)
    {
        if (null == s_meteorPool)
        {
            MakeMeteorPool();
        }
        if (null != s_meteorPool)
        {
            s_meteorPool.Allocate(pos);
        }
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);

        float rot = Random.Range(-s_spawnAng, s_spawnAng);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, rot);
        rot = rot * Mathf.Deg2Rad;
        m_vel = new Vector3(Mathf.Sin(rot), -Mathf.Cos(rot), 0.0f) * s_spawnSpeed;
        m_hitPlayer = false;

        Animator anim = GetComponent<Animator>();
        if (null != anim)
            anim.speed = Random.Range(s_animSpeedMin, s_animSpeedMax);
        m_ballRot = Random.Range(-180.0f, 180.0f);
        m_ballRotSpd = Random.Range(-s_ballRotSpd, s_ballRotSpd);
        float ballScale = Random.Range(s_ballScaleMin, s_ballScaleMax);
        m_ball.transform.localScale = ballScale * Vector3.one;
        m_ball.transform.localEulerAngles = new Vector3(0.0f, 0.0f, m_ballRot);
        if (m_spawnSound)
            m_spawnSound.Play();
#if false   //mrwTODO
        for (int i = 0; i < s_numMini; ++i)
        {
            Vector2 miniPos = m_pos + (i + 1 - 0.5f * s_numMini) / (float)s_numMini * s_miniLeadTime * m_vel;
            float ang = MathHelper.ToRadians(RandomHelper.Range(-180.0f, 180.0f));
            miniPos += new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * s_miniSpread;
            ang = MathHelper.ToRadians(RandomHelper.Range(-180.0f, 180.0f));
            Vector2 miniVel = m_vel + new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * s_miniVelSpread;
            s_miniList.Add(new Mini(miniPos, m_rot, miniVel));
        }
        Camera.Shake(1.0f, 2.0f);
#endif
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector3 pos = transform.position;
        pos += m_vel * dt;

        m_ballRot += m_ballRotSpd * dt;
        m_ballRot = Utility.WrapAngleDeg(m_ballRot);
        transform.position = pos;
        m_ball.transform.localEulerAngles = new Vector3(0.0f, 0.0f, m_ballRot);

        if (pos.y < GameManager.Get().GetLavaHeight() + s_lavaHitOffset)
        {
            Explode();
        }
    }
    public void Explode()
    {
        //        Camera.Shake(8.0f, 1.0f);
        if (null != m_explode)
        {
            ObjectPool pool = ObjectPool.GetPool(m_explode, 64);
            if (null != pool)
                pool.Allocate(transform.position);
        }
        Free();
    }

    private void OnEnable()
    {
        s_theList.Add(this);
    }

    private void OnDisable()
    {
        s_theList.Remove(this);
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
}
