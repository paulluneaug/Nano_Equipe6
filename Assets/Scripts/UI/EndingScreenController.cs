using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndingScreenController : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_endingScreenGroup;
    [SerializeField] private Image m_victoryScreen;
    [SerializeField] private Image m_defeatScreen;

    private void Start()
    {
        GameManager.Instance.RegisterEndingScreenController(this);
    }

    public void SetScreenActive(bool active)
    {
        if (active)
        {
            m_endingScreenGroup.alpha = 0.0f;
            m_endingScreenGroup.gameObject.SetActive(true);
            _ = m_endingScreenGroup.DOFade(1.0f, 0.2f);
        }
        else
        {
            m_endingScreenGroup.gameObject.SetActive(false);
            m_endingScreenGroup.alpha = 0.0f;
        }
    }

    public void SetEndingScreen(bool victory)
    {
        m_victoryScreen.gameObject.SetActive(victory);
        m_defeatScreen.gameObject.SetActive(!victory);
    }
}
