using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParkourManager : MonoBehaviour
{
    [Header("Configuración de Plataformas")]
    [Tooltip("Arrastra las plataformas en el ORDEN SECUENCIAL en que deben aparecer.")]
    public RhythmicPlatform[] platforms;

    [Header("Configuración Rítmica")]
    [Tooltip("Beats de retraso antes de que comience el primer ciclo de plataforma.")]
    public int initialDelayBeats = 4;
    
    [Tooltip("Número de beats entre la activación de cada plataforma (Ej: 1 para activar en cada beat).")]
    public int activationIntervalBeats = 1;

    private int currentPlatformIndex = 0;
    private int beatCounter = 0;

    void Awake()
    {
        // Suscribirse al evento OnBeat
        RhythmManager.OnBeat += HandlePlatformActivation;
    }

    void OnDestroy()
    {
        RhythmManager.OnBeat -= HandlePlatformActivation;
    }

    private void HandlePlatformActivation()
    {
        beatCounter++;

        // 1. Manejar el retraso inicial
        if (beatCounter <= initialDelayBeats)
        {
            return;
        }

        // 2. Verificar el intervalo de activación
        if ((beatCounter - initialDelayBeats) % activationIntervalBeats == 0)
        {
            // 3. Activar la plataforma actual
            if (platforms != null && platforms.Length > 0)
            {
                // Asegurarse de que el índice no exceda el límite, reinicia la secuencia
                if (currentPlatformIndex >= platforms.Length)
                {
                    currentPlatformIndex = 0; 
                }

                // Llamar al método de activación de la plataforma
                platforms[currentPlatformIndex].ActivatePlatform();

                Debug.Log($"Plataforma {currentPlatformIndex + 1} activada en beat {beatCounter}");
                
                // Mover al siguiente índice en la secuencia
                currentPlatformIndex++;
            }
        }
    }
    
    // Método para obtener las plataformas hijas automáticamente (opcional, pero útil)
    [ContextMenu("Get Platforms From Children")]
    private void GetPlatformsFromChildren()
    {
        // Esto solo funciona en el Editor (en el modo de edición)
        platforms = GetComponentsInChildren<RhythmicPlatform>();
        Debug.Log($"Encontradas {platforms.Length} plataformas hijas.");
    }
}