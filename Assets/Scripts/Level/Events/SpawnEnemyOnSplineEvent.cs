using UnityEngine;
using UnityEngine.Splines;
using UnityUtility.Pools;

public class SpawnEnemyOnSplineEvent : SpawnEnemyEvent<EnemyOnSpline>
{
    [SerializeField] private SplineContainer m_spline;

    [SerializeField] private int m_numberOfEnemiesToSpawn = 1;
    [SerializeField] private float m_timeBetweenSpawns = 0.0f;

    protected float m_spawnTimer = 0.0f;

    protected override int EnemiesToSpawnCount => m_numberOfEnemiesToSpawn;

    public override void StartEvent()
    {
        base.StartEvent();

        // Spawns the first enemy on the first Update
        m_spawnTimer = m_timeBetweenSpawns;
    }

    protected override PooledObject<EnemyOnSpline> SpawnEnemy()
    {
        PooledObject<EnemyOnSpline> spawnedEnemy = base.SpawnEnemy();
        spawnedEnemy.Object.SetSpline(m_spline);
        return spawnedEnemy;
    }

    public override void UpdateEvent(float deltaTime, float currentWaveTime)
    {
        base.UpdateEvent(deltaTime, currentWaveTime);

        //Spawn the new enemies
        if (m_spawnedEnemiesCount < m_numberOfEnemiesToSpawn)
        {
            if (m_spawnTimer >= m_timeBetweenSpawns)
            {
                m_spawnTimer -= m_timeBetweenSpawns;

                PooledObject<EnemyOnSpline> newEnemy = SpawnEnemy();
                newEnemy.Object.StartEnemy();
                m_spawnedEnemies.Add(new SpawnedEnemy(newEnemy));
                m_spawnedEnemiesCount++;
            }
            m_spawnTimer += deltaTime;
        }
}
}
