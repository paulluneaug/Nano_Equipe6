using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityUtility.Singletons;
using UnityUtility.Timer;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private InputActionAsset m_inputActionAsset;
    
    [Header("Players")]
    private Player m_player1;
    private Player m_player2;
    private Player m_playerMerged;
    
    [Header("Merging")]
    [SerializeField] private float m_playerMergeMaxDistance = 2f;
    [SerializeField] private float m_playerMergeCooldownTime = 2f;
    [SerializeField] private List<InputActionReference> m_mergeActions = new();

    private bool m_arePlayersMerged;
    private Timer m_mergeTimer;
    
    /**
     * Invoked every time the players merge or separate.
     * The boolean is set to true if the players are now merged, and false if they are now separated.
     */
    public event Action<bool> OnPlayerMerge;

    protected override void Start()
    {
        StartGameManager();
    }

    public void StartGameManager()
    {
        m_arePlayersMerged = false;
        
        // Set input devices to:
        //   - Keyboard and Gamepad 0 (if connected) for player 1
        //   - Keyboard and Gamepad 1 (if connected) for player 2
        if (Gamepad.all.Count > 0)
            m_inputActionAsset.FindActionMap("Player1").devices = new[]
                { InputSystem.GetDevice("Keyboard"), Gamepad.all[0] };
        
        if (Gamepad.all.Count > 1)
            m_inputActionAsset.FindActionMap("Player2").devices = new[]
                { InputSystem.GetDevice("Keyboard"), Gamepad.all[1] };
        else
            m_inputActionAsset.FindActionMap("Player2").devices = new[]
                { InputSystem.GetDevice("Keyboard") };
        
        m_mergeTimer = new Timer(m_playerMergeCooldownTime, false);
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Thank you JetBrains AI.
        
        m_mergeTimer.Update(Time.deltaTime);
        
        bool canMerge = Vector2.Distance(
            m_player1.transform.position,
            m_player2.transform.position
        ) < m_playerMergeMaxDistance;

        if (canMerge && CheckWantsToMerge())
            ToggleMerge();
    }
    
    private bool CheckWantsToMerge()
    { 
        // If there is no input in the list, return.
        if (m_mergeActions.Count != 2)
        {
            Debug.Log("GameManager.CheckWantsToMerge: There are not 2 actions in the list for merging input.");
            return false;
        }

        // Count the amount of inputs in the list that are triggered.
        int inputTriggeredCount = 0;
        foreach (InputActionReference actionReference in m_mergeActions)
            if (actionReference.action.IsPressed())
                inputTriggeredCount++;
        
        return inputTriggeredCount == m_mergeActions.Count;
    }

    private void ToggleMerge()
    {
        // Don't try to merge if the cooldown is running.
        if (m_mergeTimer.IsRunning)
            return;
        
        if (!m_arePlayersMerged)
            Merge();
        else
            Separate();

        m_mergeTimer.Start();
    }

    private void Merge()
    {
        // Set merged player position to average of individual players' positions.
        Vector3 middlePosition = new(
            (m_player1.transform.position.x + m_player2.transform.position.x) / 2,
            (m_player1.transform.position.y + m_player2.transform.position.y) / 2,
            0
        );

        m_playerMerged.transform.position = middlePosition;
        
        m_player1.MergeToPosition(middlePosition);
        m_player2.MergeToPosition(middlePosition);

        // Set merged player's velocity to average of individual players' velocities.
        m_playerMerged.Velocity = (m_player1.Velocity + m_player2.Velocity) / 2;
        
        m_arePlayersMerged = true;
        
        m_playerMerged.gameObject.SetActive(true);
        OnPlayerMerge?.Invoke(true);
    }

    private void Separate()
    {
        // Swap active objects
        m_player1.gameObject.SetActive(true);
        m_player2.gameObject.SetActive(true);
        m_playerMerged.gameObject.SetActive(false);
        
        // Set individual players' positions to merged players' position,
        // keeping individual player's z-coordinate to avoid z-fighting.
        m_player1.transform.position = new Vector3(
            m_playerMerged.transform.position.x,
            m_playerMerged.transform.position.y,
            m_player1.transform.position.z
        );

        m_player2.transform.position = new Vector3(
            m_playerMerged.transform.position.x,
            m_playerMerged.transform.position.y,
            m_player2.transform.position.z
        );
        
        // Set individual players' velocities to merged player's velocity.
        m_player1.Velocity = m_playerMerged.Velocity;
        m_player2.Velocity = m_playerMerged.Velocity;
        
        m_arePlayersMerged = false;
        OnPlayerMerge?.Invoke(false);
    }

    public void SetPlayer1(Player player)
    {
        m_player1 = player;
    }
    
    public void SetPlayer2(Player player)
    {
        m_player2 = player;
    }
    
    public void SetPlayerMerged(Player player)
    {
        m_playerMerged = player;
    }
}
