using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyCrystals : MonoBehaviour
{
    public Sound m_buySound;

    public void OnBuyOffer(int numCrystal)
    {
        //mrwTODO cost
        SaveData data = SaveData.Get();
        data.AddTimeCrystals(numCrystal);
        m_buySound.Play();
        gameObject.SetActive(false);
    }
}