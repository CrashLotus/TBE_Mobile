//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCrystal : PickUp
{
    static ObjectPool s_crystalPool;

    Animator m_anim;

    const float s_hudFlyTime = 2.0f;

    void Start()
    {
        Init(null);   
    }

    public static void MakeCrystalPool()
    {
        if (null == s_crystalPool)
        {
            GameObject crystalPrefab = Resources.Load<GameObject>("TimeCrystal");
            s_crystalPool = ObjectPool.GetPool(crystalPrefab, 2);
        }
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_anim = GetComponent<Animator>();
        if (null != m_anim)
            m_anim.Play("CrystalIdle", -1, 0.0f);
    }

    static TimeCrystal MakeCrystal(Vector3 pos)
    {
        if (null == s_crystalPool)
        {
            MakeCrystalPool();
        }
        if (null != s_crystalPool)
        {
            GameObject crystalObj = s_crystalPool.Allocate(pos);
            if (null != crystalObj)
            {
                TimeCrystal crystal = crystalObj.GetComponent<TimeCrystal>();
                if (null != crystal.m_spawnSound)
                    crystal.m_spawnSound.Play();
                return crystal;
            }
        }
        return null;
    }

    public static void Spawn(Vector3 pos, float upSpeed)
    {
        TimeCrystal crystal = MakeCrystal(pos);
        crystal.PopUp(upSpeed);
    }

    public static void Spawn(Vector3 pos)
    {
        Spawn(pos, s_spawnSpeed);
    }

    protected override void UpdateFlyToHud(float dt)
    {
        base.UpdateFlyToHud(dt);
        m_stateTimer -= dt;
        if (m_stateTimer <= 0.0f)
        {
            Free();
        }
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

    protected override void PickedUp(Player player)
    {
        base.PickedUp(player);
        if (null != m_anim)
            m_anim.Play("CrystalPickUp", -1, 0.0f);
        SaveData data = SaveData.Get();
        data.AddTimeCrystals(1);
    }
}
