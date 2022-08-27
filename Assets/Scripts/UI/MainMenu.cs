using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject m_optionsMenu;
    public GameObject m_dialogBox;
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
            m_dialogBox.SetActive(true);
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

    public void OnOptionsMenu()
    {
        m_optionsMenu.SetActive(!m_optionsMenu.activeSelf);
        m_menuSelect.Play();
    }

    public void OnQuitGame()
    {
        Application.Quit();
        m_menuSelect.Play();
    }

    public void OnResetSave()
    {
        SaveData.Get().Reset();
    }

    public void OnNewGameYes()
    {
        GameManager.Get().OnNewGame();
        m_startGameSound.Play();
    }

    public void OnNewGameNo()
    {
        m_dialogBox.SetActive(false);
    }
}
