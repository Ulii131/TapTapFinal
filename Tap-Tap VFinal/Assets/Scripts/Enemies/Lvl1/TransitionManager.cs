using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    [Header("Referencias de Objetos")]
    [Tooltip("El GameObject de la casa embrujada.")]
    public GameObject hauntedHouse;
    [Tooltip("El GameObject del suelo/cráter de parkour.")]
    public GameObject parkourGround;
    [Tooltip("El trigger que el jugador activa al final del cementerio.")]
    public Collider endTrigger;

    [Header("Configuración de Movimiento")]
    [Tooltip("Distancia que la casa debe AVANZAR o RETROCEDER para dar espacio. (Ej: 30 para avanzar)")]
    public float movementDistance = 30f; // Ahora es una distancia genérica
    [Tooltip("Tiempo que dura la animación de separación (en segundos).")]
    public float separationDuration = 2.0f;
    [Tooltip("Curva para suavizar el movimiento (Ease In Out).")]
    public AnimationCurve movementCurve; 

    // Variable para asegurar que la animación solo se dispara una vez
    private bool hasTriggered = false;

    void Start()
    {
        // Asegurarse de que el cráter esté inicialmente visible o sumergido, 
        // pero la casa debe estar colocada tapándolo completamente.
        // parkourGround.SetActive(false); // Descomentar si el cráter comienza desactivado
    }

    /// <summary>
    /// Llamado cuando el jugador entra al trigger final.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 1. Verifica la Tag del jugador y si ya se disparó la transición
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            // Desactiva el trigger para que no se dispare de nuevo
            endTrigger.enabled = false; 

            // 2. Inicia la secuencia de transición
            StartCoroutine(SeparateSequence());
        }
    }

    private IEnumerator SeparateSequence()
    {
        // Posiciones iniciales
        Vector3 initialHousePos = hauntedHouse.transform.position;
        
        // ¡CAMBIO CLAVE! Mover 30 unidades hacia adelante (Z positivo)
        Vector3 targetHousePos = initialHousePos + (Vector3.forward * movementDistance); 
        
        // Opcional: Activar el cráter si estaba deshabilitado
        parkourGround.SetActive(true); 

        // 2. Ejecutar la animación de movimiento
        float elapsed = 0f;
        while (elapsed < separationDuration)
        {
            float t = elapsed / separationDuration;
            float curveValue = movementCurve.Evaluate(t); 

            // Mover la casa hacia adelante
            hauntedHouse.transform.position = Vector3.Lerp(initialHousePos, targetHousePos, curveValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegurar la posición final
        hauntedHouse.transform.position = targetHousePos;

        Debug.Log("Transición completada. Casa movida 30 unidades hacia adelante.");
    }
}