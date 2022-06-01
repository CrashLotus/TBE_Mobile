using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInstance : PooledObject
{
    static ObjectPool s_soundPool;
    AudioSource m_source;

    const int s_numSounds = 128;

    public static SoundInstance PlaySound(Sound sound)
    {
        ObjectPool pool = GetPool();
        GameObject obj = pool.Allocate(Vector3.zero);
        SoundInstance instance = obj.GetComponent<SoundInstance>();
        instance.Play(sound);
        return instance;
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        if (null == m_source)
        {
            m_source = gameObject.AddComponent<AudioSource>();
            m_source.playOnAwake = false;
        }
    }

    public void Play(Sound sound)
    {
        sound.Play(m_source);
        StartCoroutine(KillWhenDone());
    }

    IEnumerator KillWhenDone()
    {
        while (m_source.isPlaying)
            yield return null;
        Free();
    }

    static ObjectPool GetPool()
    {
        if (null == s_soundPool)
        {
            GameObject prefab = new GameObject("Sound");
            prefab.AddComponent<SoundInstance>();
            GameObject obj = new GameObject("Pool_Sounds");
            DontDestroyOnLoad(obj);
            s_soundPool = obj.AddComponent<ObjectPool>();
            s_soundPool.m_prefab = prefab;
            s_soundPool.m_numObj = s_numSounds;
            s_soundPool.Start();
        }
        return s_soundPool;
    }
}
