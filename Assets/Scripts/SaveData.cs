//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

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
        public int m_score = 0;
        public int m_currentLevel = 0;
        public int m_playerHP = Player.s_startingHP;
        public int m_timeCrystals = 0;
        public int m_timeWarp = 0;
        public List<string> m_upgrades = new List<string>();
        public List<string> m_tutorial = new List<string>();
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

    public void ResetGame()
    {
        m_data.m_score = 0;
        m_data.m_currentLevel = 0;
        m_data.m_playerHP = Player.s_startingHP;
        m_data.m_tutorial.Clear();
        Save();
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

    public bool HasSeenTutorial(string key)
    {
        return m_data.m_tutorial.Contains(key);
    }

    public void SeeTutorial(string key)
    {
        if (false == HasSeenTutorial(key))
        {
            m_data.m_tutorial.Add(key);
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

    public int GetTimeWarp()
    {
        return m_data.m_timeWarp;
    }

    public void SetTimeWarp(int points)
    {
        m_data.m_timeWarp = points;
        Save();
    }

    public int GetScore()
    {
        return m_data.m_score;
    }

    public void SetScore(int score)
    {
        m_data.m_score = score;
        Save();
    }
}
