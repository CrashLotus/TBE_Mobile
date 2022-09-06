using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float m_noMoveZoneLeft = 4.0f;
    public float m_noMoveZoneRight = 2.0f;

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
        if (deltaX > m_noMoveZoneRight)
            deltaX -= m_noMoveZoneRight;
        else if (deltaX < -m_noMoveZoneLeft)
            deltaX += m_noMoveZoneLeft;
        else
            deltaX = 0;
        pos.x += deltaX;
        transform.position = pos;
    }
}
