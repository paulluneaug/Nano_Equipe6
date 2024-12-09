using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityUtility.Singletons;
using UnityUtility.Timer;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private InputActionAsset m_inputActionAsset;
    
    [Header("Players")]
    [SerializeField] private Player m_player1;
    [SerializeField] private Player m_player2;
    [SerializeField] private Player m_playerMerged;
    
    [Header("Merging")]
    [SerializeField] private float m_playerMergeMaxDistance = 2f;
    [SerializeField] private float m_playerMergeCooldownTime = 2f;
    [SerializeField] private List<InputActionReference> m_mergeActions = new();

    private bool m_arePlayersMerged;
    private Timer m_mergeTimer;

    protected override void Start()
    {
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
        {
            Debug.Log("GameManager.ToggleMerge: Merging.");
            m_player1.gameObject.SetActive(false);
            m_player2.gameObject.SetActive(false);
            m_playerMerged.gameObject.SetActive(true);
            m_arePlayersMerged = true;
        }
        else
        {
            Debug.Log("GameManager.ToggleMerge: Separating.");
            m_player1.gameObject.SetActive(true);
            m_player2.gameObject.SetActive(true);
            m_playerMerged.gameObject.SetActive(false);
            m_arePlayersMerged = false;
        }

        m_mergeTimer.Start();
    }
}
