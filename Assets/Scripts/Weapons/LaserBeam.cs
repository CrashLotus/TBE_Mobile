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
    float m_offset = 0.0f;

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
        m_hitSplash.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Collision
        Vector3 rightSide = new Vector3(1.0f, 0.5f, 0.0f);
        rightSide = Camera.main.ViewportToWorldPoint(rightSide);
        Vector3 start = transform.position;
        float dist = rightSide.x - start.x;
        string[] masks = {
            "Enemy",
            "EnemyRocket",
            "Egg"
        };
        LayerMask layerMask = LayerMask.GetMask(masks);
        RaycastHit2D hit = Physics2D.Raycast(start, Vector3.right, dist, layerMask);
        if (hit)
        {
            m_line.SetPosition(1, hit.point);
            m_hitSplash.SetActive(true);
            m_hitSplash.transform.position = hit.point;
        }
        else
        {
            Vector3 end = start;
            end.x = rightSide.x;
            m_line.SetPosition(1, end);
            m_hitSplash.SetActive(false);
        }

        // Animation
        float dt = Time.deltaTime;
        m_animTimer += m_animSpeed * dt;
        m_offset += m_scrollSpeed * dt;
        m_offset = m_offset - (int)m_offset;
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
        m_mat.SetTextureOffset("_MainTex", new Vector2(m_offset, 0.0f));
    }
}
