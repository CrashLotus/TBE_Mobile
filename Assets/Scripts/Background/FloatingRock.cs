using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRock : MonoBehaviour
{
    public float m_wiggleSpeedMin = 0.25f;
    public float m_wiggleSpeedMax = 2.0f;
    public float m_wiggleAmpMinX = 0.03f;
    public float m_wiggleAmpMaxX = 0.06f;
    public float m_wiggleAmpMinY = 0.1f;
    public float m_wiggleAmpMaxY = 0.4f;
    float m_wiggleTimerX;
    float m_wiggleTimerY;
    float m_wiggleSpeedX;
    float m_wiggleSpeedY;
    float m_wiggleAmpX;
    float m_wiggleAmpY;
    Vector3 m_centerPos;
    float m_scale;

    void Wiggle()
    {
        Vector3 pos = transform.localPosition;
        pos.x = m_centerPos.x + m_wiggleAmpX * Mathf.Cos(m_wiggleTimerX);
        pos.y = m_centerPos.y + m_wiggleAmpY * Mathf.Sin(m_wiggleTimerY);
        transform.localPosition = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_centerPos = transform.localPosition;
        m_scale = transform.localScale.x;
        m_wiggleTimerX = Random.Range(0.0f, 2.0f * Mathf.PI);
        m_wiggleTimerY = Random.Range(0.0f, 2.0f * Mathf.PI);
        m_wiggleSpeedX = Random.Range(m_wiggleSpeedMin, m_wiggleSpeedMax);
        m_wiggleSpeedY = Random.Range(m_wiggleSpeedMin, m_wiggleSpeedMax);
        m_wiggleAmpX = Random.Range(m_wiggleAmpMinX, m_wiggleAmpMaxX) * m_scale;
        m_wiggleAmpY = Random.Range(m_wiggleAmpMinY, m_wiggleAmpMaxY) * m_scale;
        Wiggle();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime();
        m_wiggleTimerX += dt * m_wiggleSpeedX;
        while (m_wiggleTimerX > 2.0f * Mathf.PI)
            m_wiggleTimerX -= 2.0f * Mathf.PI;
        m_wiggleTimerY += dt * m_wiggleSpeedY;
        while (m_wiggleTimerY > 2.0f * Mathf.PI)
            m_wiggleTimerY -= 2.0f * Mathf.PI;
        Wiggle();
    }
}
