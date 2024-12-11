using System;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyOnSpline : MovingEnemy
{
    public override bool IsAlive => base.IsAlive && !(m_killAtTheEndOfSpline && m_progressAlongSpline >= 1.0f);
    [SerializeField] private AnimationCurve m_speedAlongSpline;

    [SerializeField] private bool m_killAtTheEndOfSpline = true;

    [NonSerialized] private SplineContainer m_spline;
    [NonSerialized] private float m_splineLength;
    [NonSerialized] private float m_progressAlongSpline;

    public void SetSpline(SplineContainer spline)
    {
        m_spline = spline;
        m_splineLength = spline.CalculateLength();
    }

    public override void StartEnemy()
    {
        base.StartEnemy();
        m_progressAlongSpline = 0.0f;
        m_splineLength = m_spline.CalculateLength();
        transform.position = m_spline.EvaluatePosition(m_progressAlongSpline);
    }

    protected override void Move(float deltaTime)
    {
        float addedProgress = m_speedFactor * deltaTime * m_speedAlongSpline.Evaluate(m_progressAlongSpline);

        if (m_useGlobalScrollFactor)
        {
            addedProgress *= m_globalVariablesScriptable.ScrollSpeed;
        }

        m_progressAlongSpline += addedProgress / m_splineLength;

        if (m_progressAlongSpline >= 1.0f)
        {
            if (m_killAtTheEndOfSpline)
            {
                Kill(); // No VFX or SFX
            }
            return;
        }

        transform.position = m_spline.EvaluatePosition(m_progressAlongSpline);
    }
}
