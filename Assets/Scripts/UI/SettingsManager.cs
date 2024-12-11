using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private float m_fadingTime = 0.5f;

    [SerializeField] private CanvasGroup m_controlContainer;
    [SerializeField] private CanvasGroup m_volumeOptionContainer;

    [SerializeField] private Button m_controlsButton;
    [SerializeField] private Button m_volumeOptionButton;

    private void Start()
    {
        m_controlsButton.onClick.RemoveAllListeners();
        m_volumeOptionButton.onClick.RemoveAllListeners();

        m_controlContainer.gameObject.SetActive(false);
        m_volumeOptionContainer.gameObject.SetActive(false);

        m_controlsButton.onClick.AddListener(OnControlButtonPressed);
        m_volumeOptionButton.onClick.AddListener(OnVolumeOptionButtonPressed);
    }

    private void OnControlButtonPressed()
    {
        m_controlContainer.gameObject.SetActive(true);
        _ = m_controlContainer.DOFade(1, m_fadingTime).From(0);
    }

    private void OnVolumeOptionButtonPressed()
    {
        m_volumeOptionContainer.gameObject.SetActive(true);
        _ = m_volumeOptionContainer.DOFade(1, m_fadingTime).From(0);
    }
}
