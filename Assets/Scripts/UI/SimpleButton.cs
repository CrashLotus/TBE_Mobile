using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    bool m_isDown = false;
    bool m_wasDown = false;
    Vector2 m_touchPos;
    Vector2 m_touchStart;

    public bool IsButtonHold()
    {
        return m_isDown;
    }

    public bool IsButtonPress()
    {
        return m_isDown && (false == m_wasDown);
    }

    public Vector2 GetTouchPos()
    {
        return m_touchPos;
    }

    public Vector2 GetTouchStart()
    {
        return m_touchStart;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isDown = true;
        m_touchPos = m_touchStart = eventData.position;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (m_isDown)
            m_touchPos = eventData.position;
    }

    void LateUpdate()
    {
        m_wasDown = m_isDown;
    }

    public void Show(bool show)
    {
        m_isDown = false;
        m_wasDown = false;
        gameObject.SetActive(show);
    }
}
