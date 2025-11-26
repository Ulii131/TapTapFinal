using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [Header("Referencias de Fantasma")]
    [Tooltip("Arrastra aquí el Prefab del Fantasma con el script GhostMovement.")]
    public GameObject ghostPrefab;
    
    [Header("Configuración de Spawn")]
    [Tooltip("Desplazamiento vertical inicial para que el fantasma comience sumergido.")]
    public float initialVerticalOffset = -1.5f; 
    [Tooltip("Rotación inicial para que el fantasma mire hacia donde debe ir (eje Y).")]
    public float spawnRotationY = 0f;

    [Header("Control de Carril Aleatorio")]
    [Tooltip("Distancia en el eje X que representa un 'carril'.")]
    public float laneDistance = 1.5f; // Distancia entre carriles (ajusta este valor)

    [Header("Control de Intervalo Aleatorio")] 
    [Tooltip("Mínimo número de beats entre spawns (ej: 2).")]
    public int minSpawnInterval = 2; 
    
    [Tooltip("Máximo número de beats entre spawns (ej: 8).")]
    public int maxSpawnInterval = 8;
    
    private int beatsUntilNextSpawn; // El número de beats que hay que esperar.
    private int currentBeatCounter = 0; // Contador de beats transcurridos.
    
    // ----------------------------------------------------
    // SUSCRIPCIÓN AL RITMO
    // ----------------------------------------------------
    
    void Awake()
    {
        RhythmManager.OnBeat += CheckAndSpawnInterval; 
        SetRandomInterval();
    }
    
    void OnDestroy()
    {
        RhythmManager.OnBeat -= CheckAndSpawnInterval;
    }
    
    private void SetRandomInterval()
    {
        beatsUntilNextSpawn = Random.Range(minSpawnInterval, maxSpawnInterval + 1);
        Debug.Log($"Próximo fantasma saldrá en {beatsUntilNextSpawn} beats.");
    }
    
    /// <summary>
    /// Se llama en CADA beat de la música. Gestiona el contador y el spawn.
    /// </summary>
    private void CheckAndSpawnInterval() 
    {
        currentBeatCounter++; 
        
        if (currentBeatCounter >= beatsUntilNextSpawn)
        {
            SpawnGhost(); 
            
            currentBeatCounter = 0;
            SetRandomInterval(); 
        }
    }

    // ----------------------------------------------------
    // Lógica de instanciación
    // ----------------------------------------------------
    public void SpawnGhost() 
    {
        if (ghostPrefab == null)
        {
            Debug.LogError("GhostSpawner: ¡El Prefab del Fantasma no está asignado!");
            return;
        }

        Vector3 spawnPosition = transform.position + (Vector3.up * initialVerticalOffset);
        Quaternion spawnRotation = Quaternion.Euler(0f, spawnRotationY, 0f);

        GameObject newGhostGO = Instantiate(ghostPrefab, spawnPosition, spawnRotation);

        GhostMovement ghostMover = newGhostGO.GetComponent<GhostMovement>();
        if (ghostMover != null)
        {
            // ASIGNAR DESPLAZAMIENTO LATERAL ALEATORIO (-1, 0, o 1 carril)
            // Random.Range(-1, 2) genera -1, 0 o 1
            float randomLaneOffset = Random.Range(-1, 2) * laneDistance;
            ghostMover.lateralOffset = randomLaneOffset;
            
            ghostMover.EmergeAndMove();
        }
        else
        {
            Debug.LogError("El Prefab del Fantasma no tiene el script GhostMovement.");
            Destroy(newGhostGO);
        }
    }
    
    // Opcional: El método OnDrawGizmos se mantiene igual para visualización en el editor.
}