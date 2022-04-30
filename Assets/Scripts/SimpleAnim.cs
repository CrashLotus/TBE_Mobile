using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnim : PooledObject
{
    public bool m_randomRotate = true;
    public float m_minSpeed = 0.8f;
    public float m_maxSpeed = 1.2f;
    public float m_minScale = 0.8f;
    public float m_maxScale = 1.2f;
    public float m_minVolume = 0.8f;
    public float m_maxVolume = 1.2f;
    public float m_minPitch = 0.8f;
    public float m_maxPitch = 1.2f;

    Animator m_anim;
    AudioSource m_audio;

    IEnumerator DeleteWhenDone()
    {
        if (null != m_anim)
        {
            var state = m_anim.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length * state.speed);
        }
        if (null != m_audio)
        {
            while (m_audio.isPlaying)
                yield return null;
        }
        Free();
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        SetUp();
        if (null != m_anim)
        {
            m_anim.Play("Start", -1, 0.0f);
        }
        if (null != m_audio)
        {
            m_audio.Play();
        }
    }

    void SetUp()
    {
        m_anim = GetComponent<Animator>();
        m_audio = GetComponent<AudioSource>();
        if (null != m_anim)
        {
            m_anim.speed = Random.Range(m_minSpeed, m_maxSpeed);
        }
        transform.localScale = Random.Range(m_minScale, m_maxScale) * Vector3.one;
        if (m_randomRotate)
        {
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        }
        if (null != m_audio)
        {
            m_audio.volume = Random.Range(m_minVolume, m_maxVolume);
            m_audio.pitch = Random.Range(m_minPitch, m_maxPitch);
        }
        StartCoroutine(DeleteWhenDone());
    }
}
