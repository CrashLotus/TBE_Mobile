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
    public SimpleButton m_timeButton;
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
    float m_comboTimer = 0.0f;
    int m_comboPoints = 0;

    const int s_maxEggStart = 10;
    public const int s_startingHP = 3;

    static Player s_thePlayer;
    static int s_score = 0;
    static readonly Vector2[] s_hitShake =
    {
        new Vector2(2.0f, 0.3f),
        new Vector2(4.0f, 0.3f),
        new Vector2(10.0f, 0.4f),
    };
    const float s_comboDelay = 1.5f;        // kill a guy every 1.5 seconds to keep combo running
    const float s_comboCountDown = 0.25f;   // if you don't, they'll tick down every 1/4 second

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
            if (data.HasUpgrade("MISSILE1"))
                m_missileButton.gameObject.SetActive(true);
            else
                m_missileButton.gameObject.SetActive(false);
        }

        s_thePlayer = this;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime(true);
        Vector3 pos = transform.position;

        // Input
        m_fireLaser = m_fireButton.IsButtonHold() | m_fireLeft.IsButtonHold() | m_fireRight.IsButtonHold();
        m_fireLaser |= Input.GetKey(KeyCode.Space);
        bool fireLaser = m_fireLaser && (!m_fireLaserOld);

        bool fireMissile = m_missileButton.IsButtonPress();
        fireMissile |= Input.GetKeyDown(KeyCode.Tab);

        bool timeWarp = m_timeButton.IsButtonPress();
        timeWarp |= Input.GetKeyDown(KeyCode.BackQuote);

        // update velocity
        Vector3 vel = Vector3.zero;
        Joystick joystick = GameUI.Get().GetJoystick();
        if (null != joystick)
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

        if (m_vel.x < 0.0f)
            m_sprite.flipX = true;
        else if (m_vel.x > 0.0f)
            m_sprite.flipX = false;

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

        BulletTime bt = BulletTime.Get();
        if (bt.IsReady() && timeWarp)
        {
            bt.Begin();
            bt.Empty();
        }

        m_comboTimer -= dt;
        if (m_comboTimer <= 0.0f)
        {
            --m_comboPoints;
            if (m_comboPoints < 0)
                m_comboPoints = 0;
            else
                m_comboTimer += s_comboCountDown;
        }

        if (Input.GetKeyDown(KeyCode.P))
            ComboKill(transform.position);

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

        int shake = Mathf.Min((int)damage, s_hitShake.Length - 1);
        FollowCamera.Shake(s_hitShake[shake].x, s_hitShake[shake].y);

        m_comboTimer = 0.0f;
        m_comboPoints = 0;

#if CHEAT_INVULNERABLE
        return IHitPoints.DamageReturn.NO_DAMAGE;
#endif

        EggBonus startBonus = GetBonusMode();

        int maxEgg = MaxEgg();
        if (m_hitPoints > maxEgg)   // you can have more than max eggs, but you can't have more than max hit points
            m_hitPoints = maxEgg;
        int eggDamage = Mathf.Min(damage, m_hitPoints);
        if (IHitPoints.HitType.NONE == hitType)
            eggDamage = Mathf.Min(eggDamage, 3);    // max damage of 3 if you hit an enemy
        Debug.Log("Taking Damage " + eggDamage);
        int numEgg = (int)eggDamage;
        Vector3 pos = transform.position;
        TutorialManager.Get().PlayerDamaged(transform.position);
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
        if (m_hitPoints > 0)
        {
            ret = IHitPoints.DamageReturn.DAMAGED;
            m_hitPoints -= eggDamage;
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

    public static int GetEggMagnetLevel()
    {
        SaveData data = SaveData.Get();
        if (data.HasUpgrade("EGGMAGNET2"))
            return 2;
        if (data.HasUpgrade("EGGMAGNET1"))
            return 1;
        return 0;
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

    public static void ComboKill(Vector3 pos)
    {
        SaveData data = SaveData.Get();
        if (data.HasUpgrade("BULLETTIME"))
        {
            Player player = Player.Get();
            if (null != player)
            {
                if (player.m_comboPoints > 0)
                {
                    ComboNumber.Spawn(pos, GetComboPoints());
                    BulletTime.Get().AddPoints(Mathf.Min(player.m_comboPoints, 10));
                }
                ++player.m_comboPoints;
                player.m_comboTimer = s_comboDelay;
            }
        }
    }

    public static int GetComboPoints()
    {
        Player player = Player.Get();
        if (null != player)
            return player.m_comboPoints;
        return 0;
    }
}
