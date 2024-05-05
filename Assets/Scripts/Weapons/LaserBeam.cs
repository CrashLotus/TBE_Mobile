using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public Texture[] m_textures;
    public float m_animSpeed = 12.0f;   // frames per second
    public float m_scrollSpeed = -1.0f;
    LineRenderer m_line;
    Material m_mat;
    float m_animTimer;
    int m_curFrame;
    float m_uvOffset = 0.0f;
    protected SpriteRenderer m_ownerSprite;
    protected Vector3 m_localPosition;

    // Start is called before the first frame update
    void Start()
    {
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
        m_line.SetPosition(0, start);
        float dist = rightSide.x - start.x;
        string[] masks = {
            "Enemy",
            "EnemyRocket",
            "Egg",
            "Worm"
        };
        LayerMask layerMask = LayerMask.GetMask(masks);
        RaycastHit2D hit = Physics2D.Raycast(start, Vector3.right, dist, layerMask);
        if (hit)
        {
            m_line.SetPosition(1, hit.point);
        }
        else
        {
            Vector3 end = start;
            end.x = rightSide.x;
            m_line.SetPosition(1, end);
        }

        // Animation
        float dt = Time.deltaTime;
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
