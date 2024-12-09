using UnityEngine;
using UnityEngine.Splines;
using UnityUtility.Pools;

public class SpawnEnemyOnSplineEvent : SpawnEnemyEvent<EnemyOnSpline>
{
    [SerializeField] private SplineContainer m_spline;

    protected override PooledObject<EnemyOnSpline> SpawnEnemy()
    {
        PooledObject<EnemyOnSpline> spawnedEnemy = base.SpawnEnemy();
        spawnedEnemy.Object.SetSpline(m_spline);
        return spawnedEnemy;
    }
}
