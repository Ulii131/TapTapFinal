using UnityEngine;
using System.Collections;

public class RhythmicSpin : MonoBehaviour
{
    [Header("Configuración Rítmica")]
    [Tooltip("Cada cuántos beats el enemigo realizará un giro de 360 grados.")]
    public int beatsPerSpin = 4;
    
    [Header("Giro")]
    [Tooltip("El tiempo (en segundos) que durará la rotación completa de 360 grados.")]
    public float spinDuration = 0.5f; 

    private RhythmManager rhythmManager;
    private int beatCounter = 0;
    private bool isSpinning = false; // Bandera para evitar que se interrumpa un giro

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager == null) 
            Debug.LogError("RhythmicSpin: No se encontró RhythmManager.");

        // Suscribirse al evento maestro de ritmo
        if (rhythmManager != null)
        {
            RhythmManager.OnBeat += CheckAndExecuteSpin;
        }
    }

    void OnDestroy()
    {
        // Desuscribirse al destruir
        if (rhythmManager != null)
        {
            RhythmManager.OnBeat -= CheckAndExecuteSpin;
        }
    }

    private void CheckAndExecuteSpin()
    {
        // 1. Contar los beats
        beatCounter++;

        // 2. Si el contador coincide con el intervalo Y no está ya girando
        if (beatCounter >= beatsPerSpin && !isSpinning)
        {
            StartCoroutine(PerformSpin());
            beatCounter = 0;
        }
    }

    private IEnumerator PerformSpin()
    {
        isSpinning = true; 
        float startTime = Time.time;
        float totalRotation = 0f;
        float degreesPerSecond = 360f / spinDuration;

        while (totalRotation < 360f)
        {
            // Calcula cuántos grados rotar este frame
            float degreesToRotate = degreesPerSecond * Time.deltaTime;
            
            // Asegura que no gire ms de 360 grados en el ltimo frame
            if (totalRotation + degreesToRotate > 360f)
            {
                degreesToRotate = 360f - totalRotation;
            }

            // Rotar en el eje Y (vertical)
            transform.Rotate(Vector3.up, degreesToRotate, Space.Self);
            
            totalRotation += degreesToRotate;
            
            yield return null; // Esperar al siguiente frame
        }

        isSpinning = false; // El giro ha terminado
    }
}