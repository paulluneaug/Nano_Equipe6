using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private InputActionAsset m_inputActionAsset;
    
    [Header("Players")]
    [SerializeField] private Player m_player1;
    [SerializeField] private Player m_player2;
    [SerializeField] private Player m_playerMerged;
    
    [Header("Merging")]
    [SerializeField] private float m_playerMergeMaxDistance = 2f;

    private bool m_arePlayersMerged;

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
    }
    
    private void Update()
    {
        bool canMerge = Vector2.Distance(
            m_player1.transform.position,
            m_player2.transform.position
        ) < m_playerMergeMaxDistance;

        if (canMerge && m_player1.WantsToMerge() && m_player2.WantsToMerge())
            ToggleMerge();
    }

    private void ToggleMerge()
    {
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
    }
}
