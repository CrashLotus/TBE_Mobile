//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float m_noMoveZoneLeft = 4.0f;
    public float m_noMoveZoneRight = 2.0f;
    public float m_shakeScale = 1.0f / 60.0f;     // this is to scale it down from the original game size to Unity's size

    static FollowCamera s_theCamera;
    Player m_player;
    List<Shaker> m_shakers;
    float[] m_shakeAng;

    static readonly float[] s_shakeFrequency = new float[] { 12.0f, 30.3f, 52.1f };
    static readonly float[] s_shakePower = new float[] { 0.4108f, 0.5228f, 0.7469f };

    public static void Shake(float amplitude, float duration)
    {
        s_theCamera.m_shakers.Add(new Shaker(amplitude, duration));
    }

    // Start is called before the first frame update
    void Start()
    {
        s_theCamera = this;
        m_player = FindObjectOfType<Player>();
        m_shakers = new List<Shaker>();
        m_shakeAng = new float[3];
        for (int i = 0; i < 3; ++i)
            m_shakeAng[i] = 0.0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        const float s_baseAR = 16.0f / 9.0f;
        float ar = Screen.width / Screen.height / s_baseAR;
        float right = m_noMoveZoneRight * ar;
        float left = m_noMoveZoneLeft * ar;
        Vector3 pos = transform.position;
        float deltaX = m_player.transform.position.x - pos.x;
        if (deltaX > right)
            deltaX -= right;
        else if (deltaX < -left)
            deltaX += left;
        else
            deltaX = 0;
        pos.x += deltaX;

        {   // Camera Shake
            float dt = Time.unscaledDeltaTime;
            float shakeAmount = 0.0f;
            for (int i = 0; i < 3; ++i)
            {
                m_shakeAng[i] += s_shakeFrequency[i] * dt;
                if (m_shakeAng[i] > 2.0f * Mathf.PI)
                    m_shakeAng[i] -= 2.0f * Mathf.PI;
                shakeAmount += Mathf.Sin(m_shakeAng[i]) * s_shakePower[i];
            }

            float shake = 0.0f;
            for (int i = 0; i < m_shakers.Count; ++i)
            {
                m_shakers[i].Update(dt);
                if (m_shakers[i].IsDone())
                {
                    m_shakers.RemoveAt(i);
                    --i;
                }
                else
                {
                    shake += m_shakers[i].GetValue();
                }
            }
            if (shake > 10.0f)
            {
                shake = Mathf.Min(Mathf.Sqrt(shake - 10.0f) + 10.0f, 25.0f);
            }

            pos.y = m_shakeScale * shake * shakeAmount;
        }

        transform.position = pos;
    }

    class Shaker
    {
        float m_amplitude;
        float m_timer;
        float m_duration;
        float m_power;

        public Shaker(float amplitude, float duration)
        {
            m_amplitude = amplitude;
            m_duration = duration;
            m_timer = m_duration;
            m_power = m_amplitude;
        }

        public float GetValue()
        {
            return m_power;
        }

        public void Update(float dt)
        {
            m_timer -= dt;
            float t = Mathf.Max(m_timer / m_duration, 0.0f);
            t = 0.5f * Mathf.PI * t;
            m_power = Mathf.Sin(t);
            m_power = m_power * m_amplitude;
        }

        public bool IsDone()
        {
            return m_timer <= 0.0f;
        }
    }
}
