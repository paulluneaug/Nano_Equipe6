using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    public static Action animCallBack;
    private Animator m_objectAnimator;
    private AudioSource m_sfxAudioSource;
    [SerializeField] private AudioClip m_soundToPlayOnClick;
    [SerializeField] private CanvasGroup m_containerToDeactivate;
    [SerializeField] private CanvasGroup m_containerToActivate;

    public static bool canClickAgain;

    private void Awake()
    {
        m_objectAnimator = GetComponent<Animator>();
        m_sfxAudioSource = GameObject.Find("UIAudioSource").GetComponent<AudioSource>(); //(huez moi)
        GetComponent<Button>().onClick.AddListener(PlayButtonSequence);
    }

    public void InitButton()
    {
    }

    private void PlayButtonSequence()
    {
        if (m_soundToPlayOnClick != null)
        {
            PlaySoundOneShot();
        }

        if(m_containerToDeactivate != null)
        {
            m_containerToDeactivate.interactable = false;

        }
        
        m_objectAnimator.Play("MainUIButtonOnClick");
    }

    public void AnimationCallback() //Call in Animation Event (huez moi BIS)
    {
        if (m_containerToActivate != null)
        {
            m_containerToActivate.interactable = true;

        }

        m_containerToDeactivate.gameObject.SetActive(false);
        m_containerToActivate.alpha = 0;
        m_containerToActivate.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        _ = seq.Append(m_containerToDeactivate.DOFade(0, 0.2f).From(1));
        _ = seq.Append(m_containerToActivate.DOFade(1, 0.5f).From(0));

        _ = seq.Play();
    }

    //public void AnimationCallback(Action cb)
    //{
    //    animCallBack += cb;

    //    if (m_containerToDeactivate.Count > 0)
    //    {
    //        for (int i = 0; i < m_containerToDeactivate.Count; i++)
    //        {
    //            m_containerToDeactivate[i].interactable = false;
    //        }
    //    }

    //    if (m_soundToPlayOnClick != null)
    //    {
    //        PlaySoundOneShot();
    //    }
    //}

    //public void AnimationCallbackLauncher() //Call in AnimationEvent
    //{
    //    m_objectAnimator.SetTrigger("clickAnimDone");
    //    m_objectAnimator.Rebind();
    //    animCallBack?.Invoke();

    //    Debug.LogWarning("InCallBackLauncher");

    //    if (m_containerToDeactivate.Count > 0)
    //    {
    //        for (int i = 0; i < m_containerToDeactivate.Count; i++)
    //        {
    //            m_containerToDeactivate[i].interactable = true;
    //        }
    //    }
    //    animCallBack = null;
    //}

    //public void CrossFadeAnim(string animToPerform, float timeOfCrossFade = 0.2f)
    //{
    //    m_objectAnimator.CrossFade(animToPerform, timeOfCrossFade);
    //}

    //public void PlayAnim(string animToPerform)
    //{
    //    m_objectAnimator.Play(animToPerform);
    //}

    public void PlaySoundOneShot()
    {
        m_sfxAudioSource.PlayOneShot(m_soundToPlayOnClick);
    }
}
