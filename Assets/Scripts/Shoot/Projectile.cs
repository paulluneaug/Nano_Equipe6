using System;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;

public class Projectile : MonoBehaviour
{
    public ProjectileSource ProjectileSource => m_projectileSource;

    public float DamageAmout => m_damageAmout;

    [SerializeField] private ProjectileSource m_projectileSource;

    [Title("Damages")]
    [SerializeField] private ProjectileDamageType m_damageType;
    [SerializeField] private float m_damageAmout;

    [Title("Movement")]
    [SerializeField] private Vector2 m_direction;
    [SerializeField] private float m_speed;

    [NonSerialized] private IObjectPool<Projectile> m_pool;

    public void StartProjectile(IObjectPool<Projectile> pool, Vector2 direction, float speed)
    {
        m_pool = pool;
        m_direction = direction;
        m_speed = speed;
    }

    public void Release()
    {
        m_pool.Release(this);
    }

    private void Update()
    {
        transform.position += m_direction.XY0() * m_speed * Time.deltaTime;
    }
}
