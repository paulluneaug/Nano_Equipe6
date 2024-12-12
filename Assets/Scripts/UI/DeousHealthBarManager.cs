using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeousHealthBarManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image[] m_healthImages;
    [SerializeField] private float m_fadeTime;

    public void Display(bool display)
    {
        float targetAlpha = display ? 1f : 0f;
        _ = m_canvasGroup.DOFade(targetAlpha, m_fadeTime);
    }

    public void UpdateHealthBar(int health)
    {
        for (int i = 0; i < m_healthImages.Length; i++)
        {
            float targetAlpha = i < health ? 1.0f : 0.0f;
            m_healthImages[i].CrossFadeAlpha(targetAlpha, m_fadeTime, false);
        }
    }
}
