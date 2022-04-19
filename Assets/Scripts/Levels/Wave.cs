using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : ScriptableObject
{
    public string m_label;
    public string m_text;

    enum State
    {
        CENTER,
        SHRINK,
        TOP
    }

    protected bool m_isActive;
    protected bool m_isDone;
    State m_state;
    float m_timer;

    const float s_centerTime = 2.2f;
    const float s_centerScale = 1.0f;
    const float s_centerPosY = 264.0f;
    const float s_shrinkTime = 1.0f;
    const float s_shrinkScale = 0.6f;
    const float s_shrinkPosY = 64.0f;

    public Wave()
    {
        m_isActive = false;
        m_isDone = false;
        m_text = null;
        m_state = State.CENTER;
        m_timer = s_centerTime;
    }

    public void SetText(string text)
    {
        m_text = text;
        m_state = State.CENTER;
        m_timer = s_centerTime;
    }

    public virtual void Start()
    {
        m_isActive = true;
        m_isDone = false;
        m_state = State.CENTER;
        m_timer = s_centerTime;
    }

    public virtual void Stop()
    {
        m_isActive = false;
        m_isDone = true;
    }

    public virtual void Update()
    {
        float dt = Time.deltaTime;
        m_timer -= dt;
        if (m_timer < 0.0f)
        {
            switch (m_state)
            {
                case State.CENTER:
                    m_state = State.SHRINK;
                    m_timer += s_shrinkTime;
                    break;
                case State.SHRINK:
                    m_state = State.TOP;
                    break;
            }
        }
    }

    public virtual void Draw()
    {
#if false   //mrwTODO
        if (null != m_text)
        {
#if true
            float scale = 1.0f;
            float posY = 0.0f;
            switch (m_state)
            {
                case State.CENTER:
                    scale = s_centerScale;
                    posY = s_centerPosY;
                    break;
                case State.SHRINK:
                    {
                        float lerp = m_timer / s_shrinkTime;
                        float lerp2 = lerp * lerp;
                        float lerp3 = lerp * lerp2;
                        lerp = -2.0f * lerp3 + 3.0f * lerp2;
                        scale = MathHelper.Lerp(s_shrinkScale, s_centerScale, lerp);
                        posY = MathHelper.Lerp(s_shrinkPosY, s_centerPosY, lerp);
                        break;
                    }
                case State.TOP:
                    scale = s_shrinkScale;
                    posY = s_shrinkPosY;
                    break;
            }
            //            Rectangle screenRect = Game1.GetScreenRect();
            //            Vector2 origin = 0.5f * m_font.MeasureString(m_text);
            //            Vector2 pos = new Vector2(screenRect.Center.X, posY);
            Game1.DrawTextCenterAtY(m_text, posY, scale);
            //            sb.DrawString(m_font, m_text, pos, Color.White, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
#else
            Game1.DrawTextCenter(m_text);
#endif
        }
#endif
    }

    public bool IsActive()
    {
        return m_isActive;
    }

    public virtual bool IsDone()
    {
        return m_isDone;
    }

    public virtual bool IsWait()
    {
        return false;
    }

    public string GetLabel()
    {
        return m_label;
    }

    public virtual void Reset()
    {
        m_isActive = false;
        m_isDone = false;
    }
}
