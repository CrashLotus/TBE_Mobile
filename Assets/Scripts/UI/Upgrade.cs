using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string m_key;
    public string m_name;
    public int m_cost;
    [TextArea]
    public string m_desc;
    public Sprite m_icon;
    public Sprite m_display;
    public string m_preReq;

    public virtual bool IsOwned()
    {
        return SaveData.Get().HasUpgrade(m_key);
    }

    public virtual bool IsLocked()
    {
        if (null == m_preReq || m_preReq.Length == 0)
            return false;
        return false == SaveData.Get().HasUpgrade(m_preReq);
    }

    public virtual void Buy()
    {
        SaveData data = SaveData.Get();
        data.AddUpgrade(m_key);
    }
}
