using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
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
}
