using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Weapon
{
    public Texture[] m_textures;
    public float m_animSpeed = 12.0f;   // frames per second
    public float m_scrollSpeed = -1.0f;
    public GameObject m_hitSplash;
    LineRenderer m_line;
    Material m_mat;
    SpriteRenderer m_eyeSprite;
    AudioSource m_laserSound;
    float m_animTimer;
    int m_curFrame;
    float m_uvOffset = 0.0f;

    const float s_damage = 0.4f;
    const float s_force = 75.0f;

    public override void WarmUp()
    {
        base.WarmUp();
        ObjectPool.GetPool(m_hitSplash, 32);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_line = GetComponent<LineRenderer>();
        if (null == m_line)
        {
            Debug.LogError("Laser needs a LineRenderer");
            Destroy(gameObject);
            return;
        }
        m_mat = new Material(m_line.material);
        m_line.material = m_mat;
        m_eyeSprite = GetComponentInChildren<SpriteRenderer>();
        m_animTimer = 0.0f;
        m_curFrame = -1;
        m_line.enabled = false;
        m_eyeSprite.enabled = false;
        m_laserSound = GetComponent<AudioSource>();
        m_laserSound.Stop();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        float dt = BulletTime.Get().GetDeltaTime(true);

        // flip with owner
        Vector3 pos = m_offset;
        if (m_ownerSprite.flipX)
        {
            pos.x = -pos.x;
        }
        transform.localPosition = pos;

        Vector3 rightSide = new Vector3(1.0f, 0.5f, 0.0f);
        if (m_ownerSprite.flipX)
        {
            rightSide.x = -1.0f;
        }
        rightSide = Camera.main.ViewportToWorldPoint(rightSide);
        Vector3 start = transform.position;
        Vector3 end = start;
        end.x = rightSide.x;
        m_line.SetPosition(0, start);
        m_line.SetPosition(1, end);

        // Animation
        m_animTimer += m_animSpeed * dt;
        m_uvOffset += m_scrollSpeed * dt;
        m_uvOffset = m_uvOffset - (int)m_uvOffset;
        while (m_animTimer >= m_textures.Length)
        {
            m_animTimer -= m_textures.Length;
        }
        int animFrame = (int)m_animTimer;
        if (animFrame != m_curFrame)
        {
            m_curFrame = animFrame;
            m_mat.SetTexture("_MainTex", m_textures[m_curFrame]);
        }
        m_mat.SetTextureOffset("_MainTex", new Vector2(m_uvOffset, 0.0f));
    }

    public override void HoldTrigger()
    {
        base.HoldTrigger();
        m_line.enabled = true;
        m_eyeSprite.enabled = true;
        if (false == m_laserSound.isPlaying)
            m_laserSound.Play();
    }

    public override void ReleaseTrigger()
    {
        base.ReleaseTrigger();
        m_line.enabled = false;
        m_eyeSprite.enabled = false;
        m_laserSound.Stop();
    }

    protected override bool Fire()
    {
        // Collision
        Vector3 start = m_line.GetPosition(0);
        Vector3 end = m_line.GetPosition(1);
        float dist = end.x - start.x;
        string[] masks = {
            "Enemy",
            "EnemyRocket",
            "Egg",
            "Worm"
        };
        LayerMask layerMask = LayerMask.GetMask(masks);
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector3.right, dist, layerMask);
        foreach (RaycastHit2D hit in hits)
        {
            IHitPoints hp = hit.collider.gameObject.GetComponent<IHitPoints>();
            if (null != hp)
            {
                var ret = hp.Damage(s_damage, IHitPoints.HitType.LASER);
                if (ret == IHitPoints.DamageReturn.KILLED)
                {
                    // combo kill
                }
                else
                {
                    Bird bird = hit.collider.GetComponent<Bird>();
                    if (null != bird)
                    {
                        if (m_ownerSprite.flipX)
                        {
                            bird.Push(Vector3.left * s_force);
                        }
                        else
                        {
                            bird.Push(Vector3.right * s_force);
                        }
                    }
                }
                ObjectPool pool = ObjectPool.GetPool(m_hitSplash, 32);
                GameObject obj = pool.Allocate(hit.point);
                if (obj)
                {
                    SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                    sprite.flipX = m_ownerSprite.flipX;
                }
            }
        }
        return true;
    }
}
