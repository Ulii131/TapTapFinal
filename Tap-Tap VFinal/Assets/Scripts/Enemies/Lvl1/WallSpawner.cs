using UnityEngine;
using System.Collections;

public class WallSpawner : MonoBehaviour
{
    [Header("Referencias de Muro y Anuncio")]
    [Tooltip("El Prefab de la Tumba o Muro que emerge.")]
    public GameObject wallPrefab;
    [Tooltip("El Prefab de la luz/disco que anuncia la posición del muro.")]
    public GameObject warningLightPrefab;
    
    [Header("Configuración Rítmica")]
    [Tooltip("¡IMPORTANTE! Copia el BPM de tu RhythmManager aquí.")]
    public float currentBPM = 110f; // Necesario para calcular el tiempo de espera
    
    [Tooltip("El número de beats que la luz de anuncio estará encendida antes de que la pared salga.")]
    public int beatsToWaitAfterWarning = 2; 
    
    [Tooltip("La probabilidad (0-100%) de que un muro sea programado en un beat, si no hay otro activo.")]
    [Range(0f, 100f)]
    public float spawnChance = 20f; 
    
    [Header("Configuración de Emergencia del Muro")]
    public float initialVerticalOffset = -1.5f;
    public float emergenceHeight = 2f; 
    public float emergenceSpeed = 3f;
    
    [Header("Configuración de Duración del Muro")]
    [Tooltip("Tiempo en segundos que la tumba permanecerá levantada antes de hundirse.")]
    public float upDurationSeconds = 4f; 
    
    private GameObject currentWarningLight;

    void Awake()
    {
        // FIX CS0070: Eliminamos la verificación de null en el evento.
        // Asumimos que la clase RhythmManager y el evento estático están disponibles.
        RhythmManager.OnBeat += CheckAndSpawnWall; 
    }

    void OnDestroy()
    {
        // FIX CS0070: Eliminamos la verificación de null en el evento.
        RhythmManager.OnBeat -= CheckAndSpawnWall;
    }

    private void CheckAndSpawnWall()
    {
        // Solo programamos un nuevo muro si no hay uno anunciado
        if (currentWarningLight != null) return; 
        
        // 1. Aplicar la probabilidad de spawn
        if (Random.Range(0f, 100f) < spawnChance)
        {
            // 2. Si ganamos la probabilidad, iniciamos la secuencia anunciada
            StartCoroutine(AnnounceAndSpawnWall());
        }
    }

    private IEnumerator AnnounceAndSpawnWall()
    {
        // Paso 1: ANUNCIAR CON LUZ EN EL SUELO
        Vector3 lightSpawnPos = transform.position;
        lightSpawnPos.y = 0.05f; 
        
        currentWarningLight = Instantiate(warningLightPrefab, lightSpawnPos, Quaternion.identity);
        
        float beatDuration = 60f / currentBPM;
        
        // Paso 2: ESPERAR BEATS
        for (int i = 0; i < beatsToWaitAfterWarning; i++)
        {
            yield return new WaitForSeconds(beatDuration);
        }
        
        // Paso 3: DESTRUIR LUZ y Llamar a SpawnWallLogic
        Destroy(currentWarningLight);
        currentWarningLight = null;

        SpawnWallLogic();
    }
    
    private void SpawnWallLogic()
    {
        // La posición inicial del muro (sumergido)
        Vector3 spawnPosition = transform.position + (Vector3.up * initialVerticalOffset);
        
        GameObject newWallGO = Instantiate(wallPrefab, spawnPosition, Quaternion.identity);
        
        // Iniciar la animación completa (subida, espera, bajada y destrucción)
        StartCoroutine(WallLifeCycleAnimation(newWallGO));
    }
    
    /// <summary>
    /// Gestiona todo el ciclo de vida del muro: Subir, Esperar (4s), Bajar y Destruir.
    /// </summary>
    private IEnumerator WallLifeCycleAnimation(GameObject wall)
    {
        Vector3 startPos = wall.transform.position; // Posición sumergida
        Vector3 targetPos = startPos + (Vector3.up * emergenceHeight); // Posición levantada
        float duration = emergenceHeight / emergenceSpeed; 
        float elapsed = 0f;

        // PARTE 1: SUBIR (Emerger)
        while (elapsed < duration)
        {
            wall.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        wall.transform.position = targetPos;
        
        // PARTE 2: ESPERAR (4 Segundos Levantado)
        yield return new WaitForSeconds(upDurationSeconds); 
        
        // PARTE 3: BAJAR (Hundirse)
        elapsed = 0f;
        Vector3 currentPos = wall.transform.position; // Posición actual (levantada)
        
        while (elapsed < duration) // Usamos la misma duración y velocidad para hundirse
        {
            wall.transform.position = Vector3.Lerp(currentPos, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        wall.transform.position = startPos;
        
        // PARTE 4: DESTRUCCIÓN
        Destroy(wall);
    }
}