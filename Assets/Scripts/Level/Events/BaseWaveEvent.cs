using System;
using UnityEditor;
using UnityEngine;

public abstract class BaseWaveEvent : MonoBehaviour
{
    public float Time => m_time;
    public bool IsFinished => m_isFinished;

    [SerializeField] protected float m_time = 0.0f;

    [NonSerialized] protected bool m_isFinished = false;

#if UNITY_EDITOR
    [NonSerialized] protected bool m_editorLoaded;
    [NonSerialized] protected Level m_owningLevel;

#endif



    public virtual void Load()
    {
        m_isFinished = false;
    }

    public virtual void StartEvent()
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateEventEditor();
    }


    public virtual void LoadEventEditor(Level owningLevel)
    {
        m_editorLoaded = true;
        m_owningLevel = owningLevel;
    }

    public virtual void SaveEventEditor()
    {
    }

    public virtual void UpdateEventEditor()
    {

    }

    public virtual void UnloadEventEditor()
    {
        m_editorLoaded = false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position + m_time * GlobalVariablesScriptable.Instance.ScrollSpeed * Vector3.up;

        Vector3 start = center + Vector3.left * 10;
        Vector3 end = center + Vector3.right * 10;

        Gizmos.DrawLine(start, end);
    }
#endif
}
