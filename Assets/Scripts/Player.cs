using System.Collections.Generic;
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
    
    [Header("Movement")]
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_decelerationFactor;
    
    private Vector2 m_velocity = Vector2.zero;
    
    // Input State
    private Vector2 m_moveInput;
    private bool m_wantsToMerge;
    
    // ========== Unity Methods ==========
    // ===================================
    
    private void FixedUpdate()
    {
        UpdateInputState();
        Move();
        UpdateAnimation();
    }

    // ========== Movement ==========
    // ==============================
    
    private void Move()
    {
        if (m_moveInput.sqrMagnitude > 0)
        {
            m_velocity += m_acceleration * m_moveInput;

            if (m_velocity.magnitude > m_maxSpeed)
                m_velocity = m_velocity.normalized * m_maxSpeed; // Thank you JetBrains AI I guess ?
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
    
    // ========== Input ==========
    // ===========================

    private void UpdateInputState()
    {
        m_moveInput = GetMoveInput();
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
    
    public Vector2 GetVelocity() => m_velocity;
    public Vector2 SetVelocity(Vector2 velocity) => m_velocity = velocity;
}
