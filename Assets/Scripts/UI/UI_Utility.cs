using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Utility
{
    public static void SelectUI(GameObject obj)
    {
#if !UNITY_IOS && !UNITY_ANDROID
        EventSystem.current.SetSelectedGameObject(obj);
#endif
    }
}
