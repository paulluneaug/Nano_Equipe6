using System;
using UnityEngine;

public abstract class MovingEnemy : Enemy
{
    [SerializeField] protected float m_speedFactor;
    [SerializeField] protected bool m_useGlobalScrollFactor = true;

    [NonSerialized] protected GlobalVariablesScriptable m_globalVariablesScriptable;

    protected virtual void Awake()
    {
        if (m_useGlobalScrollFactor)
        {
            m_globalVariablesScriptable = GlobalVariablesScriptable.Instance;
        }
    }

    protected override void Update()
    {
        base.Update();
        Move(Time.deltaTime);
    }
    protected abstract void Move(float deltaTime);
}
