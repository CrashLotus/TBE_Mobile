using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreMenu : MonoBehaviour
{
    public TextMeshProUGUI m_crystals;
    public GameObject m_itemArea;
    public GameObject m_itemPrefab;
    public Vector3 m_itemPos;
    public float m_itemSpacing;
    public GameObject m_selection;
    public Vector3 m_selectionOffset;
    public GameObject m_buyButton;

    public Image m_display;
    public TextMeshProUGUI m_cost;
    public TextMeshProUGUI m_desc;

    public Sound m_menuSelect;
    public Sound m_buySound;

    static List<Upgrade> s_allUpgrades;
    int m_selectedIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        SaveData data = SaveData.Get();
        if (null != m_crystals)
            m_crystals.text = data.GetTimeCrystals().ToString();

        if (null == s_allUpgrades)
        {
            s_allUpgrades = new List<Upgrade>();
            TextAsset list = Resources.Load<TextAsset>("Upgrades/UpgradeList");
            string[] lines = list.text.Split("\n"[0]);
            int itemCount = 0;
            foreach (string line in lines)
            {
                s_allUpgrades.Add(Resources.Load<Upgrade>("Upgrades/" + line.TrimEnd()));
                GameObject newItem = Instantiate(m_itemPrefab);
                Button button = newItem.GetComponent<Button>();
                if (null != button)
                {
                    int index = itemCount;
                    button.onClick.AddListener(() => OnSelectItem(index) );
                }
                Transform child = newItem.transform.GetChild(0);
                if (null != child)
                {
                    Image image = child.GetComponent<Image>();
                    if (null != image)
                        image.sprite = s_allUpgrades[itemCount].m_icon;
                    child = child.GetChild(0);
                    if (null != child)
                    {
                        TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                        if (null != text)
                            text.text = s_allUpgrades[itemCount].m_name;
                    }
                }
                newItem.transform.SetParent(m_itemArea.transform, false);
                Vector3 itemPos = m_itemPos;
                itemPos.y += itemCount * m_itemSpacing;
                RectTransform rect = newItem.transform as RectTransform;
                rect.anchoredPosition3D = itemPos;
                ++itemCount;
            }
            Resources.UnloadAsset(list);
        }

        OnSelectItem(-1);
    }

    public void OnSelectItem(int item)
    {
        if (item >= 0 && item < s_allUpgrades.Count)
        {
            m_menuSelect.Play();
            bool isOwned = SaveData.Get().HasUpgrade(s_allUpgrades[item].m_key);
            if (null != m_selection)
            {
                m_selection.SetActive(true);
                Transform xform = m_itemArea.transform.GetChild(item + 1);
                m_selection.transform.position = xform.position + m_selectionOffset;
            }
            if (null != m_display)
            {
                m_display.enabled = true;
                m_display.sprite = s_allUpgrades[item].m_display;
            }
            if (null != m_cost)
            {
                if (isOwned)
                    m_cost.text = "0";
                else
                    m_cost.text = s_allUpgrades[item].m_cost.ToString();
            }
            if (null != m_desc)
            {
                m_desc.enabled = true;
                m_desc.text = s_allUpgrades[item].m_desc;
            }
            if (null != m_buyButton)
            {
                if (isOwned)
                    m_buyButton.SetActive(false);
                else
                    m_buyButton.SetActive(true);
            }
        }
        else
        {
            if (null != m_selection)
                m_selection.SetActive(false);
            if (null != m_display)
                m_display.enabled = false;
            if (null != m_cost)
                m_cost.text = "??";
            if (null != m_desc)
                m_desc.enabled = false;
            if (null != m_buyButton)
                m_buyButton.SetActive(false);
        }
        m_selectedIndex = item;
        Debug.Log("Selected Item " + item);
    }

    public void OnBuyItem()
    {
        Debug.Log("Buy Item");
        m_buySound.Play();
    }

    public void OnExitMenu()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
    }
}
