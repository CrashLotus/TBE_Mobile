using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject m_optionsMenu;
    public Sound m_menuSelect;
    public Sound m_startGameSound;

    public void OnNewGame()
    {
        GameManager.Get().OnNewGame();
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
}
