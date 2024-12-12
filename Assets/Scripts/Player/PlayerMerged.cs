using System;
using DG.Tweening;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Timer;
using UnityUtility.Utils;

public class PlayerMerged : Player
{
    [SerializeField] private Timer m_damagesIFrameTimer;

    [Title("Animation")]
    [SerializeField] private Animator m_fusionAnimator;

    [SerializeField] private SpriteRenderer m_fusionSprite;
    [SerializeField] private SpriteRenderer m_bigWingsSprite;
    [SerializeField] private SpriteRenderer m_mediumWingsSprite;
    [SerializeField] private SpriteRenderer m_smallWingsSprite;

    [Title("UI")]
    [SerializeField] private DeousHealthBarManager m_healthBarManager;

    [Title("Unstuck")]
    [SerializeField] private Transform m_unstuckTarget;
    [SerializeField] private float m_unstuckStep;

    [Title("Sound")]
    [SerializeField] private AudioSource m_laserStartAudioSource;
    [SerializeField] private AudioSource m_laserLoopAudioSource;
    [SerializeField] private AudioSource m_laserEndAudioSource;
    
    private LaserShootPattern m_laserShootPattern;
    private bool m_isShooting;
    private bool m_wasShootingLastFrame;
    
    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);

        m_healthBarManager.Display(false);
        m_healthBarManager.UpdateHealthBar(m_health);

        GameManager.Instance.OnPlayerMerge += OnPlayerMerge;

        m_allIFramesTimers.Add(m_damagesIFrameTimer);

        m_laserShootPattern = (LaserShootPattern)m_shootPattern;
    }

    protected override void RegisterPlayer()
    {
        GameManager.Instance.SetPlayerMerged(this);
    }

    public override bool TakeDamage(int damage)
    {
        if (base.TakeDamage(damage))
        {
            m_damagesIFrameTimer.Start();

            m_healthBarManager.UpdateHealthBar(m_health);

            return true;
        }
        return false;
    }

    
    
    protected override void UpdateIFramesTimers(float deltaTime)
    {
        base.UpdateIFramesTimers(deltaTime);
        _ = m_damagesIFrameTimer.Update(deltaTime);
    }

    protected override void UpdateShoot()
    {
        base.UpdateShoot();

        m_wasShootingLastFrame = m_isShooting;

        if (m_laserShootPattern.GetShootStep() == LaserShootPattern.LaserShootStep.LaserOn)
            m_isShooting = true;
        else
            m_isShooting = false;

        if (!m_wasShootingLastFrame && m_isShooting)
            PlayLaserStartAndLoopSound();
        else if (m_wasShootingLastFrame && !m_isShooting)
            PlayLaserEndSound();
    }

    private void PlayLaserStartAndLoopSound()
    {
        m_laserStartAudioSource.Play();
        m_laserLoopAudioSource.Play();
    }

    private void PlayLaserEndSound()
    {
        m_laserEndAudioSource.Play();
        m_laserLoopAudioSource.Stop();
    }

    private void OnDestroy()
    {
        if (!GameManager.ApplicationIsQuitting)
        {
            GameManager.Instance.OnPlayerMerge -= OnPlayerMerge;
        }
    }

    private void OnPlayerMerge(bool isMerged)
    {
        TriggerFusionVFX(isMerged);
        if (isMerged)
        {
            Revive();
        }
        m_healthBarManager.Display(isMerged);
        m_healthBarManager.UpdateHealthBar(m_health);
        UnstuckIfNeeded();
    }

    private void UnstuckIfNeeded()
    {
        if (TryMove(Vector2.zero))
        {
            // Not stuck
            return;
        }

        Vector2 unstuckDirection = (m_unstuckTarget.position - transform.position).XY().normalized;

        int stepCount = 1;
        Vector2 offset;
        do
        {
            offset = m_unstuckStep * stepCount * unstuckDirection;
            ++stepCount;
        }
        while (!TryMove(Vector2.zero, offset) && stepCount < 1000);
    }

    private void TriggerFusionVFX(bool isMerged)
    {
        if (isMerged) // Begin fusion animation
        {
            m_canShoot = false;
            m_fusionAnimator.Play("Fusion");

            var fusionSequence = DOTween.Sequence();

            // Fade in fusion animation and body
            _ = fusionSequence.Append(m_fusionSprite.DOFade(1, 0.1f).From(0));
            _ = fusionSequence.Append(m_bodySprite.DOFade(1, 1f).From(0));

            // Fade in wings
            _ = fusionSequence.Append(m_bigWingsSprite.DOFade(1, 0.2f).From(0));
            _ = fusionSequence.Append(m_smallWingsSprite.DOFade(1, 0.2f).From(0));
            _ = fusionSequence.Append(m_mediumWingsSprite.DOFade(1, 0.2f).From(0));

            // Fade out fusion animation
            _ = fusionSequence.Insert(1.0f, m_fusionSprite.DOFade(0, 0.25f));
            fusionSequence.onComplete += () => m_canShoot = true;

            _ = fusionSequence.Play();
        }
        else // Begin separation animation
        {
            m_canShoot = false;
            m_fusionAnimator.Play("Separation");

            var separationSequence = DOTween.Sequence();

            // Fade in fusion animation
            _ = separationSequence.Append(m_fusionSprite.DOFade(1, 0.1f).From(0));
            _ = separationSequence.AppendInterval(0.2f);

            // Fade out wings
            _ = separationSequence.Append(m_bigWingsSprite.DOFade(0, 0.2f));
            _ = separationSequence.Append(m_smallWingsSprite.DOFade(0, 0.2f));
            _ = separationSequence.Append(m_mediumWingsSprite.DOFade(0, 0.2f));

            // Fade out body and fusion animation
            _ = separationSequence.Append(m_bodySprite.DOFade(0, 0.25f));
            _ = separationSequence.Insert(1.0f, m_fusionSprite.DOFade(0, 0.25f));

            separationSequence.onComplete += () => gameObject.SetActive(false);
            separationSequence.onComplete += () => m_canShoot = true;
        }
    }
}