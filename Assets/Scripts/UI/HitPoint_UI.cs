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
    List<Animator> m_eggAnims;
    float m_waveTimer = 0.0f;
    static HitPoint_UI s_theUI;

    // Start is called before the first frame update
    void Start()
    {
        s_theUI = this;
        m_eggSlots = new List<Image>();
        m_eggAnims = new List<Animator>();
        RectTransform rect = m_eggSlot.transform as RectTransform;
        Vector3 pos = rect.anchoredPosition3D;
        for (int i = 0; i < Player.MaxEgg(); ++i)
        {
            GameObject copy = Instantiate(m_eggSlot.gameObject, transform);
            RectTransform copyRect = copy.transform as RectTransform;
            copyRect.anchoredPosition3D = pos;
            m_eggSlots.Add(copy.GetComponent<Image>());
            m_eggAnims.Add(copy.GetComponent<Animator>());
            pos.x += m_eggSpacing;
        }
        m_eggSlot.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        int numEgg = Player.NumEgg();
        int maxEgg = Player.MaxEgg();

        int eggCount = 1;
        while (eggCount <= numEgg - 2 * maxEgg)
        {
            // Gold Egg
            m_eggAnims[eggCount - 1].Play("GoldEgg");
            m_eggSlots[eggCount - 1].color = Color.white;
            ++eggCount;
        }
        if (numEgg > 2 * maxEgg)
            numEgg = 2 * maxEgg;
        while (eggCount <= numEgg - maxEgg)
        {
            // Silver Egg
            m_eggAnims[eggCount - 1].Play("SilverEgg");
            m_eggSlots[eggCount - 1].color = Color.white;
            ++eggCount;
        }
        if (numEgg > maxEgg)
            numEgg = maxEgg;
        while (eggCount <= numEgg)
        {
            // Regular Egg
            m_eggAnims[eggCount - 1].Play("BaseEgg");
            m_eggSlots[eggCount - 1].color = Color.white;
            ++eggCount;
        }
        while (eggCount <= maxEgg)
        {
            // Empty Egg
            m_eggAnims[eggCount - 1].Play("BaseEgg");
            m_eggSlots[eggCount - 1].color = m_noEggColor;
            ++eggCount;
        }

        UpdateWave(maxEgg);

        if (null != m_points)
            m_points.text = Player.GetScore().ToString();
    }

    public static HitPoint_UI Get()
    {
        return s_theUI;
    }

    public Vector3 GetEggPos()
    {
        int numEgg = Player.NumEgg() % m_eggSlots.Count;
        Vector3 pos = m_eggSlots[numEgg].transform.position;
        return pos;
    }

    public void StartWave()
    {
        m_waveTimer = 0.0f;
    }

    void UpdateWave(int numEgg)
    {
        float dt = Time.unscaledDeltaTime;
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
