using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string level = GameManager.GetCurrentLevelName();
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
        {
            level.Update();
            yield return null;
        }
        Debug.Log("Level Over");
        Resources.UnloadAsset(loadLevel.asset);
        GameManager.Get().StageClear();
    }
}
