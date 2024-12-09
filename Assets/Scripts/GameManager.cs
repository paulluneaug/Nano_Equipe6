using UnityEngine;
using UnityEngine.InputSystem;
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

        if (canMerge && m_player1.GetWantsToMerge() && m_player2.GetWantsToMerge())
            ToggleMerge();
    }

    private void ToggleMerge()
    {
        // Don't try to merge if the cooldown is running.
        if (m_mergeTimer.IsRunning)
            return;
        
        Debug.Log("Trying to merge.");
        if (m_arePlayersMerged)
        {
            m_player1.gameObject.SetActive(false);
            m_player2.gameObject.SetActive(false);
            m_playerMerged.gameObject.SetActive(true);
            m_arePlayersMerged = true;
        }
        else
        {
            m_player1.gameObject.SetActive(true);
            m_player2.gameObject.SetActive(true);
            m_playerMerged.gameObject.SetActive(false);
            m_arePlayersMerged = false;
        }

        m_mergeTimer.Start();
    }
}
