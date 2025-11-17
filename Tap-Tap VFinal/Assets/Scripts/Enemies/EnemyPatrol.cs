using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    // public float moveInterval = 3f; <-- ELIMINADO
    public int maxSteps = 5; // Cuadrículas antes de cambiar dirección
    public float stepSize = 1f; // Tamaño de la cuadrícula (1 unidad)

    private int currentStep = 0;
    private int direction = 1; // 1 para derecha, -1 para izquierda
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        // Suscribirse al evento que se dispara en cada beat
        RhythmManager.OnBeat += MoveStep; 
    }

    void OnDestroy()
    {
        // Importante: Desuscribirse del evento al destruir el objeto
        RhythmManager.OnBeat -= MoveStep;
    }

    // Este método se llama *exactamente* en cada beat
    void MoveStep()
    {
        // Mover 1 paso en la dirección actual
        transform.position += new Vector3(direction * stepSize, 0, 0);
        currentStep++;

        // Si llegó a maxSteps, cambia dirección y resetea contador
        if (currentStep >= maxSteps)
        {
            direction *= -1;
            currentStep = 0;
        }
    }
    
    // NOTA: Eliminar la Coroutine MoveRoutine()
}