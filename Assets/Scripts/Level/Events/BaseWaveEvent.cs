using System;
using UnityEngine;

public abstract class BaseWaveEvent : MonoBehaviour
{
    public float Time => m_time;
    public bool IsFinished => m_isFinished;

    [SerializeField] protected float m_time = 0.0f;

    [NonSerialized] protected bool m_isFinished = false;
    [NonSerialized] private Level m_level;

    public virtual void Load(Level level)
    {
        m_isFinished = false;
        m_level = level;
    }

    public virtual void Start()
    {
    }

    public virtual void UpdateEvent(float deltaTime, float currentWaveTime)
    {
    }

    public virtual void OnWaveEnd()
    {
    }

    protected void Finish()
    {
        m_isFinished = true;
        OnFinish();
    }

    protected virtual void OnFinish()
    {
        Debug.LogError($"Event {name} finished");
    }
}
