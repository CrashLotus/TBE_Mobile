//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapAround : MonoBehaviour
{
    void LateUpdate()
    {
        transform.position = WrapPosition(transform.position);
    }

    public static Vector3 WrapPosition(Vector3 pos)
    {
        Vector3 offset = pos - Camera.main.transform.position;
        float worldWidth = GameManager.Get().m_worldWidth;
        while (offset.x < -worldWidth)
            offset.x += 2.0f * worldWidth;
        while (offset.x > worldWidth)
            offset.x -= 2.0f * worldWidth;
        pos = Camera.main.transform.position + offset;
        return pos;
    }
}
