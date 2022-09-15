using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorMini : PooledObject
{
    Vector3 m_vel;
    static List<MeteorMini> s_theList = new List<MeteorMini>();

    public void Spawn(float ang, Vector3 vel)
    {
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
        m_vel = vel;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector3 pos = transform.position;
        pos += m_vel * dt;
        transform.position = pos;
        if (pos.y < GameManager.Get().GetLavaHeight())
        {
            Free();
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
