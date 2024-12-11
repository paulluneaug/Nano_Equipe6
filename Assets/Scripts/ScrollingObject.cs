using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    [SerializeField] private Vector2 m_scrollDirection;
    [SerializeField] private float m_scrollSpeedFactor;
    [SerializeField] private bool m_useGloablFactor = true;

    private void Update()
    {
        float scrollFator = m_scrollSpeedFactor * Time.deltaTime;

        if (m_useGloablFactor)
        {
            scrollFator *= GlobalVariablesScriptable.Instance.ScrollSpeed;
        }

        transform.Translate(scrollFator * m_scrollDirection.XY0());
    }
}
