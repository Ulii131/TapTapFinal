using UnityEngine;
using System.Collections;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab;

    public GameObject warningLightPrefab;
    
    public float currentBPM = 110f; // Necesario para calcular el tiempo de espera
    
    public int beatsToWaitAfterWarning = 2; 

    [Range(0f, 100f)]
    public float spawnChance = 20f; 
    
    public float initialVerticalOffset = -1.5f;
    public float emergenceHeight = 2f; 
    public float emergenceSpeed = 3f;
    
    public float upDurationSeconds = 4f; 
    
    public AudioClip emergenceCrunchSound;
    public float soundVolume = 0.8f;
    
    private GameObject currentWarningLight;

    void Awake()
    {
        RhythmManager.OnBeat += CheckAndSpawnWall; 
    }

    void OnDestroy()
    {
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
        // --- INICIAR SONIDO DE TIERRA/CRUNCH ---
        if (emergenceCrunchSound != null)
        {
            // PlayClipAtPoint es el método ideal para SFX de un solo uso
            AudioSource.PlayClipAtPoint(emergenceCrunchSound, transform.position, soundVolume);
        }
        // --- FIN CÓDIGO DE SONIDO ---
        
        // La posición inicial del muro (sumergido)
        Vector3 spawnPosition = transform.position + (Vector3.up * initialVerticalOffset);
        
        GameObject newWallGO = Instantiate(wallPrefab, spawnPosition, Quaternion.identity);
        
        // Iniciar la animación completa (subida, espera, bajada y destrucción)
        StartCoroutine(WallLifeCycleAnimation(newWallGO));
    }
    
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