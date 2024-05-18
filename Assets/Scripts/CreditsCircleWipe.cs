using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsCircleWipe : MonoBehaviour
{
    public float m_circleRate = 1.0f;

    float m_circleTime = 0.0f;

    // Update is called once per frame
    void Update()
    {
        m_circleTime += m_circleRate * Time.unscaledDeltaTime;
        Shader.SetGlobalFloat("_CreditWipe", m_circleTime);
    }
}
