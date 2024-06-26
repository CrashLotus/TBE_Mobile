//----------------------------------------------------------------------------------------
//	Copyright � 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : Weapon
{
    public Sound m_failSound;
    public Sound m_missileAppearSound;
    public Sound m_missile2AppearSound;
    public GameObject m_crosshairPrefab;
    public GameObject m_missile2Prefab;

    static int[] s_numMissile = { 6, 10 };
    static float[] s_maxRange = { 6.0f, 10.0f };

    bool m_isReady = false;
    SpriteRenderer m_missile;
    Animator m_anim;
    List<GameObject> m_targets;
    List<float> m_targetDist;
    List<Crosshair> m_crosshairs;

    public override void WarmUp()
    {
        base.WarmUp();
        ObjectPool.GetPool(m_crosshairPrefab, s_numMissile[1]);
        if (null != m_missile2Prefab)
        {
            ObjectPool.GetPool(m_missile2Prefab, s_numMissile[1]);
            Bullet bullet = m_missile2Prefab.GetComponent<Bullet>();
            if (null != bullet)
                bullet.WarmUp();
        }
    }

    protected override void Start()
    {
        base.Start();
        m_missile = transform.GetChild(0).GetComponent<SpriteRenderer>();
        m_anim = GetComponent<Animator>();
        m_targets = new List<GameObject>();
        m_targetDist = new List<float>();
        m_crosshairs = new List<Crosshair>();
        HideMissile();
    }

    public override void Update()
    {
        bool ready = SaveData.Get().HasUpgrade("MISSILE1");
        if (false == ready)
        {
            m_fireTimer = m_fireDelay;
            if (m_isReady)
                HideMissile();
        }

        base.Update();

        m_targets.Clear();
        if (ready)
        {
            if (m_fireTimer <= 0.0f)
            { 
                if (false == m_isReady)
                    ShowMissile();
                // update targets
                GetTargets();
            }
        }

        if (null != m_ownerSprite)
            m_missile.flipX = m_ownerSprite.flipX;

        // make sure the number of crosshairs matches the number of targets
        for (int i = m_crosshairs.Count - 1; i >= m_targets.Count; --i)
        {
            m_crosshairs[i].Free();
            m_crosshairs.RemoveAt(i);
        }
        ObjectPool pool = ObjectPool.GetPool(m_crosshairPrefab, 64);
        if (null != pool)
        {
            for (int i = m_crosshairs.Count; i < m_targets.Count; ++i)
            {
                GameObject crossObj = pool.Allocate(m_targets[i].transform.position);
                m_crosshairs.Add(crossObj.GetComponent<Crosshair>());
            }
        }
        // update the position of the crosshairs
        for (int i = 0; i < m_crosshairs.Count; ++i)
        {
            Vector3 pos = m_targets[i].transform.position;
            pos.z -= 1.0f;
            m_crosshairs[i].transform.position = pos;
        }
    }

    public override void HitTrigger()
    {
        m_triggerHold = true;
    }

    protected override bool Fire()
    {
        GameObject missilePrefab = m_bulletPrefab;
        if (SaveData.Get().HasUpgrade("MISSILE2"))
            missilePrefab = m_missile2Prefab;
        ObjectPool pool = ObjectPool.GetPool(missilePrefab, s_numMissile[1]);
        if (null == pool)
            return false;

        Vector3 pos = GetFirePos();
        List<GameObject> targets = GetTargets();
        if (targets.Count > 0)
        {
            Vector3 dir = Aim();
            float ang = 0.5f;
            float angDelta = 2.0f * Mathf.PI / targets.Count;
            foreach (GameObject target in targets)
            {
                Vector3 finalDir = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang));
                finalDir -= dir;
                ang += angDelta;
                GameObject bulletObj = pool.Allocate(pos);
                if (null != bulletObj)
                {
                    Missile bullet = bulletObj.GetComponent<Missile>();
                    bullet.Launch(pos, dir, finalDir, target);
                }
            }
            HideMissile();
            m_fireSound.Play();
            return true;
        }
        m_failSound.Play();
        return false;
    }

    void HideMissile()
    {
        m_missile.enabled = false;
        m_isReady = false;
    }

    void ShowMissile()
    {
        m_missile.enabled = true;
        if (SaveData.Get().HasUpgrade("MISSILE2"))
        {
            m_anim.Play("Appear2", -1, 0.0f);
            m_missile2AppearSound.Play();
        }
        else
        {
            m_anim.Play("Appear", -1, 0.0f);
            m_missileAppearSound.Play();
        }
        m_isReady = true;
    }

    List<GameObject> GetTargets()
    {
        m_targets.Clear();
        m_targetDist.Clear();
        EnemyBird.ProcessAll(TargetCheck);
        Worm.ProcessEachSegment(TargetCheck);

        return m_targets;
    }

    void TargetCheck(GameObject enemy)
    {
        // don't fire at things below the lava
        Vector3 enemyPos = enemy.transform.position;
        if (enemyPos.y < GameManager.Get().GetLavaHeight())
            return;
        // don't fire at anything off screen
        Bounds screenBounds = GameManager.Get().GetScreenBounds();
        if (false == screenBounds.Contains(enemyPos))
            return;

        int missileLevel = 0;
        if (SaveData.Get().HasUpgrade("MISSILE2"))
            missileLevel = 1;
        float rangeSq = s_maxRange[missileLevel] * s_maxRange[missileLevel];
        Vector3 pos = GetFirePos();
        Vector3 delta = enemyPos - pos;
        {
            float distSq = delta.sqrMagnitude;
            if (distSq < rangeSq)
            {
                bool inserted = false;
                for (int i = 0; i < m_targets.Count; ++i)
                {
                    if (distSq < m_targetDist[i])
                    {
                        m_targetDist.Insert(i, distSq);
                        m_targets.Insert(i, enemy);
                        inserted = true;
                        break;
                    }
                }
                if (false == inserted && m_targets.Count < s_numMissile[missileLevel])
                {
                    m_targetDist.Add(distSq);
                    m_targets.Add(enemy);
                }
                if (m_targets.Count > s_numMissile[missileLevel])
                {
                    m_targetDist.RemoveRange(s_numMissile[missileLevel], m_targets.Count - s_numMissile[missileLevel]);
                    m_targets.RemoveRange(s_numMissile[missileLevel], m_targets.Count - s_numMissile[missileLevel]);
                }
            }
        }
    }

    void OnDisable()
    {
        if (null == m_crosshairs)
            return;
        for (int i = m_crosshairs.Count - 1; i >= 0; --i)
        {
            m_crosshairs[i].Free();
            m_crosshairs.RemoveAt(i);
        }
    }
}
