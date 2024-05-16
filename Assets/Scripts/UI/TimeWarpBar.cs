using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeWarpBar : MonoBehaviour
{
    public Sound m_soundFill;

    Slider m_slider;
    TextMeshProUGUI m_combo;
    float m_value = 0.0f;
    float m_ratio = 0.0f;
    float m_flashTimer;
    Color m_color = Color.white;
    float m_pulseTimer;
    float m_scale;
    SimpleButton m_subButton;

    const float s_valueSpeed = 100.0f;
    static Color s_color = Color.green;
    static Color s_flashColor = Color.yellow;
    const float s_flashFreq = 6.0f;
    const float s_pulseFreq = 8.0f;
    const float s_pulseScale = 1.2f;

    public static Vector2 GetComboPos()
    {
        return new Vector2(800.0f, -200.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_slider = GetComponentInChildren<Slider>();
        m_combo = GetComponentInChildren<TextMeshProUGUI>();
        m_value = BulletTime.Get().GetTimeBarValue();
        m_subButton = null;
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            SimpleButton subButton = child.GetComponent<SimpleButton>();
            if (subButton)
            {
                m_subButton = subButton;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (null == m_slider)
            return;

        float dt = Time.unscaledDeltaTime;
        BulletTime bt = BulletTime.Get();
        float targetValue = bt.GetTimeBarValue();

        bool wasFull = m_value >= BulletTime.s_timeWarpPoints;

        float diff = targetValue - m_value;
        diff = Mathf.Clamp(diff, -s_valueSpeed * dt, s_valueSpeed * dt);
        m_value += diff;
        m_value = Mathf.Clamp(m_value, 0.0f, BulletTime.s_timeWarpPoints);

        m_ratio = m_value / BulletTime.s_timeWarpPoints;
        m_slider.value = m_ratio;
        if (m_ratio >= 1.0f)
        {
            m_flashTimer = m_flashTimer + s_flashFreq * dt;
            while (m_flashTimer > 2.0f * Mathf.PI)
                m_flashTimer -= 2.0f * Mathf.PI;
            float lerp = 0.5f - 0.5f * Mathf.Cos(m_flashTimer);
            m_color = Color.Lerp(s_color, s_flashColor, lerp);
            if (false == wasFull)
            {
                if (null != m_soundFill)
                    m_soundFill.Play();
                m_pulseTimer = 0.0f;
//                m_sparkAnim.Reset();  //mrwTODO
            }
            if (m_subButton)
                m_subButton.Show(true);
        }
        else
        {
            m_color = s_color;
            m_flashTimer = 0.0f;
            if (m_subButton)
                m_subButton.Show(false);
        }

        // update pulse
        m_pulseTimer += s_pulseFreq * dt;
        if (m_pulseTimer < Mathf.PI * 2.0f)
        {
            float lerp = Mathf.Abs(Mathf.Sin(m_pulseTimer));
            m_scale = Mathf.Lerp(1.0f, s_pulseScale, lerp);
        }
        else
        {
            m_scale = 1.0f;
        }

        m_slider.targetGraphic.color = m_color;
        m_slider.transform.localScale = new Vector3(m_scale, m_scale, m_scale);

        if (null != m_combo)
        {
            StringBuilder sb = new StringBuilder("+", 3);
            sb.Append(Player.GetComboPoints());
            m_combo.text = sb.ToString();
        }
    }
}
