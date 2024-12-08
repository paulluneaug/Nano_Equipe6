using System;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private ProjectileDamageType m_resistances;
    [SerializeField] private int m_health;
    [SerializeField] private ShootPattern m_shootPattern;

    [SerializeField, Layer] private int m_projectileLayer;

    [NonSerialized] private IObjectPool<Enemy> m_pool;


    private void Awake()
    {
        m_shootPattern.ShouldShoot = true;
    }

    private void Update()
    {
        m_shootPattern.UpdatePattern(Time.deltaTime);
        Move(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != m_projectileLayer)
        {
            return;
        }

        if (!other.TryGetComponent(out Projectile projectile))
        {
            Debug.LogError($"The layer Projectile shoul only be used on objects with the component {nameof(Projectile)}");
            return;
        }

        if (projectile.ProjectileSource == ProjectileSource.Enemy)
        {
            return;
        }

        projectile.Release();
    }

    public virtual void StartEnemy(IObjectPool<Enemy> pool)
    {
        m_pool = pool;
    }

    protected abstract void Move(float deltaTime);

    protected virtual void Release()
    {
        m_pool.Release(this);
    }
}
