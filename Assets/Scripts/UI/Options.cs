using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Slider m_sfxSlider;
    public Slider m_musicSlider;
    public Sound m_sfxAdjust;

    float m_sfxLastPlay = 0.0f;
    const float s_sfxPlayRate = 0.2f;

    void Start()
    {
        m_sfxSlider.value = GetSFXVolume();
        m_musicSlider.value = GetMusicVolume();
    }

    public void OnSFXVolume()
    {
        PlayerPrefs.SetFloat("sfxVolume", m_sfxSlider.value);
        PlayerPrefs.Save();
        SoundInstance.SetVolume(m_sfxSlider.value);
        float now = Time.realtimeSinceStartup;
        if (now - m_sfxLastPlay >= s_sfxPlayRate)
        {
            m_sfxLastPlay = now;
            m_sfxAdjust.Play();
        }
    }

    public void OnMusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", m_musicSlider.value);
        PlayerPrefs.Save();
        MusicManager.Get().SetVolume(m_musicSlider.value);
    }

    public static float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("sfxVolume", 1.0f);
    }

    public static float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("musicVolume", 0.5f);
    }
}
