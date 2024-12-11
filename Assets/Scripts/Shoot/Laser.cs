using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Laser : MonoBehaviour
{
    [SerializeField] private int m_damageAmout;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(m_damageAmout, ProjectileDamageType.Both);
        }
    }
}
