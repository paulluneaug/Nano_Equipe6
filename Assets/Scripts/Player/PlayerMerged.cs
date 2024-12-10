using DG.Tweening;
using UnityEngine;

public class PlayerMerged : Player
{
    [SerializeField] private Animator m_fusionAnimator;
    
    [SerializeField] private SpriteRenderer m_fusionSprite;
    [SerializeField] private SpriteRenderer m_bodySprite;
    [SerializeField] private SpriteRenderer m_bigWingsSprite;
    [SerializeField] private SpriteRenderer m_mediumWingsSprite;
    [SerializeField] private SpriteRenderer m_smallWingsSprite;

    private Sequence m_fusionSequence;
    
    private void Awake()
    {
        GameManager.Instance.SetPlayerMerged(this);
        gameObject.SetActive(false);
        GameManager.Instance.OnPlayerMerge += TriggerFusionVFX;
    }
    
    private void TriggerFusionVFX(bool isMerged)
    {
        if (isMerged)
        {
            m_fusionAnimator.Play("Fusion");
            
            m_fusionSequence = DOTween.Sequence().Pause();
            
            m_fusionSequence.Append(m_fusionSprite.DOFade(1, 0.1f).From(0));
            m_fusionSequence.Append(m_bodySprite.DOFade(1, 0.5f).From(0));
            m_fusionSequence.Append(m_bigWingsSprite.DOFade(1, 0.15f).From(0));
            m_fusionSequence.Append(m_smallWingsSprite.DOFade(1, 0.15f).From(0));
            m_fusionSequence.Append(m_mediumWingsSprite.DOFade(1, 0.15f).From(0));
            m_fusionSequence.Insert(1.0f, m_fusionSprite.DOFade(0, 0.5f));
            
            m_fusionSequence.Play();
        }
    }
}