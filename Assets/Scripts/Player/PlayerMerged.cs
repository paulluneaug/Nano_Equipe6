using DG.Tweening;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Timer;

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



    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);

        m_healthBarManager.Display(false);
        m_healthBarManager.UpdateHealthBar(m_health);

        GameManager.Instance.OnPlayerMerge += OnPlayerMerge;

        m_allIFramesTimers.Add(m_damagesIFrameTimer);
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