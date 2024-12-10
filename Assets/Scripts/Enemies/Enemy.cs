using System;
using UnityEngine;
using UnityUtility.CustomAttributes;

public abstract class Enemy : MonoBehaviour
{
    public virtual bool IsAlive => m_health > 0 && !m_outOfBounds;

    [SerializeField] private ProjectileDamageType m_resistances;
    [SerializeField] private int m_maxHealth;
    [SerializeField] private ShootPattern m_shootPattern;

    [SerializeField, Layer] private int m_projectileLayer;

    [NonSerialized] private int m_health;
    [NonSerialized] private bool m_outOfBounds;


    private void Awake()
    {
        m_shootPattern.ShouldShoot = true;
    }

    protected virtual void Update()
    {
        m_shootPattern.UpdatePattern(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != m_projectileLayer)
        {
            return;
        }

        if (!other.TryGetComponent(out Projectile projectile))
        {
            Debug.LogError($"The layer Projectile should only be used on objects with the component {nameof(Projectile)}");
            return;
        }

        if (projectile.ProjectileSource == ProjectileSource.Enemy)
        {
            return;
        }

        projectile.Release();

        if ((projectile.DamageType & ~m_resistances) == 0)
        {
            return;
        }

        m_health -= projectile.DamageAmout;
        if (m_health <= 0)
        {
            Kill();
        }
    }

    public virtual void StartEnemy()
    {
        m_health = m_maxHealth;
        m_shootPattern.ShouldShoot = true;
        m_outOfBounds = false;
    }

    public void WentOutOfBounds()
    {
        m_outOfBounds = true;
        Kill();
    }

    protected virtual void Kill()
    {
        gameObject.SetActive(false);
    }
}
