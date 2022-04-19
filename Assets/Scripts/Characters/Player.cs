using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Bird, IHitPoints
{
    public float m_moveSpeed = 7.0f;
    public float m_accel = 70.0f;
    public Weapon m_laserWeapon;
    public SimpleButton m_fireButton;
    public float m_topBoundary = 0.94f;
    public float m_bottomBoundary = 0.16f;

    Joystick m_joystick;
    Vector3 m_vel = Vector3.zero;
    bool m_fireLaser = false;
    bool m_fireLaserOld = false;

    const int s_maxEggStart = 10;
    const int s_maxEggFinish = 20;
    const float s_startingHP = 3.0f;

    static Player s_thePlayer;
    static int s_score = 0;
    static float s_saveHitPoints = s_startingHP;
    //mrwTODO these were for achievements
    static int s_bossesKilled = 0;
    static int s_bossesMissiled = 0;
    static int s_enemiesKilled = 0;
    static int s_enemiesMissiled = 0;
    static int s_enemiesLasered = 0;
    static int s_eggsCaught = 0;

    public static Player Get()
    {
        return s_thePlayer;
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_joystick = FindObjectOfType<Joystick>();
        m_hitPoints = s_saveHitPoints;
        s_thePlayer = this;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float dt = Time.deltaTime;

        // Input
        Vector3 move = new Vector3(m_joystick.Horizontal, m_joystick.Vertical, 0.0f);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            move.y += 1.0f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            move.y -= 1.0f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            move.x += 1.0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            move.x -= 1.0f;

        m_fireLaser = m_fireButton.IsButtonHold();
        m_fireLaser |= Input.GetKey(KeyCode.Space);
        bool fireLaser = m_fireLaser && (!m_fireLaserOld);

        // face the right direction
        if (move.x < 0.0f)
            m_sprite.flipX = true;
        else if (move.x > 0.0f)
            m_sprite.flipX = false;

        // update velocity
        Vector3 vel = move * m_moveSpeed;
        Vector3 dV = vel - m_vel;
        float accel = dV.magnitude;
        float maxAccel = m_accel * dt;
        if (accel > maxAccel)
            dV = dV / accel * maxAccel;
        m_vel += dV;

        Vector3 pos = transform.position;
        pos += m_vel * dt;

        // constrain the player to the top and bottom of the screen
        Vector3 topLeft = new Vector3(0.0f, m_topBoundary, 0.0f);   // top-left corner in view coords
        topLeft = Camera.main.ViewportToWorldPoint(topLeft);    // converted to world coords
        Vector3 botRight = new Vector3(1.0f, m_bottomBoundary, 0.0f);   // bottom-right corner in view coords
        botRight = Camera.main.ViewportToWorldPoint(botRight);    // converted to world coords
        pos.y = Mathf.Clamp(pos.y, botRight.y, topLeft.y);

        transform.position = pos;

        // fire the laser
        if (fireLaser)
            m_laserWeapon.HitTrigger();
        if (m_fireLaser)
            m_laserWeapon.HoldTrigger();
        m_fireLaserOld = m_fireLaser;
        m_fireLaser = false;

        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
//        Debug.Log("Player hit a " + collision.gameObject.name);
    }

    public IHitPoints.DamageReturn Damage(float damage, IHitPoints.HitType hitType)
    {
#if false   //mrwTODO
        if (m_eggShieldOn)
        {
            m_eggShieldOn = false;
            AudioComponent.Get().PlaySound("BubbleBurst");
            return DamageReturn.NO_DAMAGE;
        }

        m_comboTimer = 0.0f;
        m_comboPoints = 0;

        int shake = Math.Min((int)damage, s_hitShake.Length - 1);
        Camera.Shake(s_hitShake[shake].X, s_hitShake[shake].Y);

#if CHEAT_INVULNERABLE
        return DamageReturn.NO_DAMAGE;
#endif
#endif
        //        EggBonus startBonus = GetBonusMode(); //mrwTODO

        int maxEgg = MaxEgg();
        if (m_hitPoints > maxEgg)   // you can have more than max eggs, but you can't have more than max hit points
            m_hitPoints = maxEgg;
        float eggDamage = Mathf.Min(damage, m_hitPoints);
        int numEgg = (int)eggDamage;
        Vector3 pos = transform.position;
        while (numEgg >= 3)
        {
            Egg.Spawn(pos, 3);
            numEgg -= 3;
        }
        while (numEgg >= 2)
        {
            Egg.Spawn(pos, 2);
            numEgg -= 2;
        }
        while (numEgg >= 1)
        {
            Egg.Spawn(pos, 1);
            numEgg -= 1;
        }

        //        AudioComponent.Get().PlaySound("Punch");  //mrwTODO
        IHitPoints.DamageReturn ret = IHitPoints.DamageReturn.PASS_THROUGH;
        ++m_hitByType[(int)hitType];
        m_lastHit = hitType;
        if (m_hitPoints > 0.0f)
        {
            m_hitPoints -= damage;
            if (m_hitPoints <= 0.0f)
            {
                Explode();
                ret = IHitPoints.DamageReturn.KILLED;    // I've been killed
            }
            ret = IHitPoints.DamageReturn.DAMAGED;
        }

#if false   //mrwTODO
        if (DamageReturn.KILLED == ret)
        {
            // shut off the laser - if it is firing
            m_megaLaser.ReleaseTrigger();
            m_megaLaser.Update(0.0f);
            Game1.SetState(Game1.State.GAME_OVER);
        }
        else if (GetBonusMode() != startBonus)
        {
            AudioComponent.Get().PlaySound("PowerDown");
        }
#endif
        return ret;
    }

    public static void AddScore(int score)
    {
        int oldScore = s_score;
        //mrwTODO
        //if (score > 0 && Player.IsAbilityUnlocked(Upgrade.Type.EXTRAPOINTS))
        //    score += 3 * score / 10;
        s_score += score;
        //if (s_score < 0)
        //    s_score = oldScore;
    }
    public static int GetScore()
    {
        return s_score;
    }

    public static void KilledEnemy(IHitPoints.HitType hitType)
    {
        ++s_enemiesKilled;
        if (IHitPoints.HitType.MISSILE == hitType)
        {
            ++s_enemiesMissiled;
        }
        if (IHitPoints.HitType.LASER == hitType)
        {
            ++s_enemiesLasered;
        }
    }

    public static void KilledBoss(IHitPoints.HitType hitType)
    {
        ++s_bossesKilled;
        if (IHitPoints.HitType.MISSILE == hitType)
            ++s_bossesMissiled;
    }

    public void AddEgg()
    {   //mrwTODO egg bonuses
//        EggBonus startBonus = GetBonusMode();

        int maxEgg = MaxEgg();
//        if (IsAbilityUnlocked(Upgrade.Type.MEGALASER))
//            maxEgg *= 3;
//        else if (IsAbilityUnlocked(Upgrade.Type.MULTISHOT))
//            maxEgg *= 2;
        m_hitPoints += 1.0f;
        if (m_hitPoints > maxEgg)
            m_hitPoints = maxEgg;

        //EggBonus endBonus = GetBonusMode();
        //if (endBonus != startBonus)
        //{
        //    switch (endBonus)
        //    {
        //        case EggBonus.POWER_LASER:
        //            if (IsAbilityUnlocked(Upgrade.Type.EGGSHIELD))
        //            {
        //                m_eggShieldOn = true;
        //                AudioComponent.Get().PlaySound("BubbleShieldBirth");
        //            }
        //            AudioComponent.Get().PlaySound("PowerUp1");
        //            EggIcon.StartWave();
        //            break;
        //        case EggBonus.MULTISHOT:
        //            if (IsAbilityUnlocked(Upgrade.Type.EGGSHIELD))
        //            {
        //                m_eggShieldOn = true;
        //                AudioComponent.Get().PlaySound("BubbleShieldBirth");
        //            }
        //            AudioComponent.Get().PlaySound("PowerUp2");
        //            EggIcon.StartWave();
        //            break;
        //        case EggBonus.MEGA_LASER:
        //            if (IsAbilityUnlocked(Upgrade.Type.EGGSHIELD))
        //            {
        //                m_eggShieldOn = true;
        //                AudioComponent.Get().PlaySound("BubbleShieldBirth");
        //            }
        //            AudioComponent.Get().PlaySound("PowerUp3");
        //            EggIcon.StartWave();
        //            break;
        //    }
        //}
        ++s_eggsCaught;
    }

    public int NumEgg()
    {
        int numEgg = (int)m_hitPoints;
        if (numEgg < 0)
            numEgg = 0;
        return numEgg;
    }

    public static int MaxEgg()
    {
#if false   //mrwTODO
        if (IsAbilityUnlocked(Upgrade.Type.EGG5))
            return s_maxEggStart + 5;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG4))
            return s_maxEggStart + 4;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG3))
            return s_maxEggStart + 3;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG2))
            return s_maxEggStart + 2;
        else if (IsAbilityUnlocked(Upgrade.Type.EGG1))
            return s_maxEggStart + 1;
#endif
        return s_maxEggStart;
    }

    void Explode()
    {
        //mrwTODO nothing for now
    }
}
