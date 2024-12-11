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
    [SerializeField] private VFXControllerPool m_vfxPool;
    
    [SerializeField] private SFXControllerPool m_dieSfxPool;
    [SerializeField] private SFXControllerPool m_hitSfxPool;
    [SerializeField] private SFXControllerPool m_shieldSfxPool;
    
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

        if (m_health <= 0)
        {
            PlayKillVfxAndSfx();
            Kill();
        }
        else
        {
            PlayDamageSfx();
        }
    }

    private void PlayShieldSfx()
    {
        PooledObject<SFXController> sfxController = m_shieldSfxPool.Request();
        
        sfxController.Object.gameObject.SetActive(true);
        sfxController.Object.StartSFXLifeCycle(m_shieldSfxPool);
    }

    private void PlayDamageSfx()
    {
        PooledObject<SFXController> sfxController = m_hitSfxPool.Request();
        
        sfxController.Object.gameObject.SetActive(true);
        sfxController.Object.StartSFXLifeCycle(m_hitSfxPool);
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

    private void PlayKillVfxAndSfx()
    {
        PooledObject<VFXController> vfxController = m_vfxPool.Request();

        vfxController.Object.gameObject.SetActive(true);
        vfxController.Object.transform.position = transform.position;
        vfxController.Object.StartVFXLifeCycle(m_vfxPool);

        PooledObject<SFXController> sfxController = m_dieSfxPool.Request();
        
        sfxController.Object.gameObject.SetActive(true);
        sfxController.Object.StartSFXLifeCycle(m_dieSfxPool);
    }
}
