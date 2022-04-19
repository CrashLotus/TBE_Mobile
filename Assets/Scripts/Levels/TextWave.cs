using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/TextWave")]
public class TextWave : Wave
{
    public float m_delay;
    float m_textTimer;

    public override void Start()
    {
        m_textTimer = m_delay;
        Debug.Log("TextWave: " + m_text);
        base.Start();
    }

    public override void Update()
    {
        float dt = Time.deltaTime;
        m_textTimer -= dt;
        base.Update();
    }

    public override bool IsDone()
    {
        if (m_isDone)
            return true;

        if (m_textTimer <= 0.0f)
            return true;

        return false;
    }
}
