using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public GameObject m_offerPanel;

    public TextMeshProUGUI m_cost;
    public GameObject m_icon;
    public TextMeshProUGUI m_desc;

    public Sound m_menuSelect;
    public Sound m_buySound;

    List<Upgrade> m_allUpgrades;
    List<UpgradeButton> m_allButtons;
    int m_selectedIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Get();  // bootstrap the game
        SaveData data = SaveData.Get();
        if (null != m_crystals)
            m_crystals.text = data.GetTimeCrystals().ToString();

        if (null == m_allUpgrades)
        {
            m_allUpgrades = new List<Upgrade>();
            m_allButtons = new List<UpgradeButton>();
            TextAsset list = Resources.Load<TextAsset>("Upgrades/UpgradeList");
            string[] lines = list.text.Split("\n"[0]);
            int itemCount = 0;
            foreach (string line in lines)
            {
                if (line.Length == 0)
                    continue;
                m_allUpgrades.Add(Resources.Load<Upgrade>("Upgrades/" + line.TrimEnd()));
                GameObject newItem = Instantiate(m_itemPrefab);
                UpgradeButton upgradeButton = newItem.GetComponent<UpgradeButton>();
                if (null != upgradeButton)
                {
                    upgradeButton.SetUp(m_allUpgrades[itemCount]);
                }
                m_allButtons.Add(upgradeButton);
                Button button = newItem.GetComponent<Button>();
                if (null != button)
                {
                    int index = itemCount;
                    button.onClick.AddListener(() => OnSelectItem(index) );
                }
                newItem.transform.SetParent(m_itemArea.transform, false);
                Vector3 itemPos = m_itemPos;
                itemPos.x += itemCount * m_itemSpacing;
                RectTransform rect = newItem.transform as RectTransform;
                rect.anchoredPosition3D = itemPos;
                ++itemCount;
            }
            Resources.UnloadAsset(list);
            RectTransform contentRect = m_itemArea.GetComponent<RectTransform>();
            Vector2 size = contentRect.sizeDelta;
            size.x = itemCount * m_itemSpacing;
            contentRect.sizeDelta = size;
        }

        OnSelectItem(-1);
        UI_Utility.SelectUI(transform.Find("Continue").gameObject);
    }

    void Update()
    {
        SaveData data = SaveData.Get();
        if (null != m_crystals)
            m_crystals.text = data.GetTimeCrystals().ToString();
    }

    public void OnSelectItem(int item)
    {
        if (item >= 0 && item < m_allUpgrades.Count)
        {
            m_menuSelect.Play();
            bool isOwned = m_allUpgrades[item].IsOwned();
            bool isLocked = m_allUpgrades[item].IsLocked();
            if (null != m_selection)
            {
                m_selection.SetActive(true);
                Transform xform = m_itemArea.transform.GetChild(item + 1);
                m_selection.transform.localPosition = xform.localPosition + m_selectionOffset;
            }
            if (null != m_cost)
            {
                bool showIcon = true;
                if (isLocked)
                {
                    m_cost.text = "Locked";
                    showIcon = false;
                }
                else if (isOwned)
                {
                    m_cost.text = "0";
                }
                else
                {
                    m_cost.text = m_allUpgrades[item].m_cost.ToString();
                }
                if (null != m_icon)
                    m_icon.SetActive(showIcon);
            }
            if (null != m_desc)
            {
                m_desc.enabled = true;
                m_desc.text = m_allUpgrades[item].m_desc;
            }
            if (null != m_buyButton)
            {
                if (isOwned || isLocked)
                    m_buyButton.SetActive(false);
                else
                    m_buyButton.SetActive(true);
            }
        }
        else
        {
            if (null != m_selection)
                m_selection.SetActive(false);
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
        if (m_selectedIndex < 0 || m_selectedIndex >= m_allUpgrades.Count)
            return;
        
        SaveData data = SaveData.Get();
        if (m_allUpgrades[m_selectedIndex].IsOwned() || m_allUpgrades[m_selectedIndex].IsLocked())
            return;

        int cost = m_allUpgrades[m_selectedIndex].m_cost;
        if (data.GetTimeCrystals() >= cost)
        {   // buy it
            data.AddTimeCrystals(-cost);
            m_allUpgrades[m_selectedIndex].Buy();
            Debug.Log("Buy Item");
            m_buySound.Play();
            for (int i = 0; i < m_allButtons.Count; ++i)
            {
                m_allButtons[i].SetUp(m_allUpgrades[i]);
            }
            OnSelectItem(m_selectedIndex);
        }
        else
        {
            m_offerPanel.SetActive(true);
            m_menuSelect.Play();
        }
    }

    public void OnExitMenu()
    {
        GameManager.Get().ReturnToMainMenu();
        m_menuSelect.Play();
    }

    public void OnContinueGame()
    {
        GameManager.Get().OnContinueGame();
        m_menuSelect.Play();
    }
}
