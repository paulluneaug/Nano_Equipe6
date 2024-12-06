using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Multiplayer")]
    [SerializeField] private int m_playerNumber;
    
    [Header("Input Actions")]
    [SerializeField] private List<InputActionReference> m_moveActions = new List<InputActionReference>();
    [SerializeField] private List<InputActionReference> m_shootActions = new List<InputActionReference>();

    [Header("Components")]
    [SerializeField] private Rigidbody2D m_rigidbody;
    
    [Header("Movement")]
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_decelerationFactor;
    
    private Vector2 m_velocity = Vector2.zero;
    
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 moveInput = GetMoveInput();

        if (moveInput.sqrMagnitude > 0)
        {
            m_velocity += m_acceleration * GetMoveInput();

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
}
