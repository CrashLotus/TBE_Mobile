using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    public List<string> m_levelName;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLevel(m_levelName[0]));     
    }

    // mrwTODO
    IEnumerator DoSequnece()
    {
        foreach (string level in m_levelName)
        {
            yield return LoadLevel(level);
        }
        Debug.Log("All Levels Complete");
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
