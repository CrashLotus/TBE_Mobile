//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    bool m_isPaused = false;
    float m_restoreSpeed = 1.0f;
    
    Animator m_anim;


    void Start()
    {
        m_anim = GetComponent<Animator>();
        if (null == m_anim)
        {
            Debug.LogError("PlayerAnim doesn't have a Animator on it");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Get().GetPause())
        {
            if (false == m_isPaused)
            {   // pause it
                m_restoreSpeed = m_anim.speed;
                m_anim.speed = 0.0f;
                m_isPaused = true;
            }
        }
        else
        {
            if (m_isPaused)
            {   // restore
                m_anim.speed = m_restoreSpeed;
                m_isPaused = false;
            }
        }
    }
}
