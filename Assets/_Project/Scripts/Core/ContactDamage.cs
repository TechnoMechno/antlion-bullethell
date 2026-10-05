using UnityEngine;

namespace AntLion.Core{
public class ContactDamage : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private int damage = 10;


        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.TryGetComponent(out Health health)) health.TakeDamage(damage, body.position);
        }
    }
    }