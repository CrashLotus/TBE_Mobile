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

    List<Image> m_eggSlots;
    static HitPoint_UI s_theUI;

    // Start is called before the first frame update
    void Start()
    {
        s_theUI = this;
        m_eggSlots = new List<Image>();
        m_eggSlots.Add(m_eggSlot);
        Vector3 pos = m_eggSlot.transform.position;
        for (int i = 1; i < Player.MaxEgg(); ++i)
        {
            GameObject copy = Instantiate(m_eggSlot.gameObject, transform);
            pos.x += m_eggSpacing;
            copy.transform.position = pos;
            m_eggSlots.Add(copy.GetComponent<Image>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Player player = Player.Get();
        int numEgg = player.NumEgg();
        for (int i = 0; i < numEgg; ++i)
        {
            m_eggSlots[i].color = Color.white;
        }
        int maxEgg = Player.MaxEgg();
        for (int i = numEgg; i < maxEgg; ++i)
        {
            m_eggSlots[i].color = m_noEggColor;
        }

        m_points.text = Player.GetScore().ToString();
    }

    public static HitPoint_UI Get()
    {
        return s_theUI;
    }
    public Vector3 GetEggPos()
    {
        Vector3 pos = m_eggSlots[0].transform.position;
        pos.x += m_eggSpacing * Player.Get().NumEgg();
        return pos;
    }
}