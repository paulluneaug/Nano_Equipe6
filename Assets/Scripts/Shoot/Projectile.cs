using System;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;

public class Projectile : MonoBehaviour
{
    public ProjectileSource ProjectileSource => m_projectileSource;
    public int DamageAmout => m_damageAmout;
    public ProjectileDamageType DamageType => m_damageType;


    [SerializeField] private ProjectileSource m_projectileSource;

    [Title("Damages")]
    [SerializeField] private ProjectileDamageType m_damageType;
    [SerializeField] private int m_damageAmout;

    [Title("Movement")]
    [SerializeField] private Vector2 m_direction;
    [SerializeField] private float m_speed;

    [NonSerialized] private IObjectPool<Projectile> m_pool;
    [NonSerialized] private bool m_shouldRelease = false;

    public void StartProjectile(IObjectPool<Projectile> pool, Vector2 direction, float speed)
    {
        m_pool = pool;
        m_direction = direction;
        m_speed = speed;
        m_shouldRelease = false;
    }

    public void Release()
    {
        m_shouldRelease = true;
    }

    private void Update()
    {
        if (m_shouldRelease)
        {
            m_pool.Release(this);
            m_shouldRelease = false;
            return;
        }

        transform.position += m_direction.XY0() * m_speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (m_projectileSource != ProjectileSource.Player)
            {
                player.TakeDamage(m_damageAmout);
                Release();
            }
        }

        if (other.TryGetComponent(out Enemy enemy))
        {
            if (m_projectileSource != ProjectileSource.Enemy)
            {
                enemy.TakeDamage(m_damageAmout, m_damageType);
                Release();
            }
        }
    }
}
