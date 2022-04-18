using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    ObjectPool m_pool;

    virtual public void Init(ObjectPool pool)
    {
        if (null == m_pool)
            m_pool = pool;
    }

    virtual public void Stop()
    {
        StopAllCoroutines();
    }

    public void Free()
    {
        if (null == m_pool)
        {
            Destroy(gameObject);
        }
        else
        {
            m_pool.Free(gameObject);
        }
    }
}
