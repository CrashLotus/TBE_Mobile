using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnNewGame()
    {
        GameManager.Get().OnNewGame();
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }
}
