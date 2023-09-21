using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Worm;

public class Ninja : EnemyBird
{
    static ObjectPool s_ninjaPool = null;

    public static new void WarmUp()
    {
        if (null == s_ninjaPool)
        {
            MakePool();
            if (null != s_ninjaPool)
            {
                Ninja bird = s_ninjaPool.m_prefab.GetComponent<Ninja>();
                bird._WarmUp();
            }
        }
    }

    public static Ninja Spawn(Vector3 pos)
    {
        MakePool();
        if (null != s_ninjaPool)
        {
            GameObject enemyObj = s_ninjaPool.Allocate(pos);
            Ninja ninja = enemyObj.GetComponent<Ninja>();
            return ninja;
        }
        return null;
    }

    public static void MakePool()
    {
        if (null == s_ninjaPool)
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("Ninja");
            if (null == enemyPrefab)
            {
                Debug.LogError("Unable to load enemy prefab Ninja");
                return;
            }
            s_ninjaPool = ObjectPool.GetPool(enemyPrefab, 4);
        }
    }
}
