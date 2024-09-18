//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum Lesson
    {
        EGG_HEALTH,         // Catch the eggs to regain health
        EGG_FROM_ENEMY,     // Enemies release eggs
        EGG_SPAWN_ENEMY,    // If you let it hit the lava, an egg will spawn an enemy

        TOTAL
    }

    static TutorialManager s_theManager;

    public static TutorialManager Get()
    {
        if (null == s_theManager)
        {
            GameObject obj = GameManager.Get().gameObject;
            s_theManager = obj.AddComponent<TutorialManager>();
        }
        return s_theManager;
    }

    public void PlayerDamaged(Vector3 pos)
    {
        SaveData save = SaveData.Get();
        if (false == save.HasSeenTutorial("EGG_FROM_ENEMY"))
        {
            if (GameManager.Get().GetScreenBounds().Contains(pos))
            {
                GameUI.Get().SetTutorial("Eggs represent your health. Catch eggs to regain lost health.", pos, "EGG_FROM_ENEMY");
            }
        }
    }

    public void EnemyKilled(Vector3 pos)
    {
        SaveData save = SaveData.Get();
        if (false == save.HasSeenTutorial("EGG_HEALTH"))
        {
            if (GameManager.Get().GetScreenBounds().Contains(pos))
            {
                GameUI.Get().SetTutorial("Enemies release eggs when defeated.", pos, "EGG_HEALTH");
            }
        }
    }

    public void EggHitLava(Vector3 pos)
    {
        SaveData save = SaveData.Get();
        if (false == save.HasSeenTutorial("EGG_SPAWN_ENEMY"))
        {
            if (GameManager.Get().GetScreenBounds().Contains(pos))
            {
                GameUI.Get().SetTutorial("If you let it hit the lava, an egg will spawn an enemy.", pos, "EGG_SPAWN_ENEMY");
            }
        }
    }

    public void StartScene()
    {
        StartCoroutine(StartSceneCo());
    }

    IEnumerator StartSceneCo()
    {
        SaveData save = SaveData.Get();
        GameUI ui = GameUI.Get();
        while (null == ui)
        {
            ui = GameUI.Get();
            yield return null;
        }
        if (false == save.HasSeenTutorial("MISSILE_TILT"))
        {
            if (save.HasUpgrade("MISSILE1"))
            {
                ui.SetTutorial("You can fire a missile by tilting your device to the right.", Vector3.zero, "MISSILE_TILT");
                while (GameManager.Get().GetPause())
                    yield return null;
            }
        }
        if (false == save.HasSeenTutorial("TIMEWARP_TILT"))
        {
            if (save.HasUpgrade("BULLETTIME"))
            {
                ui.SetTutorial("You can activate Time Freeze by tilting your device to the left.", Vector3.zero, "TIMEWARP_TILT");
                while (GameManager.Get().GetPause())
                    yield return null;
            }
        }
    }
}
