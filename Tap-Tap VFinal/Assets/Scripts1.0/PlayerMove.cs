using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("La velocidad de movimiento constante del jugador.")]
    public float moveSpeed = 8f;
    [Tooltip("La fuerza de retroceso (impulso) que se aplica al moverse On Beat.")]
    public float moveImpulse = 0.5f; 
    
    [Header("Configuración de Salto")]
    [Tooltip("La fuerza vertical aplicada al saltar.")]
    public float jumpForce = 5f; 
    [Tooltip("LayerMask que define qué es 'suelo' para permitir el salto.")]
    public LayerMask groundLayer;
    [Tooltip("Radio para la deteccin del suelo.")]
    public float groundCheckRadius = 0.2f;

    [Header("Referencias Rítmicas y Feedback")]
    private RhythmManager rhythmManager; 
    // private CameraShake cameraShake; // Descomentar si usas CameraShake
    // private BPMErrorFeedback bpmErrorFeedback; // Descomentar si usas BPMErrorFeedback

    private Rigidbody rb;
    private Vector3 lastInputDirection = Vector3.zero;

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>(); 
        rb = GetComponent<Rigidbody>(); 
        
        if (rb == null)
        {
            Debug.LogError("PlayerMove: Requiere un Rigidbody para el movimiento y salto!");
            enabled = false;
            return;
        }

        // Congelar la rotacin para que el personaje se mantenga de pie
        rb.freezeRotation = true; 
        
        // Asumiendo que tus otras referencias existen:
        // bpmErrorFeedback = FindObjectOfType<BPMErrorFeedback>();
        // cameraShake = Camera.main.GetComponent<CameraShake>();

        if (rhythmManager == null)
        {
            Debug.LogError("PlayerMove: No se encontró el RhythmManager. ¡El juego no funcionará rítmicamente!");
        }
    }

    void Update()
    {
        // 1. Detección de Input de Movimiento
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 currentInputDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. Detección de Salto
        bool jumpInput = Input.GetKeyDown(KeyCode.Space);
        
        // --- LÓGICA DE MOVIMIENTO HORIZONTAL RÍTMICO ---
        // Aplicar impulso solo en el primer frame de la tecla presionada Y si hay beat
        if (currentInputDirection != Vector3.zero && currentInputDirection != lastInputDirection)
        {
            if (rhythmManager != null && rhythmManager.IsTimeToMove())
            {
                // Movimiento exitoso On Beat
                rb.AddForce(currentInputDirection * moveImpulse, ForceMode.VelocityChange);
                // if (cameraShake != null) cameraShake.Shake();
            }
            // else
            // {
                // if (bpmErrorFeedback != null) bpmErrorFeedback.TriggerFlash();
            // }
        }
        
        // --- LÓGICA DE SALTO RÍTMICO ---
        if (jumpInput)
        {
            if (IsGrounded() && rhythmManager != null && rhythmManager.IsTimeToMove())
            {
                // Salto exitoso On Beat: Fuerza instantnea
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        lastInputDirection = currentInputDirection;

        // 3. Aplicación de la Velocidad Constante (Movimiento Fluido)
        if (currentInputDirection.magnitude > 0.1f)
        {
            // Mantener la velocidad horizontal constante, pero permitir la cada vertical
            rb.velocity = new Vector3(currentInputDirection.x * moveSpeed, rb.velocity.y, currentInputDirection.z * moveSpeed);
        }
        else
        {
            // Reducir la velocidad horizontal rpidamente para mejor control
            rb.velocity = new Vector3(rb.velocity.x * 0.9f, rb.velocity.y, rb.velocity.z * 0.9f);
        }
    }
    
    /// <summary>
    /// Verifica si el jugador est tocando el suelo.
    /// </summary>
    private bool IsGrounded()
    {
        // El origen de la esfera est en la base del personaje (o ligeramente por encima)
        Vector3 sphereOrigin = transform.position + Vector3.up * 0.01f; 
        
        // Comprueba si una esfera en los pies toca la capa definida como suelo
        return Physics.CheckSphere(sphereOrigin, groundCheckRadius, groundLayer);
    }
}