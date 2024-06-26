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
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject m_optionsMenu;
    public DialogBox m_dialogBox;
    public GameObject m_continueGame;
    public Sound m_menuSelect;
    public Sound m_startGameSound;

    public static void GrayOutButton(GameObject buttonObj)
    {
        if (null == buttonObj)
            return;
        {
            Button button = buttonObj.GetComponent<Button>();
            if (null != button)
                button.enabled = false;
        }

        {
            Image image = buttonObj.GetComponent<Image>();
            if (null != image)
            {
                Color gray = image.color;
                gray.a = 0.5f;
                image.color = gray;
            }
        }

        { 
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (null != text)
            {
                Color gray = text.color;
                gray.a = 0.5f;
                text.color = gray;
            }
        }
    }

    void Start()
    {
        if (SaveData.Get().GetCurrentLevel() == 0)
        {   // disable the continue game option
            GrayOutButton(m_continueGame);
            UI_Utility.SelectUI(transform.Find("NewGame").gameObject);
        }
        else
        {
            UI_Utility.SelectUI(m_continueGame);
        }
    }

    public void OnNewGame()
    {
        SaveData data = SaveData.Get();
        if (data.GetCurrentLevel() == 0)
        {
            OnNewGameYes();
        }
        else
        {
            m_dialogBox.ShowDialog("Creating a new game will erase your current progress.\n\nDo you want to start a new game?", OnNewGameYes, null);
        }
    }
    public void OnContinueGame()
    {
        GameManager.Get().OnContinueGame();
        m_startGameSound.Play();
    }

    public void OnStoreMenu()
    {
        GameManager.Get().GoToStore();
        m_menuSelect.Play();
    }

    public void OnLeaderboard()
    {
        GameManager.Get().GoToLeaderboard();
        m_menuSelect.Play();
    }

    public void OnOptionsMenu()
    {
        bool openOptions = !m_optionsMenu.activeSelf;
        m_optionsMenu.SetActive(openOptions);
        m_menuSelect.Play();
        if (openOptions)
            UI_Utility.SelectUI(m_optionsMenu.transform.GetChild(0).Find("Close").gameObject);
        else
            UI_Utility.SelectUI(transform.Find("Options").gameObject);
    }

    public void OnQuitGame()
    {
        Application.Quit();
        m_menuSelect.Play();
    }

    public void OnCredits()
    {
        GameManager.Get().GoToCredits();
        m_menuSelect.Play();
    }

    public void OnResetSave()
    {
        m_dialogBox.ShowDialog("This will erase your current save and online account information.\n\nAll upgrades and Time Crystals will be removed.", DoResetSave, null);
    }

    void DoResetSave()
    {
        Debug.Log("DoResetSave()");
        SaveData.Get().Reset();
        PurchaseManager.Get().DeleteAccount();
    }

    public void OnNewGameYes()
    {
        GameManager.Get().OnNewGame();
        m_startGameSound.Play();
    }
}
