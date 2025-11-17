using UnityEngine;
using System.Collections;

public class TimedTrainCrossing : MonoBehaviour
{
    [Header("Referencias de Objetos")]
    [Tooltip("El GameObject del tren (debe tener un BoxCollider).")]
    public GameObject trainObject;
    
    [Tooltip("El componente MeshRenderer de la luz de advertencia (el material).")]
    public MeshRenderer warningLightRenderer;
    
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

        // 1. Calcular la velocidad necesaria para el cruce
        trainSpeed = crossingDistance / crossingDuration;

        // 2. Establecer puntos A y B
        startPosition = trainObject.transform.position;
        // Asumiendo que el cruce es en el Eje X:
        endPosition = startPosition + new Vector3(crossingDistance, 0, 0); 
        
        // 3. Inicializar el ciclo
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
            // --- Fase de Espera y Advertencia (7 segundos) ---
            
            // 1. Luz Verde: Dejar pasar (duración del ciclo menos el tiempo de cruce)
            float waitTime = cycleTime - crossingDuration;
            
            // Opcional: Podrías añadir un parpadeo en los últimos segundos aquí
            yield return new WaitForSeconds(waitTime); 

            // --- Fase de Cruce (3 segundos) ---
            
            // 2. Luz Roja: Viene el tren
            SetLightColor(redLightColor);
            Debug.Log("¡Advertencia! Tren viene...");

            // 3. Mover el tren y activar su Collider
            StartCoroutine(MoveTrain());

            // 4. Esperar a que el tren termine de cruzar (el tiempo de cruce)
            yield return new WaitForSeconds(crossingDuration); 

            // 5. Reiniciar posición y Luz Verde
            trainObject.transform.position = startPosition;
            SetLightColor(greenLightColor);
        }
    }
    
    private IEnumerator MoveTrain()
    {
        isCrossing = true;
        // Opcional: Activar Collider aquí si el tren está desactivado en la espera
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

        // Asegurar que el tren est exactamente en la posicin final para evitar imprecisiones de float
        trainObject.transform.position = endPosition;
        
        // Opcional: Desactivar Collider para la espera
        trainObject.GetComponent<Collider>().enabled = false; 
        isCrossing = false;
    }
}