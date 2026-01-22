using System;
using UnityEngine;
using UnityUtility.Timer;

public class LaserShootPattern : ShootPattern
{
    public enum LaserShootStep
    {
        Ready,
        LaserOn,
        Cooldown,
    }

    [SerializeField] private Laser m_laser;
    [SerializeField] private Timer m_laserTimer;
    [SerializeField] private Timer m_cooldownTimer;

    [NonSerialized] private bool m_needRelease;
    [NonSerialized] private LaserShootStep m_shootStep;

    public override bool UpdatePattern(float deltaTime)
    {
        if (m_needRelease && !m_shouldShoot)
        {
            m_needRelease = false;
        }

        switch (m_shootStep)
        {
            case LaserShootStep.Ready:
                UpdateLaserReady(deltaTime);
                break;
            case LaserShootStep.LaserOn:
                UpdateLaserOn(deltaTime);
                return true;
            case LaserShootStep.Cooldown:
                UpdateOnCooldown(deltaTime);
                break;
            default:
                break;
        }
        
        return false;
    }

    private void UpdateLaserReady(float deltaTime)
    {
        if (m_shouldShoot && !m_needRelease)
        {
            StartLaser();
        }
    }

    private void StartLaser()
    {
        m_needRelease = true;
        m_shootStep = LaserShootStep.LaserOn;

        m_laserTimer.Start();
        m_laser.ResetLaser();
        m_laser.gameObject.SetActive(true);

        GameManager.Instance.ScreenShake(m_laserTimer.Duration - 0.5f);
    }

    private void UpdateLaserOn(float deltaTime)
    {
        if (m_laserTimer.Update(deltaTime))
        {
            m_shootStep = LaserShootStep.Cooldown;
            m_cooldownTimer.Start();

            m_laser.gameObject.SetActive(false);
            m_laserTimer.Stop();
        }
    }

    private void UpdateOnCooldown(float deltaTime)
    {
        if (m_cooldownTimer.Update(deltaTime))
        {
            m_shootStep = LaserShootStep.Ready;
            m_cooldownTimer.Stop();
        }
    }

    public LaserShootStep GetShootStep()
    {
        return m_shootStep;
    }
}
