using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debug_StageSelect : MonoBehaviour
{
#if UNITY_EDITOR
    TextMeshProUGUI m_stageDisp;

    // Start is called before the first frame update
    void Start()
    {
        m_stageDisp = GetComponent<TextMeshProUGUI>();    
    }

    // Update is called once per frame
    void Update()
    {
        SaveData save = SaveData.Get();
        int level = save.GetCurrentLevel();
        int oldLevel = level;
        if (Input.GetKeyDown(KeyCode.Minus))
            --level;
        if (Input.GetKeyDown(KeyCode.Equals))
            ++level;
        level = Mathf.Clamp(level, 0, GameManager.GetNumLevels() - 1);
        if (oldLevel != level)
            save.SetCurrentLevel(level);
        m_stageDisp.text = "Stage " + (level + 1).ToString();
    }
#else
    void Awake()
    {
        DestroyImmediate(gameObject);
    }
#endif
}
