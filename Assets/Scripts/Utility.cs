//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static readonly Color s_hitFlashColor = new Color(1.0f, 0.0f, 0.0f, 0.0f);
    public const float s_hitFlashTime = 0.05f;

    /// <summary>
    /// This is the intersection of the screen-space screenPos with the plane z=zDepth
    /// </summary>
    /// <param name="screenPos">The input coordinate in screen space</param>
    /// <returns>The output coordinate in world space</returns>
    public static Vector3 ScreenToWorldPos(Vector3 screenPos, float zDepth=0.0f)
    {
        screenPos.z = 1.0f;
        Vector3 worldPos_near = Camera.main.ScreenToWorldPoint(screenPos);
        screenPos.z = 10.0f;
        Vector3 worldPos_far = Camera.main.ScreenToWorldPoint(screenPos);
        float t = (worldPos_near.z - zDepth) / (worldPos_near.z - worldPos_far.z);
        return (worldPos_far - worldPos_near) * t + worldPos_near;
    }

    public static float WrapAngleDeg(float angle)
    {
        while (angle > 180.0f)
            angle -= 360.0f;
        while (angle <= -180.0f)
            angle += 360.0f;

        return angle;
    }

    public static void HitFlash(GameObject gameObject)
    {
        if (gameObject.activeInHierarchy)
        {
            MonoBehaviour mono = gameObject.GetComponent<MonoBehaviour>();
            if (null != mono)
                mono.StartCoroutine(HitFlashUpdate(gameObject));
        }
    }

    static IEnumerator HitFlashUpdate(GameObject gameObject)
    {
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        if (null == sprite)
            sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        if (null == sprite)
            yield break;

        sprite.color = s_hitFlashColor;
        float timer = s_hitFlashTime;
        while (timer > 0.0f)
        {
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }
        sprite.color = Color.white;
    }

    public static void HitFlashReset(GameObject gameObject)
    {
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        if (null == sprite)
            sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        if (null == sprite)
            return;
        sprite.color = Color.white;
    }
}
