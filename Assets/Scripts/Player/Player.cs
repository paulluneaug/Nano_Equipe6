using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;
using UnityUtility.MathU;
using UnityUtility.Pools;
using UnityUtility.Timer;

public class Player : MonoBehaviour
{
    private static readonly int s_animatorParamVelocityX = Animator.StringToHash("DirectionX");
    private static readonly int s_animatorParamVelocityY = Animator.StringToHash("DirectionY");

    public Vector2 Velocity { get => m_velocity; set => m_velocity = value; }
    public bool KnockedDown => m_knockedDown;

    public bool DebugInvinsible = false;

    [Title("Input Actions")]
    [SerializeField] private List<InputActionReference> m_moveActions = new();
    [SerializeField] private List<InputActionReference> m_shootActions = new();

    [Title("Components")]
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private Animator m_animator;
    [SerializeField] protected ShootPattern m_shootPattern;

    [SerializeField] protected SpriteRenderer m_bodySprite;
    [SerializeField] private SpriteRenderer m_wingsSprite;

    [SerializeField] private Transform m_visualElementsRoot;
    [SerializeField] private SpriteRenderer m_bubbleSprite;

    [Title("Movement")]
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_decelerationFactor;
    [SerializeField] private Vector2 m_separationForce;

    [SerializeField] private PlayerType m_playerType;

    [SerializeField] private ContactFilter2D m_contactFilter;
    [SerializeField] private float m_collisionOffset;

    [Title("Health")]
    [SerializeField] private int m_maxHealth;
    [SerializeField] private Timer m_knockedDownTimer;

    [Header("Sound")]
    [SerializeField] private AudioSource m_hitAudioSource;
    [SerializeField] private AudioSource m_dieAudioSource;
    [SerializeField] private SFXControllerPool m_shootSfxPool;

    [Title("I Frames")]
    [SerializeField] private Timer m_mergeIFrameTimer;
    [SerializeField] private Timer m_reviveIFrameTimer;

    [NonSerialized] protected int m_health;
    [NonSerialized] private bool m_knockedDown;

    [NonSerialized] protected List<Timer> m_allIFramesTimers;

    private Vector2 m_velocity = Vector2.zero;
    private bool m_canMove = true;
    protected bool m_canShoot = true;

    [NonSerialized] private List<RaycastHit2D> m_hits;

    // Input State
    private Vector2 m_moveInput;
    private bool m_shootInput;
    private readonly bool m_wantsToMerge;

    // ========== Unity Methods ==========
    // ===================================

    protected virtual void Awake()
    {
        // Yep, that will start the Game Manager three times, but it works fine so whatever
        GameManager.Instance.StartGameManager();
        RegisterPlayer();

        Revive();

        if (m_bubbleSprite != null)
        {
            _ = m_bubbleSprite.DOFade(0.0f, 0.0f);
        }

        m_allIFramesTimers = new List<Timer>()
        {
            m_mergeIFrameTimer,
            m_reviveIFrameTimer,
        };

        m_hits = new List<RaycastHit2D>();
    }

    private void FixedUpdate()
    {
        if (m_canMove)
        {
            Move();
        }
    }

    private void Update()
    {
        UpdateInputState();
        UpdateKnockedDownState();
        if (!m_knockedDown)
        {
            UpdateShoot();
        }
        UpdateAnimation();

        UpdateIFramesTimers(Time.deltaTime);
    }

    protected virtual void UpdateIFramesTimers(float deltaTime)
    {
        _ = m_mergeIFrameTimer.Update(deltaTime);
        _ = m_reviveIFrameTimer.Update(deltaTime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!m_knockedDown)
        {
            return;
        }

        // Both have the Player layer
        if (other.gameObject.layer == gameObject.layer)
        {
            if (!m_knockedDownTimer.IsRunning)
            {
                Revive();
                m_reviveIFrameTimer.Start();

                // Hides the bubble
                if (m_bubbleSprite != null)
                {
                    Color bubbleColor = m_bubbleSprite.color;
                    bubbleColor.a = 1.0f;
                    m_bubbleSprite.color = bubbleColor;

                    _ = m_bubbleSprite.DOFade(1.0f, 0.2f);
                    _ = m_bubbleSprite.DOFade(0.0f, 0.2f);
                }
            }
        }
    }

    private void UpdateKnockedDownState()
    {
        if (!m_knockedDown)
        {
            return;
        }

        if (m_knockedDownTimer.Update(Time.deltaTime))
        {
            // Can be revived
            m_knockedDownTimer.Stop();

            // Displays the bubble
            if (m_bubbleSprite != null && m_visualElementsRoot != null)
            {
                m_visualElementsRoot.gameObject.SetActive(true);

                Color bubbleColor = m_bubbleSprite.color;
                bubbleColor.a = 0.0f;
                m_bubbleSprite.color = bubbleColor;

                _ = m_bubbleSprite.DOFade(1.0f, 0.2f);
            }
        }
    }

    protected virtual void UpdateShoot()
    {
        m_shootPattern.ShouldShoot = m_shootInput && m_canShoot;
        if (m_shootPattern.UpdatePattern(Time.deltaTime))
        {
            if(m_playerType != PlayerType.PlayerMerged)
                PlayShootSound();
        }
    }

    private void PlayShootSound()
    {
        PooledObject<SFXController> sfxController = m_shootSfxPool.Request();

        sfxController.Object.gameObject.SetActive(true);
        sfxController.Object.StartSFXLifeCycle(m_shootSfxPool);
    }

    // ========== Movement ==========
    // ==============================

    private void Move()
    {
        if (m_moveInput.sqrMagnitude > 0 && !m_knockedDown)
        {
            m_velocity += m_acceleration * m_moveInput;

            if (m_velocity.magnitude > (m_maxSpeed * m_moveInput.magnitude))
            {
                m_velocity = m_velocity.normalized * (m_maxSpeed * m_moveInput.magnitude);
            }
        }

        // If we are pressing no key or if we want to go in the opposite direction of our current velocity.
        if (m_moveInput.sqrMagnitude == 0 
            || m_knockedDown
            || MathUf.Sign(m_moveInput.x) != MathUf.Sign(m_velocity.x)
            || MathUf.Sign(m_moveInput.y) != MathUf.Sign(m_velocity.y))
        {
            m_velocity *= m_decelerationFactor;
        }


        Vector2 offset = m_velocity * Time.fixedDeltaTime;

        if (!TryMove(offset))
        {
            if (!TryMove(offset * Vector2.right))
            {
                if (!TryMove(offset * Vector2.up))
                {
                    Debug.LogWarning("Failed to move");
                }
            }
        }
        //m_rigidbody.MovePosition(m_rigidbody.position + m_velocity * Time.fixedDeltaTime);
        //m_rigidbody.linearVelocity = m_velocity;
    }

    private bool TryMove(Vector2 offset)
    {
        int collisionCount = m_rigidbody.Cast(offset, m_contactFilter, m_hits, offset.magnitude + m_collisionOffset);
        if (collisionCount > 0)
        {
            return false;
        }
        m_rigidbody.MovePosition(m_rigidbody.position + offset);
        return true;
    }

    private void UpdateAnimation()
    {
        m_animator.SetFloat(s_animatorParamVelocityX, m_velocity.x);
        m_animator.SetFloat(s_animatorParamVelocityY, m_velocity.y);
        m_animator.SetFloat(s_animatorParamVelocityY, m_velocity.y);
    }

    public void MergeTo(Vector2 position)
    {
        m_canMove = false; // Stop normal movement to avoid conflicting with the tween.

        // Move to the position
        Sequence sequence = DOTween.Sequence();
        _ = sequence.Append(m_rigidbody.DOMove(position, 0.3f).SetEase(Ease.InBack));

        // Disable the objects
        sequence.onComplete += () => gameObject.SetActive(false);
        sequence.onComplete += () => m_canMove = true;

        m_mergeIFrameTimer.Start();
    }

    public void Separate()
    {
        switch (m_playerType)
        {
            case PlayerType.Player1:
                m_velocity -= m_separationForce;
                break;
            case PlayerType.Player2:
                m_velocity += m_separationForce;
                break;

            case PlayerType.PlayerMerged:
            default:
                Debug.Log("Player.Separate : Player has an incorrect player type." +
                          "Make sure the merged player uses the PlayerMerge class.");
                break;
        }
        m_mergeIFrameTimer.Start();
    }


    // ========== Input ==========
    // ===========================

    private void UpdateInputState()
    {
        m_moveInput = GetMoveInput();
        m_shootInput = GetShootInput();
    }

    /**
     * Averages the movement input vector from all controllers.
     * If there is only one action in the list, it just returns the value from that action.
     */
    private Vector2 GetMoveInput()
    {
        Vector2 moveInput = Vector2.zero;

        foreach (InputActionReference actionReference in m_moveActions)
        {
            moveInput += actionReference.action.ReadValue<Vector2>();
        }

        return moveInput / m_moveActions.Count;
    }

    private bool GetShootInput()
    {
        bool actionPerformed = true;

        foreach (InputActionReference actionReference in m_shootActions)
        {
            actionPerformed &= actionReference.action.IsPressed();
        }

        return actionPerformed;
    }

    public ShootPattern GetShootPattern()
    {
        return m_shootPattern;
    }

    public virtual bool TakeDamage(int damage)
    {
        m_hitAudioSource.Play();

        if (DebugInvinsible)
        {
            return false;
        }

        if (m_knockedDown || IsDuringIFrames())
        {
            return false;
        }

        m_health -= damage;
        if (m_health <= 0)
        {
            KnockDown();
            return true;
        }
        return true;
    }

    private void KnockDown()
    {
        GameManager.Instance.AddScore(-10000);
        
        m_dieAudioSource.Play();
        m_knockedDown = true;
        m_knockedDownTimer.Start();

        m_visualElementsRoot.gameObject.SetActive(false);
    }

    protected void Revive()
    {
        m_knockedDown = false;
        m_health = m_maxHealth;
    }

    protected virtual void RegisterPlayer()
    {
        switch (m_playerType)
        {
            case PlayerType.Player1:
                GameManager.Instance.SetPlayer1(this);
                break;

            case PlayerType.Player2:
                GameManager.Instance.SetPlayer2(this);
                break;
            case PlayerType.PlayerMerged:
            default:
                Debug.Log("Player has an incorrect player type." +
                          "Make sure the merged player uses the PlayerMerge class.");
                break;
        }
    }

    private bool IsDuringIFrames()
    {
        return m_allIFramesTimers.Any(timer => timer.IsRunning);
    }
}
