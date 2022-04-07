using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    static Dictionary<GameObject, ObjectPool> s_allPools = new Dictionary<GameObject, ObjectPool>();
    public GameObject m_prefab;
    public int m_numObj = 100;
    
    List<GameObject> m_pool;

    public static ObjectPool GetPool(GameObject prefab, int num)
    {
        if (false == s_allPools.ContainsKey(prefab))
        {
            GameObject obj = new GameObject("Pool_" + prefab.name);
            s_allPools[prefab] = obj.AddComponent<ObjectPool>();
            s_allPools[prefab].m_prefab = prefab;
            s_allPools[prefab].m_numObj = num;
            s_allPools[prefab].Start();
        }
        return s_allPools[prefab];
    }

    protected virtual void Start()
    {
        s_allPools[m_prefab] = this;
        if (null == m_pool)
        {
            m_pool = new List<GameObject>(m_numObj);
            for (int i = 0; i < m_numObj; ++i)
            {
                m_pool.Add(GameObject.Instantiate(m_prefab));
                m_pool[i].transform.SetParent(transform);
                m_pool[i].SetActive(false);
            }
        }
    }

    public GameObject Allocate(Vector3 pos)
    {
        for (int i = 0; i < m_numObj; ++i)
        {
            if (false == m_pool[i].activeSelf)
            {   // got one
                m_pool[i].transform.SetParent(null, false);
                m_pool[i].transform.localPosition = pos;
                m_pool[i].SetActive(true);
                PooledObject pooledObject = m_pool[i].GetComponent<PooledObject>();
                if (null != pooledObject)
                {
                    pooledObject.Init(this);
                }
                return m_pool[i];
            }
        }
        return null;    // none left available
    }

    public void Free(GameObject obj)
    {
        PooledObject pooledObject = obj.GetComponent<PooledObject>();
        if (null != pooledObject)
        {
            pooledObject.Stop();
        }
        obj.transform.SetParent(transform, false);
        obj.SetActive(false);
    }
}
