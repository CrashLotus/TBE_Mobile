using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool m_isDown = false;
    bool m_wasDown = false;

    public bool IsButtonHold()
    {
        return m_isDown;
    }

    public bool IsButtonPress()
    {
        return m_isDown && (false == m_wasDown);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isDown = true;
    }

    void LateUpdate()
    {
        m_wasDown = m_isDown;
    }
}
