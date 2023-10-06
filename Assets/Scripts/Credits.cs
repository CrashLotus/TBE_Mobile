using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Credits : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
{
    public TextMeshProUGUI m_textProto;
    public float m_spacing = 120.0f;
    public float m_scrollRate = 100.0f;
    public float m_circleRate = 1.0f;

    float m_circleTime = 0.0f;

    public class CreditList
    {
        public class Entry
        {
            public string m_text;
            public float m_size;
            public float m_delay;
        }

        public List<Entry> m_theList;
    }
    CreditList m_creditsRoll;
    Transform m_root;
    Vector3 m_rootPos;
    TextMeshProUGUI m_lastElement;
    bool m_isFingerDown = false;

    // Start is called before the first frame update
    void Start()
    {
        MusicManager.Get().Play(MusicManager.SongType.CREDITS);
        TextAsset asset = Resources.Load<TextAsset>("Credits");
        XmlSerializer xmls = new XmlSerializer(typeof(CreditList));
        StringReader textRead = new StringReader(asset.text);
        m_creditsRoll = xmls.Deserialize(textRead) as CreditList;
        textRead.Close();
        Resources.UnloadAsset(asset);
        m_root = m_textProto.transform.parent;
        m_rootPos = m_root.transform.localPosition;
        Vector3 pos = m_textProto.rectTransform.anchoredPosition3D;
        float lastPos = pos.y;
        foreach (CreditList.Entry entry in m_creditsRoll.m_theList)
        {
            pos.y -= entry.m_delay * m_spacing;
            GameObject obj = Instantiate(m_textProto.gameObject, m_root);
            TextMeshProUGUI textMesh = obj.GetComponent<TextMeshProUGUI>();
            textMesh.text = entry.m_text;
            textMesh.rectTransform.anchoredPosition3D = pos;
            textMesh.rectTransform.localScale = entry.m_size * Vector3.one;
            if (pos.y < lastPos)
            {
                lastPos = pos.y;
                m_lastElement = textMesh;
            }
        }
        m_textProto.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (false == m_isFingerDown)
        {
            m_rootPos.y += m_scrollRate * Time.deltaTime;
            m_root.transform.localPosition = m_rootPos;
            Vector3 lastPos = Camera.main.ScreenToViewportPoint(m_lastElement.transform.position);
            if (lastPos.y > 1.05f)
                GameManager.Get().ReturnToMainMenu();
        }

        m_circleTime += m_circleRate * Time.unscaledDeltaTime;
        Shader.SetGlobalFloat("_CreditWipe", m_circleTime);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isFingerDown = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (m_isFingerDown)
        {
            m_rootPos.y += 1280.0f * eventData.delta.y / Screen.height;
            if (m_rootPos.y < 0.0f)
                m_rootPos.y = 0.0f;
            m_root.transform.localPosition = m_rootPos;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isFingerDown = false;
    }
}
