//----------------------------------------------------------------------------------------
//	Copyright � 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    public Slider m_sfxSlider;
    public Slider m_musicSlider;
    public Sound m_sfxAdjust;
    public TMP_Dropdown m_joystickDropdown;
    public Toggle m_tutorialToggle;

    float m_sfxLastPlay = 0.0f;
    const float s_sfxPlayRate = 0.2f;

    void Start()
    {
        m_sfxSlider.value = GetSFXVolume();
        m_musicSlider.value = GetMusicVolume();
        m_joystickDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("joystick", 0));
        m_tutorialToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("tutorial", 1) == 0 ? false : true);
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

    public void OnJoystickSelect(int select)
    {
        PlayerPrefs.SetInt("joystick", select);
        PlayerPrefs.Save();
        GameUI ui = GameUI.Get();
        if (null != ui)
            ui.UpdateOptions();
    }

    public void OnTutorialSelect(Toggle toggle)
    {
        PlayerPrefs.SetInt("tutorial", toggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
        GameUI ui = GameUI.Get();
        if (null != ui)
            ui.UpdateOptions();
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
