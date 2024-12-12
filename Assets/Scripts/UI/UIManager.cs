using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image m_fadeToBlackScreen;
    [SerializeField] private CanvasGroup m_mainMenuContainer;
    [SerializeField] private CanvasGroup m_settingsContainer;
    [SerializeField] private CanvasGroup m_creditContainer;

    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_quitButton;

    private void Start()
    {
        m_mainMenuContainer.gameObject.SetActive(true);
        m_creditContainer.gameObject.SetActive(false);

        m_playButton.onClick.AddListener(OnPlayButtonPressed);
        m_quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    private void OnPlayButtonPressed()
    {
        Sequence seq = DOTween.Sequence();

        m_fadeToBlackScreen.gameObject.SetActive(true);

        _ = seq.Append(m_fadeToBlackScreen.DOFade(1, 2).From(0));
        _ = seq.onComplete += () => SceneManager.LoadScene(1);

        _ = seq.Play();
    }

    private void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
