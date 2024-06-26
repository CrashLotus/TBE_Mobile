//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyCrystals : MonoBehaviour
{
    public Sound m_buySound;

    public void OnBuyOffer(string id)
    {
        if (PurchaseManager.Get().BuyProductID(id))
        {
            m_buySound.Play();
        }
        gameObject.SetActive(false);
    }

    public void OnPlayAd()
    {
        if (PurchaseManager.Get().ShowAd(OnAdCompleted))
        {
            gameObject.SetActive(false);
        }
    }

    static void OnAdCompleted(bool success)
    {
        if (success)
        {
            SaveData data = SaveData.Get();
            data.AddTimeCrystals(3);
        }
    }
}