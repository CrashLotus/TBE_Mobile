using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBubbles : MonoBehaviour
{
    public GameObject m_bubblePrefab;
    public float m_bubbleTimeMin = 0.04f;
    public float m_bubbleTimeMax = 0.12f;
    public float m_lavaTop = -3.0f;
    float m_bubbleTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        m_bubbleTimer -= Time.deltaTime;
        if (m_bubbleTimer <= 0.0f)
        {
            m_bubbleTimer = Random.Range(m_bubbleTimeMin, m_bubbleTimeMax);
            float x = Camera.main.aspect * Random.Range(-1.0f, 1.0f) * Camera.main.orthographicSize;
            float y = Random.Range(-Camera.main.orthographicSize, m_lavaTop);
            Vector3 pos = new Vector3(x, y, -1.0f + y / 20.0f);
            pos.x += Camera.main.transform.position.x;
            ObjectPool.Allocate(m_bubblePrefab, 32, pos);
        }
    }
}
