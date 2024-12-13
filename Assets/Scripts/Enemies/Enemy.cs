using System;
using UnityEngine;
using UnityUtility.Pools;

public abstract class Enemy : MonoBehaviour
{
    public virtual bool IsAlive => m_health > 0 && !m_outOfBounds;

    [SerializeField] private ProjectileDamageType m_resistances;
    [SerializeField] private int m_maxHealth;
    [SerializeField] private ShootPattern m_shootPattern;
    [SerializeField] private ContactDamageTrigger m_contactDamageTrigger;
    
    [SerializeField] private int m_contactDamage;

    [SerializeField] private VFXControllerPool m_damageVfxPool;
    [SerializeField] private VFXControllerPool m_deathVfxPool;

    [SerializeField] private SFXControllerPool m_dieSfxPool;
    [SerializeField] private SFXControllerPool m_hitSfxPool;
    [SerializeField] private SFXControllerPool m_shieldSfxPool;

    [SerializeField] private bool m_playSounds = true;

    [SerializeField] private int m_scoreValue;
    
    [NonSerialized] private int m_health;
    [NonSerialized] private bool m_outOfBounds;


    private void Awake()
    {
        m_shootPattern.ShouldShoot = true;
    }

    private void Start()
    {
        if (m_contactDamageTrigger)
        {
            m_contactDamageTrigger.OnContact += DealContactDamage;
        }
    }

    protected virtual void Update()
    {
        m_shootPattern.UpdatePattern(Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // If the enemy has a contact damage trigger, ignore damage from the rest of the collider.
        if (m_contactDamageTrigger)
        {
            return;
        }
        
        DealContactDamage(other);
    }

    private void DealContactDamage(Collider2D other)
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
            PlayShieldSfx();
            return;
        }

        m_health -= damage;
        bool killed = m_health <= 0;


        PlayDamageSFX(killed);
        PlayDamageVFX(killed);

        if (killed)
        {
            Kill();
            GameManager.Instance.AddScore(m_scoreValue);
        }
    }

    private void PlayShieldSfx()
    {
        if (!m_playSounds)
        {
            return;
        }

        PooledObject<SFXController> sfxController = m_shieldSfxPool.Request();
        
        sfxController.Object.gameObject.SetActive(true);
        sfxController.Object.StartSFXLifeCycle(m_shieldSfxPool);
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
        Kill(); // No VFX or SFX
    }

    protected virtual void Kill()
    {
        gameObject.SetActive(false);
    }

    private void PlayDamageVFX(bool killed)
    {
        if (killed)
        {
            PlayVFX(m_deathVfxPool);
        }
        else
        {
            PlayVFX(m_damageVfxPool);
        }
    }

    private void PlayDamageSFX(bool killed)
    {
        if (!m_playSounds)
        {
            return;
        }

        if (killed)
        {
            PlaySFX(m_dieSfxPool);
        }
        else
        {
            PlaySFX(m_hitSfxPool);
        }
    }

    private void PlaySFX(SFXControllerPool pool)
    {
        if (pool == null)
        {
            return;
        }

        PooledObject<SFXController> sfxController = pool.Request();

        sfxController.Object.gameObject.SetActive(true);
        sfxController.Object.transform.position = transform.position;
        sfxController.Object.StartSFXLifeCycle(pool);
    }

    private void PlayVFX(VFXControllerPool pool)
    {
        if (pool == null)
        {
            return;
        }

        PooledObject<VFXController> vfxController = pool.Request();

        vfxController.Object.gameObject.SetActive(true);
        vfxController.Object.transform.position = transform.position;
        vfxController.Object.StartVFXLifeCycle(pool);
    }
}
