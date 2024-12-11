using System;
using UnityEngine;

public class ContactDamageTrigger : MonoBehaviour
{
    public event Action<Collider2D> OnContact;

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnContact?.Invoke(other);
    }
}
