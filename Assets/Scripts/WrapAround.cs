using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapAround : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 offset = pos - Camera.main.transform.position;
        float worldWidth = GameManager.Get().m_worldWidth;
        while (offset.x < -worldWidth)
            offset.x += 2.0f * worldWidth;
        while (offset.x > worldWidth)
            offset.x -= 2.0f * worldWidth;
        pos = Camera.main.transform.position + offset;
        transform.position = pos;
    }
}
