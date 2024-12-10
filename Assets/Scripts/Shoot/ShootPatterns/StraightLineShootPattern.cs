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


    public override void UpdatePattern(float deltaTime)
    {
        m_emitters.ForEach(emitter => emitter.UpdateEmitter(deltaTime, m_shouldShoot));
    }

    private void OnDrawGizmosSelected()
    {
        m_emitters.ForEach(emitter => emitter.DrawGizmos(transform));
    }
}
