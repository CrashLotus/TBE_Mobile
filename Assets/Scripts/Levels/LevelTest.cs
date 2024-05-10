using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string level = GameManager.GetCurrentLevelName();
        //string level = "Levels/TestWorm";
        StartCoroutine(LoadLevel(level));     
    }

    IEnumerator LoadLevel(string name)
    {
        Debug.Log("Loading level " + name);
        ResourceRequest loadLevel = Resources.LoadAsync<Level>(name);
        yield return loadLevel;
        Debug.Log("Start Level");
        Level level = Instantiate(loadLevel.asset as Level);
        level.Start();
        while (false == level.IsDone())
        {   // wait for the level to end
            if (level.IsAbort())
                yield break;
            level.Update();
            yield return null;
        }
        Debug.Log("Level Over");
        Resources.UnloadAsset(loadLevel.asset);
        // just in case, make sure the scene is completely cleared out
        while ( (Egg.GetCount() > 0)
            || (EnemyBird.GetCount() > 0)
            || (Worm.GetCount() > 0)
            )
        {
            yield return null;
        }
        // all done
        if (level.IsAbort())
            yield break;
        GameManager.Get().StageClear();
    }
}
