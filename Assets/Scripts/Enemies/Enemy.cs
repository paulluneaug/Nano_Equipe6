using System;
using UnityEngine;
using UnityUtility.Pools;

public abstract class Enemy : MonoBehaviour
{
    public virtual bool IsAlive => m_health > 0 && !m_outOfBounds;

    [SerializeField] private ProjectileDamageType m_resistances;
    [SerializeField] private int m_maxHealth;
    [SerializeField] private ShootPattern m_shootPattern;

    [SerializeField] private int m_contactDamage;
    [SerializeField] private VFXControllerPool m_vfxPool;

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

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.TakeDamage(m_contactDamage);
        }
    }

    public virtual void TakeDamage(int damage, ProjectileDamageType damageType)
    {
        if ((damageType & ~m_resistances) == 0)
        {
            return;
        }

        m_health -= damage;
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
        PooledObject<VFXController> vfxController = m_vfxPool.Request();

        vfxController.Object.gameObject.SetActive(true);
        vfxController.Object.transform.position = transform.position;
        vfxController.Object.StartVFXLifeCycle(m_vfxPool);

        gameObject.SetActive(false);
    }
}
