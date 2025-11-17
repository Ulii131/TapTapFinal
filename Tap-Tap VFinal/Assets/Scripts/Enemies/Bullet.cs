using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damageAmount = 1;
    public float speed = 10f;
    public float lifetime = 5f;

    // ELIMINAR: private Transform playerTransform;
    private Vector3 moveDirection; // ESTO ALMACENA EL VECTOR DE DIRECCIÓN FIJA

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    // MÉTODO MODIFICADO: Ahora acepta un Vector3 (la dirección)
    public void Initialize(Vector3 direction)
    {
        // Almacenar la dirección fija que la bala debe seguir
        moveDirection = direction;
    }

    void Update()
    {
        // Mover la bala constantemente en la dirección fija
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SimpleHealthSystem healthSystem = other.GetComponent<SimpleHealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            // Puedes añadir efectos de impacto antes de destruir
            Destroy(gameObject);
        }
    }
}