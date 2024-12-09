using UnityEditor;
using UnityEngine;
using UnityUtility.CustomAttributes;

public class Level : MonoBehaviour
{
    public float CurrentLevelTime => m_currentLevelTime;

    public float CurrentWaveTime => m_currentWaveTime;

#if UNITY_EDITOR
    [Button(nameof(PopulateWaves))]
    [ShowIf(nameof(m_populateWavesHidden)), SerializeField] private bool m_populateWavesHidden; // Just to display the button
#endif
    [SerializeField] private Wave[] m_waves;

    private float m_currentLevelTime = 0.0f;
    private float m_currentWaveTime = 0.0f;

    private int m_currentWaveIndex = 0;
    private Wave m_currentWave;

    private void Start()
    {
        LoadLevel();
    }

    private void Update()
    {
        if (UpdateLevel(Time.deltaTime))
        {
            Debug.LogError("Level Finished");
        }
    }

    public void LoadLevel()
    {
        m_currentWaveIndex = 0;
        if (m_waves.Length == 0)
        {
            return;
        }
        m_currentWave = m_waves[m_currentWaveIndex];
        m_currentWave.Load(this);

    }

    public bool UpdateLevel(float deltaTime)
    {
        if (m_currentWave == null)
        {
            return true;
        }

        m_currentLevelTime += deltaTime;
        m_currentWaveTime += deltaTime;

        if (m_currentWave.UpdateWave(deltaTime, m_currentWaveTime))
        {
            // Next Wave
            m_currentWave.FinishWave();

            m_currentWaveIndex++;
            if (m_currentWaveIndex < m_waves.Length)
            {
                m_currentWave = m_waves[m_currentWaveIndex];

                m_currentWaveTime = 0.0f;
                m_currentWave.Load(this);
            }
            else
            {
                m_currentWave = null;
                return true;
            }
        }
        return false;
    }

#if UNITY_EDITOR
    private void PopulateWaves()
    {
        m_waves = GetComponentsInChildren<Wave>();
        EditorUtility.SetDirty(this);
    }
#endif
}
