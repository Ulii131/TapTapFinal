using UnityEngine;
using System.Collections;

public class TimedTrainCrossing : MonoBehaviour
{
    [Header("Referencias de Objetos")]
    [Tooltip("El GameObject del tren (debe tener un BoxCollider).")]
    public GameObject trainObject;
    
    [Tooltip("El componente MeshRenderer de la luz de advertencia (el material).")]
    public MeshRenderer warningLightRenderer;
    
    // --- INICIO CÓDIGO DE SONIDO ---
    [Header("Configuración de Audio del Tren")]
    [Tooltip("El clip de sonido del tren (motor/traqueteo) a reproducir en loop.")]
    public AudioClip trainPassSound;
    private AudioSource trainAudioSource; // Referencia al AudioSource del tren
    // --- FIN CÓDIGO DE SONIDO ---

    [Header("Configuración de Tiempo y Velocidad")]
    [Tooltip("Tiempo en segundos entre cada cruce (incluye el tiempo de cruce).")]
    public float cycleTime = 10f; 
    
    [Tooltip("Tiempo que dura el cruce del tren de A a B.")]
    public float crossingDuration = 3f;
    
    [Tooltip("Distancia total que recorrerá el tren (debe ser la longitud de tu nivel).")]
    public float crossingDistance = 50f; 

    [Header("Configuración de Materiales")]
    public Color greenLightColor = Color.green;
    public Color redLightColor = Color.red;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float trainSpeed;
    private bool isCrossing = false;

    void Start()
    {
        if (trainObject == null || warningLightRenderer == null)
        {
            Debug.LogError("¡Faltan referencias de Tren o Luz de Advertencia en TimedTrainCrossing!");
            enabled = false;
            return;
        }

        // --- INICIO CÓDIGO DE SONIDO ---
        // 1. Obtener el AudioSource del objeto del tren
        trainAudioSource = trainObject.GetComponent<AudioSource>();
        if (trainAudioSource == null)
        {
            Debug.LogWarning("El GameObject del tren no tiene un componente AudioSource. Añadiendo uno.");
            trainAudioSource = trainObject.AddComponent<AudioSource>();
        }
        
        if (trainAudioSource != null && trainPassSound != null)
        {
            trainAudioSource.clip = trainPassSound;
            trainAudioSource.loop = true; // Configurar para que suene en loop
            trainAudioSource.playOnAwake = false; // Asegurarse de que no suene al inicio
        }
        // --- FIN CÓDIGO DE SONIDO ---

        // 2. Calcular la velocidad necesaria para el cruce
        trainSpeed = crossingDistance / crossingDuration;

        // 3. Establecer puntos A y B
        startPosition = trainObject.transform.position;
        // Asumiendo que el cruce es en el Eje X:
        endPosition = startPosition + new Vector3(crossingDistance, 0, 0); 
        
        // 4. Inicializar el ciclo
        SetLightColor(greenLightColor);
        StartCoroutine(TrainCycle());
    }

    private void SetLightColor(Color color)
    {
        // Cambia el color del material de la luz (asumiendo que tiene un material con color Emission o base)
        warningLightRenderer.material.color = color;
    }

    private IEnumerator TrainCycle()
    {
        while (true) // Bucle infinito de cruce
        {
            // --- Fase de Espera y Advertencia ---
            
            // Luz Verde: Dejar pasar
            float waitTime = cycleTime - crossingDuration;
            yield return new WaitForSeconds(waitTime); 

            // --- Fase de Cruce ---
            
            // Luz Roja: Viene el tren
            SetLightColor(redLightColor);
            Debug.Log("¡Advertencia! Tren viene...");

            // 1. Mover el tren y activar su Collider
            StartCoroutine(MoveTrain());
            
            // 2. Esperar a que el tren termine de cruzar
            yield return new WaitForSeconds(crossingDuration); 

            // 3. Reiniciar posición y Luz Verde
            trainObject.transform.position = startPosition;
            SetLightColor(greenLightColor);
            
            // --- DETENER SONIDO (Al terminar el cruce) ---
            if (trainAudioSource != null && trainAudioSource.isPlaying)
            {
                trainAudioSource.Stop();
            }
            // ---------------------------------------------
        }
    }
    
    private IEnumerator MoveTrain()
    {
        isCrossing = true;
        
        // --- INICIAR SONIDO (Al comenzar el cruce) ---
        if (trainAudioSource != null && !trainAudioSource.isPlaying)
        {
            trainAudioSource.Play();
        }
        // ---------------------------------------------
        
        // Activar Collider
        trainObject.GetComponent<Collider>().enabled = true; 

        float currentTravelTime = 0;
        
        while (currentTravelTime < crossingDuration)
        {
            // Distancia = Velocidad * Tiempo
            float distanceToMove = trainSpeed * Time.deltaTime;
            
            trainObject.transform.Translate(Vector3.right * distanceToMove, Space.World);

            currentTravelTime += Time.deltaTime;
            yield return null; // Esperar al siguiente frame
        }

        // Asegurar que el tren esté exactamente en la posición final 
        trainObject.transform.position = endPosition;
        
        // Desactivar Collider
        trainObject.GetComponent<Collider>().enabled = false; 
        isCrossing = false;
    }
}