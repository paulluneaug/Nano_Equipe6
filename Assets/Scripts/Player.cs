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
    [SerializeField] private float m_deceleration;
    
    private Vector2 m_velocity = Vector2.zero;
    
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        m_rigidbody.MovePosition(m_rigidbody.position + GetMoveInput());
    }
    
    /**
     * Averages the movement input vector from all controllers.
     * If there is only one action in the list, it just returns the value from that action.
     */
    private Vector2 GetMoveInput()
    {
        if (m_moveActions.Count == 0)
        {
            Debug.Log("Player.GetMoveInput: No action in list.");
            return Vector2.zero;
        }

        Vector2 moveInput = Vector2.zero;

        foreach (InputActionReference actionReference in m_moveActions)
        {
            moveInput += actionReference.action.ReadValue<Vector2>();
        }

        return moveInput / m_moveActions.Count;
    }
}
