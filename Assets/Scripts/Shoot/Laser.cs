using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Laser : MonoBehaviour
{
    [SerializeField] private int m_damageAmout;

    [NonSerialized] private HashSet<Enemy> m_touchedEnemies;

    public void ResetLaser()
    {
        m_touchedEnemies ??= new HashSet<Enemy>();
        m_touchedEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (!m_touchedEnemies.Add(enemy))
            {
                return;
            }
            enemy.TakeDamage(m_damageAmout, ProjectileDamageType.Both);
        }
    }
}
