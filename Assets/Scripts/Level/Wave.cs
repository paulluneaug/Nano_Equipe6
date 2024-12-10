using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Utils;

public class Wave : MonoBehaviour
{
#if UNITY_EDITOR
    [Title("Editor")]
    [Button(nameof(LoadWaveEditor))]
    [Button(nameof(SaveWaveEditor))]
    [Button(nameof(UnloadWaveEditor))]
    [Title("Events")]
    [Button(nameof(PopulateEvents))]
    [ShowIf(nameof(m_populateEventsHidden)), SerializeField] private bool m_populateEventsHidden; // Just to display the button
#endif
    [SerializeField] private BaseWaveEvent[] m_waveEvents;

    private List<BaseWaveEvent> m_startedEvents;

    private int m_eventToStartIndex = 0;
    private int m_eventsCount;

    public void Load()
    {
        m_waveEvents.Sort(
            (BaseWaveEvent event1, BaseWaveEvent event2) =>
            event1.Time.CompareTo(event2.Time));

        m_eventToStartIndex = 0;
        m_eventsCount = m_waveEvents.Length;
        m_startedEvents = new List<BaseWaveEvent>(m_eventsCount);

        m_waveEvents.ForEach(waveEvent => waveEvent.Load());
    }

    public bool UpdateWave(float deltaTime, float currentWaveTime)
    {
        // Starts the new events
        while (m_eventToStartIndex < m_eventsCount
            && currentWaveTime >= m_waveEvents[m_eventToStartIndex].Time)
        {
            BaseWaveEvent eventToStart = m_waveEvents[m_eventToStartIndex];

            m_startedEvents.Add(eventToStart);

            eventToStart.StartEvent();
            m_eventToStartIndex++;
        }

        // Updates all started evants
        bool allEventFinished = true;
        foreach (BaseWaveEvent startedEvent in m_startedEvents)
        {
            if (startedEvent.IsFinished)
            {
                continue;
            }
            allEventFinished = false;
            startedEvent.UpdateEvent(deltaTime, currentWaveTime);
        }

        // Checks whether the wave is finished by checking whether all events started and finished
        if (allEventFinished && m_startedEvents.Count == m_eventsCount)
        {
            return true;
        }
        return false;
    }

    public void FinishWave()
    {
        Debug.LogError($"Wave {name} finished");
        m_waveEvents.ForEach(waveEvent => waveEvent.OnWaveEnd());
    }


#if UNITY_EDITOR

    private void OnValidate()
    {
        UpdateWaveEditor();
    }

    private void PopulateEvents()
    {
        m_waveEvents = GetComponentsInChildren<BaseWaveEvent>();
        m_waveEvents.Sort(
            (BaseWaveEvent event1, BaseWaveEvent event2) =>
            event1.Time.CompareTo(event2.Time));

        EditorUtility.SetDirty(this);
    }

    public void LoadWaveEditor()
    {
        m_waveEvents.ForEach(waveEvent => waveEvent.LoadEventEditor(GetComponentInParent<Level>()));
    }

    public void UpdateWaveEditor()
    {
        m_waveEvents.ForEach(waveEvent => waveEvent.UpdateEventEditor());
    }

    public void SaveWaveEditor()
    {
        m_waveEvents.ForEach(waveEvent => waveEvent.SaveEventEditor());
        m_waveEvents.ForEach(waveEvent => EditorUtility.SetDirty(waveEvent));
        EditorUtility.SetDirty(this);
    }

    public void UnloadWaveEditor()
    {
        m_waveEvents.ForEach(waveEvent => waveEvent.UnloadEventEditor());
    }
#endif
}
