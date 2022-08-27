using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class SaveData
{
    static SaveData s_theData;

    public class Data
    {
        public int m_currentLevel = 0;
        public int m_playerHP = Player.s_startingHP;
        public int m_timeCrystals = 0;
        public List<string> m_upgrades = new List<string>();
    }
    Data m_data;

    public static SaveData Get()
    {
        if (null == s_theData)
        {
            s_theData = new SaveData();
            s_theData.Load();
        }
        return s_theData;
    }

    static string GetFilename()
    {
        string filename = Application.persistentDataPath + "/tbe.xml";
        return filename;
    }

    void Load()
    {
        string filename = GetFilename();
        if (File.Exists(filename))
        {
            StreamReader sr = new StreamReader(filename);
            XmlSerializer xmls = new XmlSerializer(typeof(Data));
            m_data = xmls.Deserialize(sr) as Data;
            sr.Close();
        }
        else
        {
            m_data = new Data();
        }
    }

    public void Save()
    {
        string filename = GetFilename();
        XmlSerializer xmls = new XmlSerializer(typeof(Data));
        StreamWriter sw = new StreamWriter(filename);
        xmls.Serialize(sw, m_data);
        sw.Close();
    }

    public bool HasUpgrade(string key)
    {
        return m_data.m_upgrades.Contains(key);
    }

    public void AddUpgrade(string key)
    {
        if (false == HasUpgrade(key))
        {
            m_data.m_upgrades.Add(key);
            Save();
        }
    }

    public int GetTimeCrystals()
    {
        return m_data.m_timeCrystals;
    }

    public void AddTimeCrystals(int num)
    {
        m_data.m_timeCrystals += num;
        Save();
    }

    public void Reset()
    {
        m_data = new Data();
        Save();
    }

    public int GetCurrentLevel()
    {
        return m_data.m_currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        m_data.m_currentLevel = level;
        Save();
    }

    public int GetPlayerHP()
    {
        return m_data.m_playerHP;
    }

    public void SetPlayerHP(int hp)
    {
        m_data.m_playerHP = hp;
        Save();
    }
}
