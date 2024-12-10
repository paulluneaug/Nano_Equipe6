using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;
using UnityUtility.Timer;

public class Player : MonoBehaviour
{
    private static readonly int s_animatorParamVelocityX = Animator.StringToHash("DirectionX");
    private static readonly int s_animatorParamVelocityY = Animator.StringToHash("DirectionY");

    public Vector2 Velocity { get => m_velocity; set => m_velocity = value; }
    public bool KnockedDown => m_knockedDown;

    [Header("Multiplayer")]
    [SerializeField] private int m_playerNumber;

    [Header("Input Actions")]
    [SerializeField] private List<InputActionReference> m_moveActions = new();
    [SerializeField] private List<InputActionReference> m_shootActions = new();

    [Header("Components")]
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private Animator m_animator;
    [SerializeField] private ShootPattern m_shootPattern;

    [Header("Movement")]
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_decelerationFactor;

    [SerializeField] private PlayerType m_playerType;

    [Title("Health")]
    [SerializeField] private int m_maxHealth;
    [SerializeField] private Timer m_knockedDownTimer;

    [NonSerialized] private int m_health;
    [NonSerialized] private bool m_knockedDown;

    private Vector2 m_velocity = Vector2.zero;
    private bool m_isMerging;

    // Input State
    private Vector2 m_moveInput;
    private bool m_shootInput;
    private readonly bool m_wantsToMerge;

    // ========== Unity Methods ==========
    // ===================================

    private void Awake()
    {
        // Yep, that will start the Game Manager three times, but it works fine so whatever
        GameManager.Instance.StartGameManager();

        switch (m_playerType)
        {
            case PlayerType.Player1:
                GameManager.Instance.SetPlayer1(this);
                break;

            case PlayerType.Player2:
                GameManager.Instance.SetPlayer2(this);
                break;
            default:
                Debug.Log("Player has an incorrect player type." +
                          "Make sure the merged player uses the PlayerMerge class.");
                break;
        }

        Revive();
    }

    private void FixedUpdate()
    {
        if (!m_knockedDown && !m_isMerging)
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
        }
    }

    private void UpdateShoot()
    {
        m_shootPattern.ShouldShoot = m_shootInput;
        m_shootPattern.UpdatePattern(Time.deltaTime);
    }

    // ========== Movement ==========
    // ==============================

    private void Move()
    {
        if (m_moveInput.sqrMagnitude > 0)
        {
            m_velocity += m_acceleration * m_moveInput;

            if (m_velocity.magnitude > (m_maxSpeed * m_moveInput.magnitude))
            {
                m_velocity = m_velocity.normalized * (m_maxSpeed * m_moveInput.magnitude);
            }
        }

        // If we are pressing no key or if we want to go in the opposite direction of our current velocity.
        if (m_moveInput.sqrMagnitude == 0
           || (m_moveInput.x < 0 && m_velocity.x > 0)
           || (m_moveInput.x > 0 && m_velocity.x < 0)
           || (m_moveInput.y < 0 && m_velocity.y > 0)
           || (m_moveInput.y > 0 && m_velocity.y < 0))
        {
            m_velocity *= m_decelerationFactor;
        }

        m_rigidbody.MovePosition(m_rigidbody.position + m_velocity * Time.fixedDeltaTime);
    }

    private void UpdateAnimation()
    {
        m_animator.SetFloat(s_animatorParamVelocityX, m_velocity.x);
        m_animator.SetFloat(s_animatorParamVelocityY, m_velocity.y);
        m_animator.SetFloat(s_animatorParamVelocityY, m_velocity.y);
    }

    public void MergeToPosition(Vector2 position)
    {
        m_isMerging = true;
        Sequence sequence = DOTween.Sequence();
        _ = sequence.Append(m_rigidbody.DOMove(position, 0.3f).SetEase(Ease.InCubic));
        sequence.onComplete += () => gameObject.SetActive(false);
        sequence.onComplete += () => m_isMerging = false;
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


    public virtual void TakeDamage(int damage)
    {
        m_health -= damage;
        if (m_health <= 0)
        {
            Kill();
        }
    }

    private void Kill()
    {
        m_knockedDown = true;
        m_knockedDownTimer.Start();
    }

    protected void Revive()
    {
        m_knockedDown = false;
        m_health = m_maxHealth;
    }
}
