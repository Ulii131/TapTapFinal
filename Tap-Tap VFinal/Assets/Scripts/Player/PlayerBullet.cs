using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private string enemyTag = "Enemy";

    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 fireDirection, float bulletSpeed)
    {
        direction = fireDirection.normalized;
        speed = bulletSpeed;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1); // Disparo hace 1 de daño
            }

            Destroy(gameObject); // Destruye la bala
        }
        else if (!other.isTrigger && !other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
