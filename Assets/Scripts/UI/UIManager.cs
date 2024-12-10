using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup m_mainMenuContainer;
    [SerializeField] private CanvasGroup m_settingsContainer;
    [SerializeField] private CanvasGroup m_creditContainer;

    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_quitButton;

    private void Start()
    {
        m_mainMenuContainer.gameObject.SetActive(true);
        m_settingsContainer.gameObject.SetActive(false);
        m_creditContainer.gameObject.SetActive(false);

        m_playButton.onClick.AddListener(OnPlayButtonPressed);
        m_quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    private void OnPlayButtonPressed()
    {
        Debug.Log("Starting game");
        //SceneManager.LoadScene(1);
    }

    private void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
