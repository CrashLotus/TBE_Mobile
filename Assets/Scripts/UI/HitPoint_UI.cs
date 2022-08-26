using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitPoint_UI : MonoBehaviour
{
    public TMP_Text m_points;
    public Image m_eggSlot;
    public float m_eggSpacing = 56.0f;
    public Color m_noEggColor;
    public float m_scale = 1.0f;
    public float m_waveScale = 1.25f;
    public float m_waveFreq = 16.0f;
    public float m_waveOffset = 0.75f;

    List<Image> m_eggSlots;
    float m_waveTimer = 0.0f;
    static HitPoint_UI s_theUI;

    // Start is called before the first frame update
    void Start()
    {
        s_theUI = this;
        m_eggSlots = new List<Image>();
        m_eggSlots.Add(m_eggSlot);
        RectTransform rect = m_eggSlot.transform as RectTransform;
        Vector3 pos = rect.anchoredPosition3D;
        for (int i = 1; i < Player.MaxEgg(); ++i)
        {
            GameObject copy = Instantiate(m_eggSlot.gameObject, transform);
            pos.x += m_eggSpacing;
            RectTransform copyRect = copy.transform as RectTransform;
            copyRect.anchoredPosition3D = pos;
            m_eggSlots.Add(copy.GetComponent<Image>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        int numEgg = 0;
        Player player = Player.Get();
        if (null != player)
        {
            numEgg = player.NumEgg();
        }
        for (int i = 0; i < numEgg; ++i)
        {
            m_eggSlots[i].color = Color.white;
        }
        int maxEgg = Player.MaxEgg();
        for (int i = numEgg; i < maxEgg; ++i)
        {
            m_eggSlots[i].color = m_noEggColor;
        }
        UpdateWave(numEgg);

        m_points.text = Player.GetScore().ToString();
    }

    public static HitPoint_UI Get()
    {
        return s_theUI;
    }

    public Vector3 GetEggPos()
    {
        int numEgg = 0;
        Player player = Player.Get();
        if (null != player)
            numEgg = player.NumEgg();
        if (numEgg > m_eggSlots.Count - 1)
            numEgg = m_eggSlots.Count - 1;
        Vector3 pos = m_eggSlots[numEgg].transform.position;
        return pos;
    }

    public void StartWave()
    {
        m_waveTimer = 0.0f;
    }

    void UpdateWave(int numEgg)
    {
        float dt = Time.deltaTime;
        m_waveTimer += m_waveFreq * dt;
        float eggWaveTime = m_waveTimer;
        for (int i = 0; i < numEgg; ++i)
        {
            if (eggWaveTime > 0.0f && eggWaveTime < Mathf.PI)
            {
                float lerp = Mathf.Sin(eggWaveTime);
                m_eggSlots[i].transform.localScale = Mathf.Lerp(m_scale, m_waveScale, lerp) * Vector3.one;
            }
            else
            {
                m_eggSlots[i].transform.localScale = m_scale * Vector3.one;
            }
            eggWaveTime -= m_waveOffset;
        }
    }
}
