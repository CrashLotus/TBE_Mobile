using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string m_key;
    public string m_name;
    public int m_cost;
    public string m_desc;
    public Sprite m_icon;
    public Sprite m_display;
}
