using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Bird, IHitPoints
{
    public enum EggBonus
    {
        NONE,
        POWER_LASER,
        MULTISHOT,
        MEGA_LASER
    }

    public float m_accel = 70.0f;
    public SimpleButton m_steering;
    public float m_gainX = 0.02f;
    public float m_gainY = 0.2f;
    public Weapon m_laserWeapon;
    public SimpleButton m_fireButton;
    public SimpleButton m_fireLeft;
    public SimpleButton m_fireRight;
    public Weapon m_missileWeapon;
    public SimpleButton m_missileButton;
    public GameObject m_eggShield;
    public float m_topBoundary = 0.94f;
    public float m_bottomBoundary = 0.16f;
    public Sound m_hitSound;
    public Sound m_powerUp1;
    public Sound m_powerUp2;
    public Sound m_powerUp3;
    public Sound m_powerDown;
    public Sound m_eggShieldOn;
    public Sound m_eggShieldOff;

    Vector3 m_vel = Vector3.zero;
    bool m_fireLaser = false;
    bool m_fireLaserOld = false;
    bool m_isEggShieldOn = false;
    Animator m_eggShieldAnim;

    const int s_maxEggStart = 10;
    public const int s_startingHP = 3;

    static Player s_thePlayer;
    static int s_score = 0;
    //mrwTODO these were for achievements
    static int s_bossesKilled = 0;
    static int s_bossesMissiled = 0;
    static int s_enemiesKilled = 0;
    static int s_enemiesMissiled = 0;
    static int s_enemiesLasered = 0;
    static int s_eggsCaught = 0;

    public static Player Get()
    {
        if (null != s_thePlayer && s_thePlayer.m_hitPoints > 0)
            return s_thePlayer;
        return null;
    }

    public void WarmUp()
    {
        m_laserWeapon.WarmUp();
    }

    public override void Init(ObjectPool pool)
    {
        base.Init(pool);
        m_hitPoints = SaveData.Get().GetPlayerHP();
        m_isEggShieldOn = false;
        m_eggShieldAnim = m_eggShield.GetComponent<Animator>();
        m_eggShield.SetActive(false);
        
        // check the egg shield
        SaveData data = SaveData.Get();
        if (data.HasUpgrade("EGGSHIELD"))
        {
            EggBonus startBonus = GetBonusMode();
            if (EggBonus.NONE != startBonus)
                EggShieldOn();
        }

        if (null != m_missileButton)
        {
            if (SaveData.Get().HasUpgrade("MISSILE1"))
                m_missileButton.gameObject.SetActive(true);
            else
                m_missileButton.gameObject.SetActive(false);
        }
        s_thePlayer = this;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float dt = Time.deltaTime;
        Vector3 pos = transform.position;

        // Input
        m_fireLaser = m_fireButton.IsButtonHold() | m_fireLeft.IsButtonHold() | m_fireRight.IsButtonHold();
        m_fireLaser |= Input.GetKey(KeyCode.Space);
        bool fireLaser = m_fireLaser && (!m_fireLaserOld);

        bool fireMissile = m_missileButton.IsButtonPress();
        fireMissile |= Input.GetKeyDown(KeyCode.Tab);

        // update velocity
        Vector3 vel = Vector3.zero;
        Joystick joystick = GameUI.Get().GetJoystick();
        if (null == joystick)
        {
            if (m_steering.IsButtonHold() && dt > 0.0f)
            {
                Vector3 screenPos = Camera.main.WorldToViewportPoint(pos);
                Vector3 target = Camera.main.ScreenToViewportPoint(m_steering.GetTouchPos());
                Vector3 start = Camera.main.ScreenToViewportPoint(m_steering.GetTouchStart());
                Vector3 delta = Vector3.zero;
                delta.x = target.x - start.x;
                delta.y = target.y - screenPos.y;
                vel = new Vector3(m_gainX * delta.x, m_gainY * delta.y, 0.0f);
                vel.x = Mathf.Clamp(vel.x, -m_horizSpeed, m_horizSpeed);
                vel.y = Mathf.Clamp(vel.y, -m_vertSpeed, m_vertSpeed);
            }
        }
        else
        {
            Vector3 move = new Vector3(joystick.Horizontal, joystick.Vertical, 0.0f);
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                move.y += 1.0f;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                move.y -= 1.0f;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                move.x += 1.0f;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                move.x -= 1.0f;
            vel = new Vector3(move.x * m_horizSpeed, move.y * m_vertSpeed, 0.0f);
        }
        Vector3 dV = vel - m_vel;
        float accel = dV.magnitude;
        float maxAccel = m_accel * dt;
        if (accel > maxAccel)
            dV = dV / accel * maxAccel;
        m_vel += dV;

        pos += m_vel * dt;

        switch(PlayerPrefs.GetInt("steering", 0))
        {
            case 0: // free aim
                if (m_vel.x < 0.0f)
                    m_sprite.flipX = true;
                else if (m_vel.x > 0.0f)
                    m_sprite.flipX = false;
                break;
            case 1: // face right
                m_sprite.flipX = false;
                break;
            case 2: // locked fire
                if (false == m_fireLaser)
                {
                    if (m_vel.x < 0.0f)
                        m_sprite.flipX = true;
                    else if (m_vel.x > 0.0f)
                        m_sprite.flipX = false;
                }
                break;
        }

        if (m_fireLeft.IsButtonHold())
            m_sprite.flipX = true;
        if (m_fireRight.IsButtonHold())
            m_sprite.flipX = false;

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

        // fire the missile
        if (fireMissile)
            m_missileWeapon.HitTrigger();

        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
//        Debug.Log("Player hit a " + collision.gameObject.name);
    }

    public IHitPoints.DamageReturn Damage(int damage, IHitPoints.HitType hitType)
    {
        if (m_isEggShieldOn)
        {
            m_isEggShieldOn = false;
            m_eggShieldAnim.SetBool("ShieldOn", false);
            m_eggShieldOff.Play();
            return IHitPoints.DamageReturn.NO_DAMAGE;
        }

#if false
        m_comboTimer = 0.0f;
        m_comboPoints = 0;

        int shake = Math.Min((int)damage, s_hitShake.Length - 1);
        Camera.Shake(s_hitShake[shake].X, s_hitShake[shake].Y);

#if CHEAT_INVULNERABLE
        return DamageReturn.NO_DAMAGE;
#endif
#endif
        EggBonus startBonus = GetBonusMode();

        int maxEgg = MaxEgg();
        if (m_hitPoints > maxEgg)   // you can have more than max eggs, but you can't have more than max hit points
            m_hitPoints = maxEgg;
        int eggDamage = Mathf.Min(damage, m_hitPoints);
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

        if (null != m_hitSound)
            m_hitSound.Play();
        IHitPoints.DamageReturn ret = IHitPoints.DamageReturn.PASS_THROUGH;
        ++m_hitByType[(int)hitType];
        m_lastHit = hitType;
        if (m_hitPoints > 0)
        {
            ret = IHitPoints.DamageReturn.DAMAGED;
            m_hitPoints -= damage;
            if (m_hitPoints <= 0)
            {
                Explode();
                ret = IHitPoints.DamageReturn.KILLED;    // I've been killed
            }
        }

        if (IHitPoints.DamageReturn.KILLED == ret)
        {
            //mrwTODO shut off the laser - if it is firing
            //            m_megaLaser.ReleaseTrigger();
            //            m_megaLaser.Update(0.0f);
            GameManager.Get().GameOver();
            gameObject.SetActive(false);
        }
        else if (GetBonusMode() != startBonus)
        {
            m_powerDown.Play();
        }
        return ret;
    }

    public EggBonus GetBonusMode()
    {
        int maxEgg = MaxEgg();
        SaveData data = SaveData.Get();
        if (data.HasUpgrade("MEGALASER") && m_hitPoints >= 3 * maxEgg)
            return EggBonus.MEGA_LASER;
        if (data.HasUpgrade("MULTISHOT") && m_hitPoints >= 2 * maxEgg)
            return EggBonus.MULTISHOT;
        if (data.HasUpgrade("POWERLASER") && m_hitPoints >= maxEgg)
            return EggBonus.POWER_LASER;
        return EggBonus.NONE;
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

    void EggShieldOn()
    {
        if (false == m_isEggShieldOn)
        {
            m_isEggShieldOn = true;
            m_eggShield.SetActive(true);
            m_eggShieldAnim.SetBool("ShieldOn", true);
            m_eggShieldOn.Play();
        }
    }

    public void AddEgg()
    {
        SaveData data = SaveData.Get();
        EggBonus startBonus = GetBonusMode();

        int maxEgg = MaxEgg();
        if (data.HasUpgrade("MEGALASER"))
            maxEgg *= 3;
        if (data.HasUpgrade("MULTISHOT"))
            maxEgg *= 2;
        m_hitPoints += 1;
        if (m_hitPoints > maxEgg)
            m_hitPoints = maxEgg;

        EggBonus endBonus = GetBonusMode();
        if (endBonus != startBonus)
        {
            switch (endBonus)
            {
                case EggBonus.POWER_LASER:
                    if (data.HasUpgrade("EGGSHIELD"))
                    {
                        EggShieldOn();
                    }
                    m_powerUp1.Play();
                    HitPoint_UI.Get().StartWave();
                    break;
                case EggBonus.MULTISHOT:
                    if (data.HasUpgrade("EGGSHIELD"))
                    {
                        EggShieldOn();
                    }
                    m_powerUp2.Play();
                    HitPoint_UI.Get().StartWave();
                    break;
                case EggBonus.MEGA_LASER:
                    if (data.HasUpgrade("EGGSHIELD"))
                    {
                        EggShieldOn();
                    }
                    m_powerUp3.Play();
                    HitPoint_UI.Get().StartWave();
                    break;
            }
        }
        ++s_eggsCaught;
    }

    public static int NumEgg()
    {
        if (null == s_thePlayer)
            return SaveData.Get().GetPlayerHP();

        int numEgg = s_thePlayer.m_hitPoints;
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
