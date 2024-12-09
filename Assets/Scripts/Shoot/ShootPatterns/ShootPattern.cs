using System;
using UnityEngine;

public abstract class ShootPattern : MonoBehaviour
{
    public bool ShouldShoot { get => m_shouldShoot; set => m_shouldShoot = value; }

    [NonSerialized] private bool m_shouldShoot;

    public abstract void UpdatePattern(float tdeltaTime);
}
