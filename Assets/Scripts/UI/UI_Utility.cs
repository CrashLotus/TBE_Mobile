//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

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
