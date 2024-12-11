using System;
using UnityEngine;

public abstract class ShootPattern : MonoBehaviour
{
    public bool ShouldShoot { get => m_shouldShoot; set => m_shouldShoot = value; }

    [NonSerialized] protected bool m_shouldShoot;

    public abstract bool UpdatePattern(float tdeltaTime);
}
