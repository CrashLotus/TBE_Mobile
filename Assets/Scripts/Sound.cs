using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Sound")]
public class Sound : ScriptableObject
{
    public AudioClip m_clip;
    public float m_minPitch = 0.8f;
    public float m_maxPitch = 1.4f;
    public float m_minVol = 0.8f;
    public float m_maxVol = 1.0f;

    public void Play(AudioSource source)
    {
        source.clip = m_clip;
        source.pitch = Random.Range(m_minPitch, m_maxPitch);
        source.volume = Random.Range(m_minVol, m_maxVol);
        source.Play();
    }

    public void Play()
    {
        SoundInstance.PlaySound(this);
    }
}
