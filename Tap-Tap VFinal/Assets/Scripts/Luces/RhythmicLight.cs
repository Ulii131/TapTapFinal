using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class RhythmicLight : MonoBehaviour
{
    [Header("Configuración de Luz Rítmica")]
    [Tooltip("La frecuencia del parpadeo (1 = cada beat, 2 = cada segundo beat, etc.).")]
    [SerializeField] private int beatsPerFlash = 1;
    
    [Tooltip("Intensidad de la luz cuando está ENCENDIDA.")]
    [SerializeField] private float intensityOn = 1.0f;
    
    [Tooltip("Intensidad de la luz cuando está APAGADA (puede ser 0 para parpadeo total).")]
    [SerializeField] private float intensityOff = 0.0f;
    
    private Light targetLight;
    private int beatCounter = 0;

    void Start()
    {
        targetLight = GetComponent<Light>();
        
        // 1. Suscribirse al evento maestro de ritmo
        RhythmManager.OnBeat += FlashLight; 
        
        // Establecer el estado inicial
        if (targetLight != null)
        {
            targetLight.intensity = intensityOff;
        }
    }

    void OnDestroy()
    {
        // 2. Desuscribirse al destruir
        RhythmManager.OnBeat -= FlashLight;
    }

    private void FlashLight()
    {
        // Incrementar el contador con cada beat
        beatCounter++;

        // 3. Verificar si es el momento de parpadear
        if (beatCounter >= beatsPerFlash)
        {
            // Alternar la intensidad de la luz
            if (targetLight.intensity == intensityOff)
            {
                targetLight.intensity = intensityOn;
            }
            else
            {
                targetLight.intensity = intensityOff;
            }
            
            // Reiniciar el contador
            beatCounter = 0;
        }
    }
}