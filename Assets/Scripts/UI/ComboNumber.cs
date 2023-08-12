using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ComboNumber : PooledObject
{
    static ObjectPool s_numberPool;

    float m_timer;
    Vector2 m_startPos;
    TextMeshProUGUI m_text;

    const float s_timeOut = 1.0f;

    public static void MakeNumberPool()
    {
        if (null == s_numberPool)
        {
            GameObject numberPrefab = Resources.Load<GameObject>("ComboNumber");
            s_numberPool = ObjectPool.GetPool(numberPrefab, 32);
        }
    }

    public static void Spawn(Vector3 worldPos, int number)
    {
        if (null == s_numberPool)
            MakeNumberPool();
        if (null != s_numberPool)
        {
            GameObject numObj = s_numberPool.Allocate(worldPos);
            if (null != numObj)
            {
                numObj.transform.SetParent(GameUI.Get().transform, false);
                TextMeshProUGUI textMesh = numObj.GetComponent<TextMeshProUGUI>();
                if (null != textMesh)
                {
                    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
                    textMesh.rectTransform.anchoredPosition = textMesh.transform.InverseTransformPoint(screenPos);
                    StringBuilder sb = new StringBuilder("+", 3);
                    sb.Append(number);
                    textMesh.text = sb.ToString();
                    ComboNumber combo = numObj.GetComponent<ComboNumber>();
                    if (null != combo)
                    {
                        combo.m_timer = s_timeOut;
                        combo.m_startPos = textMesh.rectTransform.anchoredPosition;
                        combo.m_text = textMesh;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_timer -= Time.unscaledDeltaTime;
        if (m_timer <= 0.0f)
            Free();
        else
        {
            float lerp = m_timer / s_timeOut;
            m_text.rectTransform.anchoredPosition = Vector2.Lerp(TimeWarpBar.GetComboPos(), m_startPos, lerp);
        }
    }
}
