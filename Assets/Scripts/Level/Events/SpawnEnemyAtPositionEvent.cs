using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;
using UnityUtility.Utils;

public class SpawnEnemyAtPositionEvent : SpawnEnemyEvent<Enemy>
{
    [Serializable]
    protected class SpawnPoint
    {
        public Vector3 Position;

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SpawnPoint other)
            {
                return other.Position.Equals(Position);
            }
            return false;
        }
    }

    protected override int EnemiesToSpawnCount => m_spawnPoints.Length;

    [SerializeField]
    [Disable]
    private SpawnPoint[] m_spawnPoints;

#if UNITY_EDITOR
    [NonSerialized] private Dictionary<SpawnPoint, Transform> m_loadedPositions;
#endif


    public override void StartEvent()
    {
        base.StartEvent();

        foreach (SpawnPoint spawnPoint in m_spawnPoints)
        {
            PooledObject<Enemy> spawnedEnemy = base.SpawnEnemy();
            spawnedEnemy.Object.transform.position = spawnPoint.Position;
            spawnedEnemy.Object.StartEnemy();

            m_spawnedEnemies.Add(new SpawnedEnemy(spawnedEnemy));

            m_spawnedEnemiesCount++;
        }
    }

#if UNITY_EDITOR
    private Vector3 GetHandleFromSpawnPoint(SpawnPoint spawnPoint)
    {
        return m_owningLevel.transform.position + spawnPoint.Position + m_time * m_owningLevel.GlobalScrollFactor * Vector3.up;
    }

    private Vector3 GetSpawnPointFromHandle(Transform handle)
    {
        return handle.position - m_owningLevel.transform.position - m_time * m_owningLevel.GlobalScrollFactor * Vector3.up;
    }

    private Transform CreateHandle(int i)
    {
        GameObject spawnPointHandle = new GameObject($"SpawnPoint_{i}");
        Transform handleTransform = spawnPointHandle.transform;
        handleTransform.parent = transform;

        // Visual
        GameObject enemyPrefab = Instantiate(m_enemyPool.Prefab, handleTransform);
        enemyPrefab.SetActive(true);

        return handleTransform;
    }

    public override void LoadEventEditor(Level owningLevel)
    {
        if (m_editorLoaded)
        {
            return;
        }

        base.LoadEventEditor(owningLevel);
        m_loadedPositions = new Dictionary<SpawnPoint, Transform>();

        for (int i = 0; i < m_spawnPoints.Length; i++)
        {
            SpawnPoint spawnPoint = m_spawnPoints[i];

            // To prevent errors when a SpawnPoint is duplicated and Unity considers that they equals
            while (m_loadedPositions.ContainsKey(spawnPoint))
            {
                spawnPoint = NudgeSpawnPoint(i);
            }

            Transform handleTransform = CreateHandle(i);

            handleTransform.position = GetHandleFromSpawnPoint(spawnPoint);


            m_loadedPositions.Add(spawnPoint, handleTransform);
        }
    }

    private SpawnPoint NudgeSpawnPoint(int i)
    {
        SpawnPoint nudged = new SpawnPoint
        {
            Position = m_spawnPoints[i].Position + Vector3.up,
        };
        m_spawnPoints[i] = nudged;
        return nudged;
    }

    public override void UpdateEventEditor()
    {
        base.UpdateEventEditor();

        if (!m_editorLoaded)
        {
            return;
        }

        HashSet<Vector3> seenPositions = new HashSet<Vector3>();
        for (int iSpawnPoint = 0; iSpawnPoint < m_spawnPoints.Length; iSpawnPoint++)
        {
            SpawnPoint spawnPoint = m_spawnPoints[iSpawnPoint];

            if (spawnPoint == null)
            {
                continue;
            }

            // If the element is dupicated in the list
            while (!seenPositions.Add(spawnPoint.Position))
            {
                spawnPoint = NudgeSpawnPoint(iSpawnPoint);
            }

            // The handle is already created
            if (m_loadedPositions.TryGetValue(spawnPoint, out Transform handle))
            {
                handle.position = GetHandleFromSpawnPoint(spawnPoint);
            }
            // The handle does not already exist
            else
            {
                Transform handleTransform = CreateHandle(iSpawnPoint);
                handleTransform.position = GetHandleFromSpawnPoint(spawnPoint);

                m_loadedPositions.Add(spawnPoint, handleTransform);
            }
        }

        // Too many handles
        if (m_loadedPositions.Count > m_spawnPoints.Length)
        {
            Span<SpawnPoint> pointsToDelete = new SpawnPoint[m_loadedPositions.Count - m_spawnPoints.Length];
            int nextIndex = 0;
            foreach (var pair in m_loadedPositions)
            {
                SpawnPoint spawnPoint = pair.Key;
                // The handle is no longer in the spawnSoints
                if (!m_spawnPoints.Contains(spawnPoint))
                {
                    pointsToDelete[nextIndex++] = spawnPoint;

                    pair.Value.gameObject.Destroy();
                }
            }

            for (int iToDelete = 0; iToDelete < nextIndex; iToDelete++)
            {
                _ = m_loadedPositions.Remove(pointsToDelete[iToDelete]);
            }
        }
    }

    public override void SaveEventEditor()
    {
        base.SaveEventEditor();

        if (!m_editorLoaded)
        {
            return;
        }

        foreach (var pair in m_loadedPositions)
        {
            pair.Key.Position = GetSpawnPointFromHandle(pair.Value);
        }
    }

    public override void UnloadEventEditor()
    {
        if (!m_editorLoaded)
        {
            return;
        }
        base.UnloadEventEditor();

        m_loadedPositions.ForEach(pair => pair.Value.gameObject.Destroy());
        m_loadedPositions.Clear();
    }

    private void OnDrawGizmos()
    {
        if (m_editorLoaded)
        {
            DrawGizmos();
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        if (m_spawnPoints == null)
        {
            return;
        }

        Color gizmosColor = Gizmos.color;

        Gizmos.color = Color.magenta;
        foreach (SpawnPoint spawnPoint in m_spawnPoints)
        {
            Gizmos.DrawWireSphere(transform.position + spawnPoint.Position, 0.5f);
        }

        Gizmos.color = gizmosColor;
    }
#endif
}
