using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : ScriptableObject
{
    public string m_label;
    public string m_text;

    protected bool m_isActive;
    protected bool m_isDone;

    public Wave()
    {
        m_isActive = false;
        m_isDone = false;
        m_text = null;
    }

    public virtual void Start()
    {
        m_isActive = true;
        m_isDone = false;
        if (null != m_text && m_text.Length > 0)
        {
            GameUI.Get().SetLabel(m_text);
        }
    }

    public virtual void Stop()
    {
        m_isActive = false;
        m_isDone = true;
    }

    public virtual void Update()
    {
        // Does nothing by default
    }

    public bool IsActive()
    {
        return m_isActive;
    }

    public virtual bool IsDone()
    {
        return m_isDone;
    }

    public virtual bool IsWait()
    {
        return false;
    }

    public string GetLabel()
    {
        return m_label;
    }

    public virtual void Reset()
    {
        m_isActive = false;
        m_isDone = false;
    }
}
