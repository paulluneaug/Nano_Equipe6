using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityUtility.MathU;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup m_audioMixerGroup;
    [SerializeField] private string m_exposedVolumeParameter;
    [SerializeField] private Slider m_slider;

    private void Start()
    {
        m_slider.onValueChanged.RemoveAllListeners();

        m_slider.onValueChanged.AddListener(ChangeVolume);
    }

    private void ChangeVolume(float volume)
    {
        float unormalizedValue = volume.RemapFrom01(-80, 10);

        if(volume > 0)
        {
            float finalVolume = Mathf.Log10(volume) * 20;
            _ = m_audioMixerGroup.audioMixer.SetFloat(m_exposedVolumeParameter, finalVolume);
            return;
        }

        _ = m_audioMixerGroup.audioMixer.SetFloat(m_exposedVolumeParameter, -80);

    }
}
