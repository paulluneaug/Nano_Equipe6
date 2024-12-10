using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float m_fadingTime;

    [SerializeField] private CanvasGroup m_mainMenuContainer;
    [SerializeField] private CanvasGroup m_settingsContainer;
    [SerializeField] private CanvasGroup m_creditContainer;

    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_settingsButton;
    [SerializeField] private Button m_creditButton;
    [SerializeField] private Button m_quitButton;

    private void Start()
    {
        m_mainMenuContainer.gameObject.SetActive(true);
        m_settingsContainer.gameObject.SetActive(false);
        m_creditContainer.gameObject.SetActive(false);
    }

    private void OnPlayButtonPressed()
    {
        SceneManager.LoadScene(1);
    }

    private void OnSettingsButtonPressed()
    {
        _ = m_mainMenuContainer.DOFade(0, m_fadingTime);
        _= m_settingsContainer.DOFade(1, m_fadingTime);
    }

    private void OnCreditButtonPressed()
    {
        _ = m_mainMenuContainer.DOFade(0, m_fadingTime);
        _ = m_creditContainer.DOFade(1, m_fadingTime);
    }
}
