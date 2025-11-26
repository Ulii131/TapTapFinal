using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private string enemyTag = "Enemy";


    private Vector3 direction;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float effectOffsetForward = 0.6f;
    [SerializeField] private float effectOffsetUp = 0.25f;

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
            // Obtener punto de contacto aproximado (aquí usamos la posición de la bala)
            Vector3 contactPoint = transform.position;

            // Instanciar efecto simple (sin pool)
            if (hitEffectPrefab != null)
            {
                GameObject go = Instantiate(hitEffectPrefab, Vector3.zero, Quaternion.identity);
                // si el prefab tiene HitEffect se le pasa el transform del enemigo y contactPoint
                HitEffect he = go.GetComponent<HitEffect>();
                if (he != null)
                    he.Play(other.transform, contactPoint, effectOffsetForward, effectOffsetUp);
                else
                {
                    // fallback: posicionar y destruir si no tiene script
                    go.transform.position = other.transform.position + other.transform.forward * effectOffsetForward + Vector3.up * effectOffsetUp;
                    Destroy(go, 0.5f);
                }
            }

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
