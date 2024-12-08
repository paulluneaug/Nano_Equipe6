using UnityEngine;
using UnityUtility.CustomAttributes;

[RequireComponent(typeof(Collider2D))]
public class ProjectileKiller : MonoBehaviour
{
    [SerializeField, Layer] private int m_projectileLayer = 6;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != m_projectileLayer)
        {
            return;
        }

        if (!other.TryGetComponent(out Projectile projectile))
        {
            Debug.LogError($"The layer Projectile shoul only be used on objects with the component {nameof(Projectile)}");
            return;
        }

        projectile.Release();
    }
}
