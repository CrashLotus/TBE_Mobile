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
                save.SeeTutorial("EGG_FROM_ENEMY");
                GameUI.Get().SetTutorial("Eggs represent your health. Catch eggs to regain lost health.", pos);
                GameManager.Get().SetPause(true);
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
                save.SeeTutorial("EGG_HEALTH");
                GameUI.Get().SetTutorial("Enemies release eggs when defeated.", pos);
                GameManager.Get().SetPause(true);
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
                save.SeeTutorial("EGG_SPAWN_ENEMY");
                GameUI.Get().SetTutorial("If you let it hit the lava, an egg will spawn an enemy.", pos);
                GameManager.Get().SetPause(true);
            }
        }
    }
}
