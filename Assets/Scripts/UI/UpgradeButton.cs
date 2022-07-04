using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Image m_icon;
    public Image m_lock;
    public Image m_check;
    public TextMeshProUGUI m_name;
    public Sprite m_buttonBlue;
    public Sprite m_buttonRed;
    public Sprite m_buttonGreen;

    public void SetUp(Upgrade upgrade)
    {
        Image buttonImage = GetComponent<Image>();
        bool isOwned = upgrade.IsOwned();
        bool isLocked = upgrade.IsLocked();

        m_icon.sprite = upgrade.m_icon;
        m_name.text = upgrade.m_name;
        if (isLocked)
        {
            m_lock.enabled = true;
            m_check.enabled = false;
            buttonImage.sprite = m_buttonRed;
        }
        else if (isOwned)
        {
            m_lock.enabled = false;
            m_check.enabled = true;
            buttonImage.sprite = m_buttonGreen;
        }
        else
        {
            m_lock.enabled = false;
            m_check.enabled = false;
            buttonImage.sprite = m_buttonBlue;
        }
    }
}
