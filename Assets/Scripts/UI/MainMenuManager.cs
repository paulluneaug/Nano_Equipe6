using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityUtility.MathU;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SoundOptionRadio m_soundOptionRadio;

    [SerializeField] private Slider m_mainVolumeSlider;
    [SerializeField] private Slider m_sfxVolumeSlider;
    [SerializeField] private Slider m_musicVolumeSlider;

    private void SetMainVolume(float value)
    {
        float unormalizedValue = m_mainVolumeSlider.value.RemapFrom01(-80, 10);

        if(unormalizedValue > -80)
        {
            m_soundOptionRadio.mainVolume = unormalizedValue;
            return;
        }

        m_soundOptionRadio.mainVolume = -80;
    }

    private void SetSFXVolume(float value)
    {
        float unormalizedValue = m_sfxVolumeSlider.value.RemapFrom01(-80, 10);

        if (unormalizedValue > -80)
        {
            m_soundOptionRadio.sfxVolume = unormalizedValue;
            return;
        }

        m_soundOptionRadio.sfxVolume = -80;
    }

    private void SetMusicVolume(float value)
    {
        float unormalizedValue = m_musicVolumeSlider.value.RemapFrom01(-80, 10);

        if (unormalizedValue > -80)
        {
            m_soundOptionRadio.musicVolume = unormalizedValue;
            return;
        }

        m_soundOptionRadio.musicVolume = -80;
    }
}
