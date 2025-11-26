using UnityEngine;
using System.Collections;

public class RhythmicPlatform : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [Tooltip("Altura a la que se elevará la plataforma.")]
    public float riseHeight = 1f; 
    [Tooltip("Velocidad de subida/bajada.")]
    public float animationSpeed = 5f;

    [Header("Configuración Rítmica")]
    [Tooltip("Número de beats que la plataforma permanece elevada.")]
    public int durationInBeats = 4;
    [Tooltip("¡IMPORTANTE! Copia el BPM de tu RhythmManager aquí.")]
    public float currentBPM = 110f; // Necesario para calcular el tiempo de espera
    
    // Variables privadas de posición
    private Vector3 submergedPosition;
    private Vector3 elevatedPosition;

    void Start()
    {
        // Define las posiciones iniciales y finales
        submergedPosition = transform.position;
        elevatedPosition = submergedPosition + Vector3.up * riseHeight;
    }

    /// <summary>
    /// Inicia el ciclo completo: Subir -> Esperar -> Bajar.
    /// Es llamado por el ParkourManager en el beat correcto.
    /// </summary>
    public void ActivatePlatform()
    {
        // Asegurarse de que no haya ciclos previos corriendo
        StopAllCoroutines(); 
        StartCoroutine(PlatformCycle());
    }

    private IEnumerator PlatformCycle()
    {
        float duration = riseHeight / animationSpeed;
        float elapsed = 0f;

        // PARTE 1: SUBIR (Emerger)
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(submergedPosition, elevatedPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = elevatedPosition;

        // PARTE 2: ESPERAR (Duración en Beats)
        float beatDurationTime = 60f / currentBPM;
        
        // Espera X beats, asegurando que se queda arriba el tiempo correcto
        yield return new WaitForSeconds(beatDurationTime * durationInBeats);

        // PARTE 3: BAJAR (Hundirse)
        elapsed = 0f;
        while (elapsed < duration)
        {
            // Lerp de la posición elevada a la posición sumergida
            transform.position = Vector3.Lerp(elevatedPosition, submergedPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = submergedPosition;
    }
}