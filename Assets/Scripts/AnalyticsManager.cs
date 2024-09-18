//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

//#define ANALYTICS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ANALYTICS
using Firebase;
using Firebase.Analytics;
#endif

public class AnalyticsManager : MonoBehaviour
{
    static AnalyticsManager s_theManager;
#if ANALYTICS
    FirebaseApp m_app;
#endif

    public static AnalyticsManager Get()
    {
        if (null == s_theManager)
        {   // just add this as a component to the existing GameManager object
            GameManager gm = GameManager.Get();
            s_theManager = gm.gameObject.AddComponent<AnalyticsManager>();
            s_theManager.Initialize();
        }
        return s_theManager;
    }

    // Start is called before the first frame update
    void Initialize()
    {
#if ANALYTICS
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                m_app = FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#endif
    }

    public void LevelStart(int levelNum)
    {
#if ANALYTICS
        if (null != m_app)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart,
                new Parameter(FirebaseAnalytics.ParameterLevel, levelNum));
        }
#endif
    }

    public void LevelComplete(int levelNum)
    {
#if ANALYTICS
        if (null != m_app)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd,
                new Parameter(FirebaseAnalytics.ParameterLevel, levelNum));
        }
#endif
    }

    public void GameOver(int levelNum)
    {
#if ANALYTICS
        if (null != m_app)
        {
            FirebaseAnalytics.LogEvent("GameOver",
                new Parameter(FirebaseAnalytics.ParameterLevel, levelNum));
        }
#endif
    }

    public void ShowAd(string adId)
    {
#if ANALYTICS
        if (null != m_app)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression,
                new Parameter(FirebaseAnalytics.ParameterAdUnitName, adId));
        }
#endif
    }
}
