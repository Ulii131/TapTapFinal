using UnityEngine;

public class CameraPeek : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("La sensibilidad de la cmara a los movimientos del ratón.")]
    public float sensitivity = 0.5f; 
    
    [Tooltip("El ngulo mximo que la cmara puede rotar en X e Y (ej. 5 grados).")]
    public float maxAngle = 5f; 
    
    [Tooltip("Velocidad de interpolacin para un movimiento suave.")]
    public float smoothSpeed = 10f;

    private Quaternion initialRotation;
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Guardar la rotacin inicial de la cmara (su posicin "fija")
        initialRotation = transform.localRotation;
        
        // Opcional: Bloquear y ocultar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Obtener el input del ratón
        float inputMouseX = Input.GetAxis("Mouse X") * sensitivity;
        float inputMouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 2. Acumular y Limitar la Rotación (Vertical: Eje X, Horizontal: Eje Y)
        
        // Acumular la rotación. Clamp para limitar el ángulo total
        rotationX += inputMouseY;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);
        
        rotationY += inputMouseX;
        rotationY = Mathf.Clamp(rotationY, -maxAngle, maxAngle);

        // 3. Crear la rotación objetivo combinando la rotación inicial y la nueva rotación
        
        // La rotación se aplica sobre los ejes globales del mundo y luego se combina con la rotacin inicial.
        Quaternion targetRotation = initialRotation * Quaternion.Euler(-rotationX, rotationY, 0f);

        // 4. Aplicar la rotación suavemente (Slerp)
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}