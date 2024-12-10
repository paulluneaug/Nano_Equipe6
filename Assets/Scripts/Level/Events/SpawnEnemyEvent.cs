using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility.Pools;

public abstract class SpawnEnemyEvent<TEnemy> : BaseWaveEvent
    where TEnemy : Enemy
{
    protected struct SpawnedEnemy
    {
        public bool IsAlive => m_enemy.Object.IsAlive;

        private readonly PooledObject<TEnemy> m_enemy;

        public SpawnedEnemy(PooledObject<TEnemy> enemy)
        {
            m_enemy = enemy;
        }

        public void ReleaseEnemy()
        {
            m_enemy.Release();
        }
    }


    [SerializeField] protected BaseEnemyPool<TEnemy> m_enemyPool;

    protected abstract int EnemiesToSpawnCount { get; }

    //Cache 
    protected List<SpawnedEnemy> m_spawnedEnemies;
    protected int m_spawnedEnemiesCount = 0;

    public override void Load()
    {
        base.Load();
        m_spawnedEnemies = new List<SpawnedEnemy>(EnemiesToSpawnCount);
    }

    public override void UpdateEvent(float deltaTime, float currentWaveTime)
    {
        base.UpdateEvent(deltaTime, currentWaveTime);

        if (m_isFinished)
        {
            return;
        }

        // Checks wether all spawned enemies are dead
        bool noEnemiesAlive = !m_spawnedEnemies.Any(enemy => enemy.IsAlive);

        if (m_spawnedEnemiesCount == EnemiesToSpawnCount && noEnemiesAlive)
        {
            Finish();
        }
    }

    protected override void OnFinish()
    {
        base.OnFinish();
        m_spawnedEnemies.ForEach(enemy => enemy.ReleaseEnemy());
    }

    protected virtual PooledObject<TEnemy> SpawnEnemy()
    {
        PooledObject<TEnemy> requestedEnemy = m_enemyPool.Request();

        requestedEnemy.Object.gameObject.SetActive(true);

        return requestedEnemy;
    }
}
