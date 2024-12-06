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
    [SerializeField] private List<InputActionReference> m_mergeActions = new();

    [Header("Components")]
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private Animator m_animator;
    
    [Header("Movement")]
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_decelerationFactor;
    
    private Vector2 m_velocity = Vector2.zero;
    
    private void FixedUpdate()
    {
        Move();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        m_animator.SetFloat(s_animatorParamVelocityX, m_velocity.x);
        m_animator.SetFloat(s_animatorParamVelocityY, m_velocity.y);
        m_animator.SetFloat(s_animatorParamVelocityY, m_velocity.y);
    }

    private void Move()
    {
        Vector2 moveInput = GetMoveInput();

        if (moveInput.sqrMagnitude > 0)
        {
            m_velocity += m_acceleration * moveInput;

            if (m_velocity.magnitude > m_maxSpeed)
                m_velocity = m_velocity.normalized * m_maxSpeed; // Thank you JetBrains AI I guess ?
        }
        
        // If we are pressing no key or if we want to go in the opposite direction of our current velocity.
        if(moveInput.sqrMagnitude == 0
           || (moveInput.x < 0 && m_velocity.x > 0)
           || (moveInput.x > 0 && m_velocity.x < 0)
           || (moveInput.y < 0 && m_velocity.y > 0)
           || (moveInput.y > 0 && m_velocity.y < 0))
        {
            m_velocity *= m_decelerationFactor;
        }
        
        m_rigidbody.MovePosition(m_rigidbody.position + m_velocity * Time.fixedDeltaTime);
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

    public bool WantsToMerge()
    {
        if (m_mergeActions.Count == 0)
            return false;
        
        int inputTrueCount = 0;
        foreach (InputActionReference actionReference in m_mergeActions)
            if (actionReference.action.triggered)
                inputTrueCount++;
        
        return inputTrueCount == m_mergeActions.Count;
    }
}
