using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundInstance : PooledObject
{
    static ObjectPool s_soundPool;
    static AudioMixerGroup s_sfxGroup;
    AudioSource m_source;

    const int s_numSounds = 128;

    public static void WarmUp()
    {
        GetPool();
    }

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
        m_source.outputAudioMixerGroup = s_sfxGroup;
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
        if (null == s_sfxGroup)
        {
            AudioMixer mixer = Resources.Load<AudioMixer>("Mixer");
            if (null == mixer)
            {
                Debug.LogError("Unable to load Mixer");
            }
            else
            {
                AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
                s_sfxGroup = groups[0];
                SetVolume(Options.GetSFXVolume());
            }
        }
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

    public static void SetVolume(float volume)
    {
        float sfxVolume = Mathf.Clamp(volume, 0.0001f, 1.0f);
        sfxVolume = Mathf.Log10(sfxVolume) * 20.0f;
        s_sfxGroup.audioMixer.SetFloat("SFXVolume", sfxVolume);
    }
}
