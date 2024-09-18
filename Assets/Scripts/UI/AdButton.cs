//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdButton : MonoBehaviour
{
    public Sprite m_grayImage;

    Button m_button;
    Image m_image;
    Sprite m_origSprite;
    bool m_isReady = true;

    void Start()
    {
        m_button = GetComponent<Button>();
        m_image = GetComponent<Image>();
        m_origSprite = m_image.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (PurchaseManager.Get().IsAdReady())
        {
            if (false == m_isReady)
            {
                m_image.sprite = m_origSprite;
                m_button.enabled = true;
                m_isReady = true;
            }
        }
        else
        {
            if (m_isReady)
            {
                m_image.sprite = m_grayImage;
                m_button.enabled = false;
                m_isReady = false;
            }
        }
    }
}
