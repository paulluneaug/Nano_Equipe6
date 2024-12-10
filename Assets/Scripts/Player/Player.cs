using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private static readonly int s_animatorParamVelocityX = Animator.StringToHash("DirectionX");
    private static readonly int s_animatorParamVelocityY = Animator.StringToHash("DirectionY");

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
    
    private Vector2 m_velocity = Vector2.zero;
    private bool m_isMerging;
    
    // Input State
    private Vector2 m_moveInput;
    private bool m_shootInput;
    private bool m_wantsToMerge;
    
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
    }
    
    private void FixedUpdate()
    {
        if(!m_isMerging)
            Move();
    }

    private void Update()
    {
        UpdateInputState();
        UpdateShoot();
        UpdateAnimation();
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
                m_velocity = m_velocity.normalized * (m_maxSpeed * m_moveInput.magnitude);
        }
        
        // If we are pressing no key or if we want to go in the opposite direction of our current velocity.
        if(m_moveInput.sqrMagnitude == 0
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
        sequence.Append(m_rigidbody.DOMove(position, 0.3f).SetEase(Ease.InCubic));
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
            actionPerformed &= actionReference.action.IsPressed();

        return actionPerformed;
    }
    
    public Vector2 GetVelocity() => m_velocity;
    public void SetVelocity(Vector2 velocity) => m_velocity = velocity;
}
