using UnityEngine;
using UnityUtility.Utils;

[RequireComponent(typeof(Collider2D))]
public class EnemiesKiller : MonoBehaviour
{
    [SerializeField] private LayerMask m_enemiesLayers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_enemiesLayers.Contains(other.gameObject.layer))
        {
            return;
        }

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.WentOutOfBounds();
        }

    }
}
