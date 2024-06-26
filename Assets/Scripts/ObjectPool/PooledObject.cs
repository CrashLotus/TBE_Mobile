//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

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
        if (null == this)
            return;
        if (null == m_pool)
            Destroy(gameObject);
        else
            m_pool.Free(gameObject);
    }
}
