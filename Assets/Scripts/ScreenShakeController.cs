using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityUtility.Timer;

public class ScreenShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_shakingCamera;
    [SerializeField] private CinemachineCamera m_normalCamera;

    [NonSerialized] private Timer m_shakeTimer;


    private void Awake()
    {
        m_shakeTimer ??= new Timer(0.0f, false);
    }

    private void Update()
    {
        if (m_shakeTimer.Update(Time.deltaTime))
        {
            SetShakeState(false);
        }
    }

    public void Shake(float duration)
    {
        if (m_shakeTimer.IsRunning)
        {
            m_shakeTimer.Stop();
            SetShakeState(false);
        }

        m_shakeTimer.Duration = duration;
        m_shakeTimer.Start();
        SetShakeState(true);
    }

    private void SetShakeState(bool state)
    {
        m_shakingCamera.gameObject.SetActive(state);
        m_normalCamera.gameObject.SetActive(!state);
    }
}
