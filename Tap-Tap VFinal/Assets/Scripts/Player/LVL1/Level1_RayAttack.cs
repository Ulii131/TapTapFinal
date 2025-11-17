using UnityEngine;

public class Level1_RayAttack : MonoBehaviour
{
    [Header("Configuración del Rayo de Luz")]
    [Tooltip("Máxima distancia que puede alcanzar el rayo.")]
    public float rayDistance = 20f;
    [Tooltip("El daño infligido al enemigo si impacta.")]
    public int damageAmount = 1;
    [Tooltip("La capa de objetos (enemigos) que el rayo debe detectar.")]
    public LayerMask enemyLayer;

    [Header("Referencias")]
    [Tooltip("El punto de origen del rayo (ej. la punta de la linterna o la cámara).")]
    public Transform shootOrigin; 
    [Tooltip("Prefab del efecto visual del rayo (LineRenderer) que se destruye rápido.")]
    public GameObject rayEffectPrefab; 

    private RhythmManager rhythmManager; 

    void Start()
    {
        // Si no se asigna un origen de disparo, usa la posición y dirección de este GameObject.
        if (shootOrigin == null)
        {
            shootOrigin = transform; 
        }

        rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager == null) 
            Debug.LogError("RayAttack: No se encontró RhythmManager.");
    }

    void Update()
    {
        // Detecta el input del click derecho
        if (Input.GetMouseButtonDown(1))
        {
            // Valida el ritmo usando el RhythmManager
            if (rhythmManager != null && rhythmManager.IsTimeToMove())
            {
                ShootRay();
            }
        }
    }

    void ShootRay()
    {
        Vector3 origin = shootOrigin.position;
        Vector3 direction = shootOrigin.forward; 
        RaycastHit hit;

        // Lanzar el rayo
        if (Physics.Raycast(origin, direction, out hit, rayDistance, enemyLayer))
        {
            // Rayo impactó un enemigo
            
            // Instanciar efecto visual. La rotación apunta el rayo en la dirección del impacto.
            if (rayEffectPrefab != null)
            {
                Instantiate(rayEffectPrefab, origin, Quaternion.LookRotation(direction));
            }

            // Aplicar daño
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
        else
        {
            // Rayo no impactó, solo se crea el efecto visual hasta el límite de distancia
            if (rayEffectPrefab != null)
            {
                Instantiate(rayEffectPrefab, origin, Quaternion.LookRotation(direction));
            }
        }
    }
}