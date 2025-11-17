using UnityEngine;
using System.Collections;

public class WildWestShooter : MonoBehaviour
{
    [Header("Referencias Rítmicas")]
    [Tooltip("Cada cuntos beats el enemigo girar y disparar.")]
    public int beatsPerAction = 4;
    
    private RhythmManager rhythmManager;
    private int beatCounter = 0;
    
    [Header("Ataque y Rotación")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float spinDuration = 0.5f; // El tiempo que dura la rotacin 360
    public float moveSpeed = 3f; // Velocidad de movimiento lateral (si no tienes script de movimiento)

    private bool isSpinning = false;
    private Vector3 movementDirection = Vector3.left; // Asumimos que se mueve de Derecha a Izquierda

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager == null) 
            Debug.LogError("WildWestShooter: No se encontr RhythmManager.");

        // Suscribirse al evento maestro de ritmo
        RhythmManager.OnBeat += CheckAndExecuteAction;
    }

    void OnDestroy()
    {
        // Desuscribirse al destruir
        if (rhythmManager != null)
        {
            RhythmManager.OnBeat -= CheckAndExecuteAction;
        }
    }

    void Update()
    {
        // La nica tarea de Update es el movimiento lateral, si no est girando
        if (!isSpinning)
        {
            // Simple movimiento lateral (Ajusta esto si usas un script de movimiento distinto)
            transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
            // Podras aadir aqu la lgica para cambiar 'movementDirection' al llegar a un lmite.
        }
    }

    private void CheckAndExecuteAction()
    {
        // 1. Contar los beats
        beatCounter++;

        // 2. Si el contador coincide con el intervalo y no est ya girando
        if (beatCounter >= beatsPerAction && !isSpinning)
        {
            StartCoroutine(SpinAndShoot());
            beatCounter = 0;
        }
    }

    private IEnumerator SpinAndShoot()
    {
        isSpinning = true; // Bloquea el movimiento lateral
        float startTime = Time.time;
        float degreesPerSecond = 360f / spinDuration;
        bool hasFired = false;
        
        while (Time.time < startTime + spinDuration)
        {
            // 1. Rotacin: Gira en el eje Y (vertical)
            transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.Self);
            
            // 2. Disparo: Disparar justo a la mitad del giro (para dar tiempo a la animacin)
            if (!hasFired && (Time.time - startTime) >= (spinDuration / 2f))
            {
                FireProjectile();
                hasFired = true;
            }
            
            yield return null; // Esperar al siguiente frame
        }

        // Asegurar que la rotacin termine exactamente en 360 grados (opcional, para limpieza)
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
                                                 transform.localEulerAngles.y, 
                                                 transform.localEulerAngles.z); // Depende de tu pivot.

        isSpinning = false; // Permite que el movimiento lateral contine
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Instanciar y lanzar el proyectil desde el punto de disparo
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            // Asegúrate de que tu prefab de proyectil tenga un script para darle velocidad inicial
            
            // Ejemplo de cdigo para el proyectil (asume que Bullet.cs existe)
            // if (bullet.TryGetComponent<Bullet>(out Bullet b))
            // {
            //     b.Initialize(transform.forward, bulletSpeed); // Inicializa con direccin hacia adelante
            // }
        }
        else
        {
            Debug.LogWarning("¡Faltan referencias de proyectil o punto de disparo!");
        }
    }
}