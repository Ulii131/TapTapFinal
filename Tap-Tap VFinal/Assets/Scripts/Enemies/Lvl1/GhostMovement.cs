using System.Collections;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public float emergenceHeight = 2f; 

    public float emergenceSpeed = 1.5f;
    
    public float forwardSpeed = 5f;
    public Vector3 direction = Vector3.forward;
    public float lateralOffset = 0f; 
    public float lifetimeSeconds = 7f; 
    public int damageAmount = 10; 
    
    // --- INICIO CÓDIGO DE SONIDO ---
    public AudioSource ambientAudioSource;
    public AudioClip ambientSpookySound;
    // --- FIN CÓDIGO DE SONIDO ---
    
    private Vector3 targetPosition;
    private bool isEmerging = false;
    private bool isFloating = false;
    private bool hasDealtDamage = false; 
    
    void Start()
    {
        // 1. Programar la destrucción del fantasma
        Destroy(gameObject, lifetimeSeconds); 

        // APLICAR MOVIMIENTO LATERAL: Ajustar la posición inicial en X
        if (lateralOffset != 0f)
        {
            transform.position += Vector3.right * lateralOffset;
        }

        // 2. Definir la posición objetivo una vez emerge
        targetPosition = transform.position + (Vector3.up * emergenceHeight);
        
        // 3. Obtener AudioSource si no se asignó en el Inspector (seguridad)
        if (ambientAudioSource == null)
        {
            ambientAudioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isEmerging)
        {
            // Mover hacia la posición objetivo (arriba)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, emergenceSpeed * Time.deltaTime);

            // Si llegamos a la posición objetivo, pasamos al estado flotante
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isEmerging = false;
                isFloating = true;
                
                // --- INICIAR SONIDO AMBIENTAL ---
                if (ambientAudioSource != null && ambientSpookySound != null)
                {
                    ambientAudioSource.clip = ambientSpookySound;
                    ambientAudioSource.loop = true; // Reproducir en loop
                    ambientAudioSource.Play();
                }
                // --- FIN CÓDIGO DE SONIDO ---

                Debug.Log("Fantasma Termina de Emerger. Comienza a Flotar.");
            }
        }

        if (isFloating)
        {
            // Mover hacia adelante en línea recta
            transform.Translate(direction * forwardSpeed * Time.deltaTime, Space.World);
        }
    }
    
    /// <summary>
    /// Detecta cuando el fantasma colisiona con otro objeto (ya que es un Trigger).
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 1. Verificar si el daño ya fue aplicado o si el objeto no es el jugador
        if (hasDealtDamage || !other.CompareTag("Player"))
        {
            return;
        }

        // 2. Obtener el componente de salud del jugador (usando SimpleHealthSystem)
        SimpleHealthSystem playerHealth = other.GetComponent<SimpleHealthSystem>();

        if (playerHealth != null)
        {
            // 3. Aplicar el daño
            playerHealth.TakeDamage(damageAmount);
            hasDealtDamage = true; // Marca el daño como aplicado
            
            Debug.Log($"El fantasma colisionó con el jugador. Daño aplicado: {damageAmount}");
            
            // Opcional: Si quieres que el fantasma desaparezca inmediatamente después de golpear
            // Destroy(gameObject);
        }
    }

    
    /// <summary>
    /// Método público para iniciar la secuencia de emergencia.
    /// Es llamado por el GhostSpawner.
    /// </summary>
    public void EmergeAndMove()
    {
        isEmerging = true;
    }

    private void OnBecameInvisible()
    {
        // Se destruye si sale del campo de visión (método de limpieza)
        Destroy(gameObject);
    }
    
    // --- INICIO CÓDIGO DE SONIDO (Limpieza) ---
    void OnDestroy()
    {
        // Asegurar que el sonido se detenga si se destruye antes de tiempo
        if (ambientAudioSource != null)
        {
            ambientAudioSource.Stop();
        }
    }
}