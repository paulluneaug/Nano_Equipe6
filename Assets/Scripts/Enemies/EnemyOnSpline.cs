using System;
using UnityEngine;
using UnityEngine.Splines;
using UnityUtility.Pools;

public class EnemyOnSpline : Enemy
{
    [SerializeField] private float m_speedFactor;
    [SerializeField] private AnimationCurve m_speedAlongSpline;

    [SerializeField] private SplineContainer m_spline;
    [NonSerialized] private float m_splineLength;
    [NonSerialized] private float m_progressAlongSpline;

    public void SetSpline(SplineContainer spline)
    {
        m_spline = spline;
        m_splineLength = spline.CalculateLength();
    }

    private void Start()
    {
        StartEnemy(null);
    }

    public override void StartEnemy(IObjectPool<Enemy> pool)
    {
        base.StartEnemy(pool);
        m_progressAlongSpline = 0.0f;
        m_splineLength = m_spline.CalculateLength();
    }

    protected override void Move(float deltaTime)
    {
        m_progressAlongSpline += m_speedFactor * deltaTime * m_speedAlongSpline.Evaluate(m_progressAlongSpline) / m_splineLength;

        if (m_progressAlongSpline >= 1.0f)
        {
            Release();
            return;
        }

        transform.position = m_spline.EvaluatePosition(m_progressAlongSpline);
    }
}
