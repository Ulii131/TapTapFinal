using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightningFlasher : MonoBehaviour
{
    [Header("Configuración de Intensidad")]
    [Tooltip("Intensidad máxima del relámpago (ej. 8.0 - 15.0).")]
    public float maxFlashIntensity = 10f;
    [Tooltip("Intensidad base de la luz (la oscuridad normal del cielo).")]
    public float ambientIntensity = 0.5f;

    [Header("Configuración de Tiempos")]
    [Tooltip("Tiempo mínimo de espera entre secuencias de relámpagos.")]
    public float minWaitTime = 5f;
    [Tooltip("Tiempo máximo de espera entre secuencias de relámpagos.")]
    public float maxWaitTime = 15f;
    [Tooltip("Duración de cada destello individual (muy corto para simular luz).")]
    public float flashDuration = 0.05f;

    private Light targetLight;
    
    void Start()
    {
        targetLight = GetComponent<Light>();

        if (targetLight == null)
        {
            Debug.LogError("LightningFlasher requiere un componente Light.");
            enabled = false;
            return;
        }

        // Establecer la intensidad base
        targetLight.intensity = ambientIntensity;
        
        // Iniciar la rutina de relámpagos
        StartCoroutine(StartLightningSequence());
    }

    private IEnumerator StartLightningSequence()
    {
        while (true)
        {
            // 1. Esperar un tiempo aleatorio hasta el próximo relámpago
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 2. Determinar la cantidad de destellos en la secuencia (1 a 3)
            int numFlashes = Random.Range(1, 4); 

            for (int i = 0; i < numFlashes; i++)
            {
                // 3. Destello: Aumentar la intensidad instantáneamente
                targetLight.intensity = maxFlashIntensity;
                
                // 4. Mantener el destello por una corta duración
                yield return new WaitForSeconds(flashDuration);
                
                // 5. Apagar o bajar al nivel ambiente
                targetLight.intensity = ambientIntensity;

                // 6. Esperar un tiempo muy corto entre destellos rápidos (solo si hay más de uno)
                if (i < numFlashes - 1)
                {
                    float pause = Random.Range(flashDuration * 2, flashDuration * 4);
                    yield return new WaitForSeconds(pause);
                }
            }
        }
    }
}