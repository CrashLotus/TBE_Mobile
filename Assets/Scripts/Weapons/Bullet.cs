using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float m_speed = 14.0f;
    protected Vector3 m_vel;

    public virtual void Fire(Vector3 pos, Vector3 dir)
    {
        transform.position = pos;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x));
        m_vel = dir * m_speed;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.flipX = dir.x < 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos += m_vel * Time.deltaTime;
        transform.position = pos;
    }
}
