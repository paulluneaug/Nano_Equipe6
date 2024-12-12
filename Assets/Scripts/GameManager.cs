using System;
using System.Collections.Generic;
using DG.Tweening;
using SFX;
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

    [SerializeField] private AudioSource m_fusionAudioSource;
    [SerializeField] private AudioSource m_separationAudioSource;
    [SerializeField] private AudioSource m_separationFailedAudioSource;
    
    [SerializeField] private AudioSource m_magicalGirlMusic;
    [SerializeField] private AudioSource m_deousMusic;
    
    private bool m_arePlayersMerged;
    private Timer m_mergeTimer;
    private int m_score;

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
        m_score = 0;
        m_arePlayersMerged = false;
        
        IntroMusicManager.Instance.StopIntroMusic();
        
        m_magicalGirlMusic.volume = 1.0f;
        m_deousMusic.volume = 0.0f;

        // Set input devices to:
        //   - Keyboard and Gamepad 0 (if connected) for player 1
        //   - Keyboard and Gamepad 1 (if connected) for player 2
        if (Gamepad.all.Count > 0)
        {
            m_inputActionAsset.FindActionMap("Player1").devices = new[]
                { InputSystem.GetDevice("Keyboard"), Gamepad.all[0] };
        }

        if (Gamepad.all.Count > 1)
        {
            m_inputActionAsset.FindActionMap("Player2").devices = new[]
                { InputSystem.GetDevice("Keyboard"), Gamepad.all[1] };
        }
        else
        {
            m_inputActionAsset.FindActionMap("Player2").devices = new[]
                { InputSystem.GetDevice("Keyboard") };
        }

        m_mergeTimer = new Timer(m_playerMergeCooldownTime, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameOver();
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            m_player1.DebugInvinsible = !m_player1.DebugInvinsible;
            m_player2.DebugInvinsible = !m_player2.DebugInvinsible;
            m_playerMerged.DebugInvinsible = !m_playerMerged.DebugInvinsible;
        }

        if ((m_arePlayersMerged && m_playerMerged.KnockedDown) ||
            (!m_arePlayersMerged && (m_player1.KnockedDown && m_player2.KnockedDown)))
        {
            GameOver();
            return;
        }


        _ = m_mergeTimer.Update(Time.deltaTime);

        bool canMerge = Vector2.Distance(
            m_player1.transform.position,
            m_player2.transform.position
        ) < m_playerMergeMaxDistance;

        if (canMerge && CheckWantsToMerge())
        {
            ToggleMerge();
        }
    }

    private static void GameOver()
    {
        ReloadScene();
    }

    private static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Instance.StartGameManager();
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
        {
            if (actionReference.action.IsPressed())
            {
                inputTriggeredCount++;
            }
        }

        return inputTriggeredCount == m_mergeActions.Count;
    }

    private void ToggleMerge()
    {
        // Don't try to merge if the cooldown is running.
        if (m_mergeTimer.IsRunning)
        {
            return;
        }

        if (!m_arePlayersMerged)
        {
            Merge();
            m_mergeTimer.Start();
        }
        else
        {
            if (Separate())
            {
                m_mergeTimer.Start();
            }
        }
    }

    private void Merge()
    {
        m_fusionAudioSource.Play();
        
        Sequence musicCrossFadeSequence = DOTween.Sequence();
        musicCrossFadeSequence.Insert(0.0f, m_magicalGirlMusic.DOFade(0.0f, 2.0f));
        musicCrossFadeSequence.Insert(0.0f, m_deousMusic.DOFade(1.0f, 2.0f));

        // Set merged player position to average of individual players' positions.
        Vector3 middlePosition = new(
            (m_player1.transform.position.x + m_player2.transform.position.x) / 2,
            (m_player1.transform.position.y + m_player2.transform.position.y) / 2,
            0
        );

        m_playerMerged.transform.position = middlePosition;

        m_player1.MergeTo(middlePosition);
        m_player2.MergeTo(middlePosition);

        // Set merged player's velocity to average of individual players' velocities.
        m_playerMerged.Velocity = (m_player1.Velocity + m_player2.Velocity) / 2;

        m_arePlayersMerged = true;

        m_playerMerged.gameObject.SetActive(true);
        OnPlayerMerge?.Invoke(true);
    }

    /**
     * Returns true if successfully separated, false if the separation failed (laser is on for example)
     */
    private bool Separate()
    {
        LaserShootPattern laser = (LaserShootPattern)m_playerMerged.GetShootPattern();
        if (laser.GetShootStep() == LaserShootPattern.LaserShootStep.LaserOn)
        {
            m_separationFailedAudioSource.Play();
            return false;
        }

        m_separationAudioSource.Play();
        
        Sequence musicCrossFadeSequence = DOTween.Sequence();
        musicCrossFadeSequence.Insert(0.0f, m_magicalGirlMusic.DOFade(1.0f, 2.0f));
        musicCrossFadeSequence.Insert(0.0f, m_deousMusic.DOFade(0.0f, 2.0f));

        var sequence = DOTween.Sequence();
        _ = sequence.AppendInterval(1.4f);
        sequence.onComplete += () =>
        {
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

            // Swap active objects
            m_player1.gameObject.SetActive(true);
            m_player2.gameObject.SetActive(true);

            m_player1.Separate();
            m_player2.Separate();
        };

        m_arePlayersMerged = false;
        OnPlayerMerge?.Invoke(false);

        return true;
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

    public void AddScore(int scoreValue)
    {
        m_score += scoreValue;
        Debug.Log("Score increased to " + m_score);
    }
}
