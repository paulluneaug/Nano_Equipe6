using UnityEngine;
using UnityUtility.CustomAttributes;

[RequireComponent(typeof(Collider2D))]
public class EnemiesKiller : MonoBehaviour
{

    [SerializeField, Layer] private int m_enemiesLayer = 7;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != m_enemiesLayer)
        {
            return;
        }

        if (!other.TryGetComponent(out Enemy enemy))
        {
            Debug.LogError($"The layer Enemy shoul only be used on objects with the component {nameof(Enemy)}");
            return;
        }

        enemy.WentOutOfBounds();
    }
}
