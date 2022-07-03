using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public enum SongType
    {
        TITLE,
        GAME,
        SHOP,
        CREDITS
    }
    public AudioClip m_songTitle;
    public AudioClip m_songGame;
    public AudioClip m_songShop;
    public AudioClip m_songCredits;

    static MusicManager s_theManager;
    AudioSource m_source;
    AudioMixerGroup m_mixerGroup;

    public static MusicManager Get()
    {
        if (null == s_theManager)
        {
            GameObject musicObj = Resources.Load<GameObject>("MusicManager");
            if (null == musicObj)
            {
                Debug.LogError("Unable to load MusicManager prefab");
                return null;
            }
            GameObject newObj = Instantiate(musicObj);
            newObj.name = "MusicManager";
            DontDestroyOnLoad(newObj);
            s_theManager = newObj.GetComponent<MusicManager>();
            s_theManager.Init();
        }
        return s_theManager;
    }

    void Init()
    {
        m_source = GetComponent<AudioSource>();
        if (null == m_source)
        {
            m_source = gameObject.AddComponent<AudioSource>();
            AudioMixer mixer = Resources.Load<AudioMixer>("Mixer");
            if (null == mixer)
            {
                Debug.LogError("Unable to load Mixer");
            }
            else
            {
                AudioMixerGroup[] groups = mixer.FindMatchingGroups("Music");
                m_mixerGroup = groups[0];
                m_source.outputAudioMixerGroup = m_mixerGroup;
            }
        }
        else
        {
            m_mixerGroup = m_source.outputAudioMixerGroup;
        }
        m_source.playOnAwake = false;
        m_source.loop = true;
        SetVolume(Options.GetMusicVolume());
    }

    public void Play(SongType song)
    {
        switch (song)
        {
            case SongType.TITLE:
                _Play(m_songTitle);
                break;
            case SongType.GAME:
                _Play(m_songGame);
                break;
            case SongType.SHOP:
                _Play(m_songShop);
                break;
            case SongType.CREDITS:
                _Play(m_songCredits);
                break;
        }
    }

    public void SetVolume(float volume)
    {
        float musicVol = Mathf.Clamp(volume, 0.0001f, 1.0f);
        musicVol = Mathf.Log10(musicVol) * 20.0f;
        m_mixerGroup.audioMixer.SetFloat("MusicVolume", musicVol);
    }

    void _Play(AudioClip song)
    {
        m_source.Stop();
        if (null == song)
            return;
        m_source.clip = song;
        m_source.Play();
    }
}
