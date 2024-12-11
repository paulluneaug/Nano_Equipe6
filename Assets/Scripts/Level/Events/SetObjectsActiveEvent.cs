using System;
using UnityEngine;
using UnityUtility.Utils;

public class SetObjectsActiveEvent : BaseWaveEvent
{
    [Serializable]
    private struct ObjectState
    {
        public GameObject GameObject => m_gameObject;
        public bool TargetState => m_targetState;

        [SerializeField] private GameObject m_gameObject;
        [SerializeField] private bool m_targetState;
    }

    [SerializeField] private ObjectState[] m_objectsToSetActive;


    public override void StartEvent()
    {
        base.StartEvent();
        m_objectsToSetActive.ForEach(obj => obj.GameObject.SetActive(obj.TargetState));
        Finish();
    }
}
