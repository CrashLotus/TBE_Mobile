using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_moveSpeed = 7.0f;
    public float m_accel = 70.0f;
    public Weapon m_laserWeapon;

    Joystick m_joystick;
    SpriteRenderer m_sprite;
    Vector3 m_vel = Vector3.zero;
    bool m_fireLaser = false;
    bool m_fireLaserOld = false;


    // Start is called before the first frame update
    void Start()
    {
        m_joystick = FindObjectOfType<Joystick>();
        m_sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        Vector3 move = new Vector3(m_joystick.Horizontal, m_joystick.Vertical, 0.0f);
        if (move.x < 0.0f)
            m_sprite.flipX = true;
        else if (move.x > 0.0f)
            m_sprite.flipX = false;
        Vector3 vel = move * m_moveSpeed;
        Vector3 dV = vel - m_vel;
        float accel = dV.magnitude;
        float maxAccel = m_accel * dt;
        if (accel > maxAccel)
            dV = dV / accel * maxAccel;
        m_vel += dV;

        Vector3 pos = transform.position;
        pos += m_vel * dt;
        transform.position = pos;

        bool fireLaser = m_fireLaser && (!m_fireLaserOld);

        // fire the laser
        if (fireLaser)
            m_laserWeapon.HitTrigger();
        if (m_fireLaser)
            m_laserWeapon.HoldTrigger();
        m_fireLaserOld = m_fireLaser;
        m_fireLaser = false;
    }

    public void OnFireButton()
    {
        m_fireLaser = true;
    }
}
