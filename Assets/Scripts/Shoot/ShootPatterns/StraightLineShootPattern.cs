using UnityEngine;
using UnityUtility.Utils;

public class StraightLineShootPattern : ShootPattern
{
    [SerializeField] private ProjectileEmitter[] m_emitters = new ProjectileEmitter[0];
    [SerializeField] private ProjectilePool m_projectilesPool;

    private void Awake()
    {
        m_emitters.ForEach(emitter => emitter.Initalize(m_projectilesPool, transform));
    }


    public override bool UpdatePattern(float deltaTime)
    {
        bool isUpdating = false;
        foreach (ProjectileEmitter emitter in m_emitters)
        {
            isUpdating |= emitter.UpdateEmitter(deltaTime, m_shouldShoot);
        }
        
        return isUpdating;
    }

    private void OnDrawGizmosSelected()
    {
        m_emitters.ForEach(emitter => emitter.DrawGizmos(transform));
    }
}
