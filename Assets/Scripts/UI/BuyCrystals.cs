using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyCrystals : MonoBehaviour
{
    public GameObject m_itemArea;
    public GameObject m_itemPrefab;
    public Vector3 m_itemPos;
    public float m_itemSpacing;
    public Sound m_buySound;

    List<CrystalOffer> m_allOffsers;

    // Start is called before the first frame update
    void Start()
    {
        if (null == m_allOffsers)
        {
            m_allOffsers = new List<CrystalOffer>();
            TextAsset list = Resources.Load<TextAsset>("Crystals/CrystalList");
            string[] lines = list.text.Split("\n"[0]);
            int itemCount = 0;
            foreach (string line in lines)
            {
                m_allOffsers.Add(Resources.Load<CrystalOffer>("Crystals/" + line.TrimEnd()));
                GameObject newItem = Instantiate(m_itemPrefab);
                Button button = newItem.GetComponent<Button>();
                if (null != button)
                {
                    int index = itemCount;
                    button.onClick.AddListener(() => OnBuyOffer(index));
                }
                Transform number = newItem.transform.Find("Number");
                if (null != number)
                {
                    TextMeshProUGUI text = number.GetComponent<TextMeshProUGUI>();
                    if (null != text)
                        text.text = m_allOffsers[itemCount].m_numCrystal.ToString();
                }
                number = newItem.transform.Find("Cost");
                if (null != number)
                {
                    TextMeshProUGUI text = number.GetComponent<TextMeshProUGUI>();
                    if (null != text)
                    {
                        string cost = string.Format("${0:0.00}", m_allOffsers[itemCount].m_cost);
                        text.text = cost;
                    }
                }
                newItem.transform.SetParent(m_itemArea.transform, false);
                Vector3 itemPos = m_itemPos;
                itemPos.x += itemCount * m_itemSpacing;
                RectTransform rect = newItem.transform as RectTransform;
                rect.anchoredPosition3D = itemPos;
                ++itemCount;
            }
        }
    }

    public void OnBuyOffer(int item)
    {
        if (item >= 0 && item < m_allOffsers.Count)
        {
            //mrwTODO cost
            SaveData data = SaveData.Get();
            data.AddTimeCrystals(m_allOffsers[item].m_numCrystal);
        }
        m_buySound.Play();
        gameObject.SetActive(false);
    }
}