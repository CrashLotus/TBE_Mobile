using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float m_parallax = 0.5f;

    Vector3 m_startPos;
    Vector3 m_cameraStart;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_startPos = transform.position;
        m_cameraStart = Camera.main.transform.position;
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        Vector3 delta = Camera.main.transform.position - m_cameraStart;
        Vector3 pos = m_startPos;
        pos += delta * (1.0f - m_parallax);
        transform.position = pos;
    }
}
