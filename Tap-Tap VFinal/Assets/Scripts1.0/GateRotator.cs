using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateRotator : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Ángulo final al que rotará el objeto en el eje Y (ej: -90 para abrir a la izquierda).")]
    public float targetRotationY = -90f; 
    
    [Tooltip("Velocidad de rotación. Cuanto mayor sea, más rápido se abrirá el portón.")]
    public float rotationSpeed = 3f;

    private bool isActivated = false;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Guardar la rotación inicial del objeto al comenzar el juego
        initialRotation = transform.rotation;
        
        // Calcular la rotación final (solo afecta al eje Y, dejando X y Z igual)
        targetRotation = Quaternion.Euler(
            transform.eulerAngles.x, 
            initialRotation.eulerAngles.y + targetRotationY, 
            transform.eulerAngles.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es el jugador y si el mecanismo no ha sido activado
        // Asegúrate de que tu jugador tenga el Tag "Player"
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            Debug.Log("Portón activado por el jugador.");
        }
    }

    void Update()
    {
        // Si el mecanismo ha sido activado y la rotación no ha terminado
        if (isActivated)
        {
            // Mover la rotación actual gradualmente hacia la rotación objetivo
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime * rotationSpeed
            );

            // Opcional: Si el portón está casi en su posición final, detener el Update
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                // Forzar la posición final exacta
                transform.rotation = targetRotation; 
                enabled = false; // Desactivar el script después de la rotación para ahorrar recursos
            }
        }
    }
}