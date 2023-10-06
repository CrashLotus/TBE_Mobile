using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScreenDude : MonoBehaviour
{
    public float m_rotSpeed = 180.0f;
    public float m_endScale = 0.25f;
    public float m_scaleTime = 2.0f;
    float m_rot = 0.0f;
    public float m_scaleTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        m_rot += m_rotSpeed * Time.unscaledDeltaTime;
        if (m_rot > 180.0f)
            m_rot -= 360.0f;
        m_scaleTimer += Time.unscaledDeltaTime;
        if (m_scaleTimer > m_scaleTime)
            m_scaleTimer = m_scaleTime;
        float scale = Mathf.Lerp(1.0f, m_endScale, m_scaleTimer / m_scaleTime);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, m_rot);
        transform.localScale = scale * Vector3.one;
    }
}
