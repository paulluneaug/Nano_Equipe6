using System;
using UnityEngine;
using UnityUtility.Pools;
using UnityUtility.CustomAttributes;
using UnityUtility.MathU;

[Serializable]
public class ProjectileEmitter
{
    [Title("Emmiter Settings")]
    [SerializeField] private float m_angle;
    [SerializeField] private Vector2 m_offset;

    [Space]

    [SerializeField] private float m_shootDelay;
    [SerializeField] private float m_projectileSpeed;
    [SerializeField] private float m_startDelay;

    [Title("Gizmos", italic: true)]
    [SerializeField] private bool m_displayGizmos = true;
    [SerializeField] private Color m_rayColor = Color.red;

    [NonSerialized] private IObjectPool<Projectile> m_projectilePool;
    [NonSerialized] private Transform m_owner;

    // Timer
    [NonSerialized] private bool m_duringStartDelay = true;
    [NonSerialized] private float m_timer;

    public void Initalize(IObjectPool<Projectile> projectilePool, Transform owner)
    {
        m_projectilePool = projectilePool;
        m_owner = owner;
        m_duringStartDelay = true;
    }

    public bool UpdateEmitter(float deltaTime, bool shouldShoot)
    {
        m_timer += deltaTime;
        if (m_duringStartDelay) 
        {
            if (m_timer >= m_startDelay)
            {
                m_timer -= m_startDelay;
                m_duringStartDelay = false;
            }
            return false;
        }

        if (!shouldShoot)
        {
            return false;
        }

        if (m_timer > m_shootDelay) 
        {
            m_timer = 0.0f;
            Shoot();
        }

        return true;
    }

    private void Shoot()
    {
        Projectile requestedProjectile = m_projectilePool.Request().Object;

        Vector3 projectilePosition = m_owner.position + m_offset.XY0();
        Quaternion projectileRotation = Quaternion.Euler(0.0f, 0.0f, m_angle * MathUf.RAD_2_DEG);
        requestedProjectile.transform.SetPositionAndRotation(projectilePosition, projectileRotation);

        requestedProjectile.gameObject.SetActive(true);

        requestedProjectile.StartProjectile(m_projectilePool, GetVectorFromAngle(), m_projectileSpeed);
    }

    private Vector2 GetVectorFromAngle()
    {
        return new Vector2(MathUf.Cos(m_angle), MathUf.Sin(m_angle));
    }

    public void DrawGizmos(Transform owner)
    {
        if (!m_displayGizmos)
        {
            return;
        }

        Color gizmosColor = Gizmos.color;
        Gizmos.color = m_rayColor;

        Vector3 startPosition = owner.position + m_offset.XY0();
        Gizmos.DrawLine(startPosition, startPosition + GetVectorFromAngle().XY0());

        Gizmos.color = gizmosColor;
    }
}
