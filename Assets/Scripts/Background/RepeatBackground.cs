using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatBackground : Parallax
{
    public Bounds m_bounds;

    GameObject m_prototype;
    List<GameObject> m_pool;

    // Start is called before the first frame update
    protected override void Start()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (null == renderer)
        {
            Debug.LogError("RepeatBackground with no renderer");
            Destroy(this);
        }

        if (m_bounds.extents.x <= 0.0f)
            m_bounds = GetBounds(transform);

        m_prototype = transform.GetChild(0).gameObject;
        m_pool = new List<GameObject>();

        base.Start();
    }

    static Bounds GetBounds(Transform xform)
    {
        Renderer[] allRenderes = xform.GetComponentsInChildren<Renderer>();
        Bounds bound = allRenderes[0].bounds;
        for (int i = 1; i < allRenderes.Length; ++i)
            bound.Encapsulate(allRenderes[i].bounds);
        return bound;
    }

    /// <summary>
    /// "Free" this section of the background back into the pool for reuse
    /// </summary>
    /// <param name="obj"></param>
    void Free(GameObject obj)
    {
        m_pool.Add(obj);
        obj.SetActive(false);
    }

    /// <summary>
    /// "Allocate" a copy of the background object...
    /// If there's one available in the pool, give us that. If not, make a new one.
    /// </summary>
    /// <returns></returns>
    GameObject Allocate()
    {
        if (m_pool.Count > 0)
        {
            GameObject obj = m_pool[m_pool.Count - 1];
            obj.SetActive(true);
            m_pool.RemoveAt(m_pool.Count - 1);
            return obj;
        }

        GameObject newObj = Instantiate(m_prototype);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    // Update is called once per frame
    protected override void LateUpdate()
    {
        // go through deleting any tiles that have scrolled off
        float xMax = float.MinValue;
        float xMin = float.MaxValue;
        Transform firstChild = null;
        Transform lastChild = null;
        for (int childIndex = 0; childIndex < transform.childCount; ++childIndex)
        {
            // find the boundaries of the object in view-space (0.0 to 1.0)
            Transform child = transform.GetChild(childIndex);
            if (child.gameObject.activeInHierarchy)
            {
                if (null == firstChild)
                    firstChild = child;
                lastChild = child;
                Vector3 leftPos = child.position + m_bounds.min;
                Vector3 rightPos = child.position + m_bounds.max;
                Vector3 topLeft = Camera.main.WorldToViewportPoint(leftPos);
                Vector3 botRight = Camera.main.WorldToViewportPoint(rightPos);
                if (botRight.x < 0.0f)
                {   // off the left
                    Free(child.gameObject);
                }
                else if (topLeft.x > 1.0f)
                {   // off the right
                    Free(child.gameObject);
                }
                xMax = Mathf.Max(botRight.x, xMax);
                xMin = Mathf.Min(topLeft.x, xMin);
            }
        }

        // add new tiles to fill to the right
        while (xMax < 1.0f)
        {
            GameObject newObj = Allocate();
            Vector3 pos = lastChild.position;
            pos.x += m_bounds.size.x;
            newObj.transform.position = pos;
            newObj.transform.localScale = firstChild.localScale;
            pos.x += m_bounds.size.x;
            xMax = Camera.main.WorldToViewportPoint(pos).x;
            newObj.transform.SetAsLastSibling();
            lastChild = newObj.transform;
        }
        // add new tiles to fill to the left
        while (xMin > 0.0f)
        {
            GameObject newObj = Allocate();
            Vector3 pos = firstChild.position;
            pos.x -= m_bounds.size.x;
            newObj.transform.position = pos;
            newObj.transform.localScale = firstChild.localScale;
            pos.x -= m_bounds.size.x;
            xMin = Camera.main.WorldToViewportPoint(pos).x;
            newObj.transform.SetAsFirstSibling();
            firstChild = newObj.transform;
        }

        base.LateUpdate();
    }
}
