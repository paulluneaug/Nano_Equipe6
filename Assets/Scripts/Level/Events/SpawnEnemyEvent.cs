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

    [SerializeField] private int m_numberOfEnemiesToSpawn = 1;
    [SerializeField] private float m_timeBetweenSpawns = 0.0f;

    [SerializeField] private BaseEnemyPool<TEnemy> m_enemyPool;

    //Cache 
    protected List<SpawnedEnemy> m_spawnedEnemies;
    protected int m_spawnedEnemiesCount = 0;
    protected float m_spawnTimer = 0.0f;

    public override void Load(Level level)
    {

        base.Load(level);
        m_spawnedEnemies = new List<SpawnedEnemy>(m_numberOfEnemiesToSpawn);
    }

    public override void Start()
    {
        base.Start();
        // Spawns the first enemy on the first Update
        m_spawnTimer = m_timeBetweenSpawns;
    }

    public override void UpdateEvent(float deltaTime, float currentWaveTime)
    {
        base.UpdateEvent(deltaTime, currentWaveTime);

        if (m_isFinished)
        {
            return;
        }

        //Spawn the new enemies
        if (m_spawnedEnemiesCount < m_numberOfEnemiesToSpawn)
        {
            if (m_spawnTimer >= m_timeBetweenSpawns)
            {
                m_spawnTimer -= m_timeBetweenSpawns;

                PooledObject<TEnemy> newEnemy = SpawnEnemy();
                newEnemy.Object.StartEnemy();
                m_spawnedEnemies.Add(new SpawnedEnemy(newEnemy));
                m_spawnedEnemiesCount++;
            }
            m_spawnTimer += deltaTime;
        }

        // Checks wether all spawned enemies are dead
        bool noEnemiesAlive = !m_spawnedEnemies.Any(enemy => enemy.IsAlive);

        if (m_spawnedEnemiesCount == m_numberOfEnemiesToSpawn && noEnemiesAlive)
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
