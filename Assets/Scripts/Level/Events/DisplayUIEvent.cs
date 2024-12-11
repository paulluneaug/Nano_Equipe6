using System;
using UnityEngine;
using UnityUtility.Timer;

public class DisplayUIEvent : BaseWaveEvent
{
    private enum State
    {
        FadeIn,
        Display,
        FadeOut,
    }

    [SerializeField] private CanvasGroup m_uiToDisplay;
    [SerializeField] private float m_displayTime;
    [SerializeField] private float m_fadeTime;

    [NonSerialized] private Timer m_displayTimer;
    [NonSerialized] private Timer m_fadeTimer;
    [NonSerialized] private State m_currentState;


    public override void Load()
    {
        base.Load();
        m_displayTimer = new Timer(m_displayTime, false);
        m_fadeTimer = new Timer(m_fadeTime, false);

        m_uiToDisplay.gameObject.SetActive(false);
    }

    public override void StartEvent()
    {
        base.StartEvent();
        StartFadeIn();
    }

    public override void UpdateEvent(float deltaTime, float currentWaveTime)
    {
        base.UpdateEvent(deltaTime, currentWaveTime);
        switch (m_currentState)
        {
            case State.FadeIn:
                UpdateFadeIn(deltaTime);
                break;
            case State.Display:
                UpdateDisplay(deltaTime);
                break;
            case State.FadeOut:
                UpdateFadeOut(deltaTime);
                break;
            default:
                break;
        }
    }

    private void StartFadeIn()
    {
        m_uiToDisplay.alpha = 0.0f;
        m_uiToDisplay.gameObject.SetActive(true);
        m_currentState = State.FadeIn;
        m_fadeTimer.Start();
    }

    private void UpdateFadeIn(float deltaTime)
    {
        if (m_fadeTimer.Update(deltaTime))
        {
            StartDisplay();
            return;
        }
        m_uiToDisplay.alpha = m_fadeTimer.Progress;
    }

    private void StartDisplay()
    {
        m_fadeTimer.Stop();
        m_uiToDisplay.alpha = 1.0f;
        m_currentState = State.Display;
        m_displayTimer.Start();
    }

    private void UpdateDisplay(float deltaTime)
    {
        if (m_displayTimer.Update(deltaTime))
        {
            StartFadeOut();
            return;
        }
    }

    private void StartFadeOut()
    {
        m_currentState = State.FadeOut;
        m_fadeTimer.Start();
    }

    private void UpdateFadeOut(float deltaTime)
    {
        if (m_fadeTimer.Update(deltaTime))
        {
            Finish();
            return;
        }
        m_uiToDisplay.alpha = 1.0f - m_fadeTimer.Progress;
    }

    protected override void OnFinish()
    {
        base.OnFinish();
        m_fadeTimer.Stop();
        m_uiToDisplay.alpha = 0.0f;
        m_uiToDisplay.gameObject.SetActive(false);
    }
}
