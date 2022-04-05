using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float m_noMoveZone = 10.0f;

    Player m_player;

    // Start is called before the first frame update
    void Start()
    {
        m_player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        float deltaX = m_player.transform.position.x - pos.x;
        if (deltaX > m_noMoveZone)
            deltaX -= m_noMoveZone;
        else if (deltaX < -m_noMoveZone)
            deltaX += m_noMoveZone;
        else
            deltaX = 0;
        pos.x += deltaX;
        transform.position = pos;
    }
}
