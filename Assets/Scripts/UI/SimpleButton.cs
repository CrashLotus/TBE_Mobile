using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool m_isDown = false;

    public bool IsButtonHold()
    {
        return m_isDown;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isDown = true;
    }
}
