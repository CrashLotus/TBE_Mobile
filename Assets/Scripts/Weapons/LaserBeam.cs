using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public Texture[] m_textures;
    public float m_animSpeed = 12.0f;   // frames per second
    public float m_scrollSpeed = -1.0f;
    public GameObject m_hitSplash;
    LineRenderer m_line;
    Material m_mat;
    float m_animTimer;
    int m_curFrame;
    float m_uvOffset = 0.0f;
    protected SpriteRenderer m_ownerSprite;
    protected Vector3 m_localPosition;

    const float s_damage = 0.1f;
    public float s_force = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        ObjectPool.GetPool(m_hitSplash, 16);
        m_line = GetComponent<LineRenderer>();
        if (null == m_line)
        {
            Debug.LogError("Laser needs a LineRenderer");
            Destroy(gameObject);
            return;
        }
        m_mat = new Material(m_line.material);
        m_line.material = m_mat;
        m_animTimer = 0.0f;
        m_curFrame = -1;

        m_ownerSprite = transform.parent.GetComponent<SpriteRenderer>();
        m_localPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = BulletTime.Get().GetDeltaTime(true);

        // flip with owner
        Vector3 pos = m_localPosition;
        if (m_ownerSprite.flipX)
        {
            pos.x = -pos.x;
        }
        
        transform.localPosition = pos;

        // Collision
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
        float dist = rightSide.x - start.x;
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
                var ret = hp.Damage(60.0f * s_damage * dt, IHitPoints.HitType.LASER);
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
                            bird.Push(Vector3.left * s_force);
                        }
                    }
                }
                ObjectPool pool = ObjectPool.GetPool(m_hitSplash, 16);
                GameObject obj = pool.Allocate(hit.point);
                if (obj)
                {
                    SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                    sprite.flipX = m_ownerSprite.flipX;
                }
            }
        }

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
}
