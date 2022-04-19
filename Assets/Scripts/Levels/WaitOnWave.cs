using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Waves/WaitOnWave")]
public class WaitOnWave : Wave
{
    string m_waitLabel;

    public override bool IsDone()
    {
        if (m_isDone)
            return true;

        if (null != m_label && m_label.Length > 0)
        {   // if you have a label, wait for that label to be done
            Wave waitWave = Level.Get().FindWave(m_waitLabel);
            if (null != waitWave)
                return waitWave.IsDone();
        }
        else
        {   // if you have no label, wait for everything up to this point to be done
            return false == Level.Get().IsActive();
        }

        return true;
    }

    public override bool IsWait()
    {
        return true;
    }
}
