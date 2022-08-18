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
        //mrwTODO cost
        if (PurchaseManager.Get().BuyProductID(id))
        {
//            SaveData data = SaveData.Get();
//            data.AddTimeCrystals(5);
            m_buySound.Play();
        }
        gameObject.SetActive(false);
    }

    public void OnPurchaseFailed()
    {
        Debug.LogError("OnPurchaseFailed");
        gameObject.SetActive(false);
    }
}