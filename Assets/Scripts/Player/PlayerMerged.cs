using DG.Tweening;
using UnityEngine;

public class PlayerMerged : Player
{
    [SerializeField] private Animator m_fusionAnimator;
    
    [SerializeField] private SpriteRenderer m_fusionSprite;
    [SerializeField] private SpriteRenderer m_bigWingsSprite;
    [SerializeField] private SpriteRenderer m_mediumWingsSprite;
    [SerializeField] private SpriteRenderer m_smallWingsSprite;

    private void Awake()
    {
        GameManager.Instance.SetPlayerMerged(this);
        gameObject.SetActive(false);
        GameManager.Instance.OnPlayerMerge += TriggerFusionVFX;

        Revive();
    }

    private void OnDestroy()
    {
        if (!GameManager.ApplicationIsQuitting)
        {
            GameManager.Instance.OnPlayerMerge -= TriggerFusionVFX;
        }
    }

    private void TriggerFusionVFX(bool isMerged)
    {
        if (isMerged) // Begin fusion animation
        {
            m_canShoot = false;
            m_fusionAnimator.Play("Fusion");

            var fusionSequence = DOTween.Sequence();

            // Fade in fusion animation and body
            fusionSequence.Append(m_fusionSprite.DOFade(1, 0.1f).From(0));
            fusionSequence.Append(m_bodySprite.DOFade(1, 1f).From(0));
            
            // Fade in wings
            fusionSequence.Append(m_bigWingsSprite.DOFade(1, 0.2f).From(0));
            fusionSequence.Append(m_smallWingsSprite.DOFade(1, 0.2f).From(0));
            fusionSequence.Append(m_mediumWingsSprite.DOFade(1, 0.2f).From(0));
            
            // Fade out fusion animation
            fusionSequence.Insert(1.0f, m_fusionSprite.DOFade(0, 0.25f));
            fusionSequence.onComplete += () => m_canShoot = true;
            
            fusionSequence.Play();
        }
        else // Begin separation animation
        {
            m_canShoot = false;
            m_fusionAnimator.Play("Separation");

            var separationSequence = DOTween.Sequence();
            
            // Fade in fusion animation
            separationSequence.Append(m_fusionSprite.DOFade(1, 0.1f).From(0));
            separationSequence.AppendInterval(0.2f);
            
            // Fade out wings
            separationSequence.Append(m_bigWingsSprite.DOFade(0, 0.2f));
            separationSequence.Append(m_smallWingsSprite.DOFade(0, 0.2f));
            separationSequence.Append(m_mediumWingsSprite.DOFade(0, 0.2f));
            
            // Fade out body and fusion animation
            separationSequence.Append(m_bodySprite.DOFade(0, 0.25f));
            separationSequence.Insert(1.0f, m_fusionSprite.DOFade(0, 0.25f));

            separationSequence.onComplete += () => gameObject.SetActive(false);
            separationSequence.onComplete += () => m_canShoot = true;
        }
    }
}