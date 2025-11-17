using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PatternSequencer : MonoBehaviour
{
    [Header("Referencias")]
    // [ELIMINAR ESTE LISTA PÚBLICA, YA NO ES NECESARIO ASIGNARLA]
    // public List<GridTile> allTiles;         
    
    // [CAMBIAR A PRIVADA Y DEJARLA VACÍA, LA LLENAREMOS EN START]
    private List<GridTile> allTiles = new List<GridTile>(); 
    
    public Transform playerTransform;        
    public GameObject barrelSpawner;          

    [Header("Configuración del Puzzle")]
    public int patternLength = 4;           
    public int beatsPerPattern = 8;         
    
    private List<Vector3> correctPositions = new List<Vector3>();
    private int beatCounter = 0;
    
    void Start()
    {
        // ----------------------------------------------------
        // --- CÓDIGO CLAVE PARA ENCONTRAR TODOS LOS TILES ---
        // Encuentra todos los GameObjects en la escena que tienen el script GridTile.cs
        allTiles.AddRange(FindObjectsOfType<GridTile>());
        // ----------------------------------------------------

        if (allTiles.Count == 0 || playerTransform == null || barrelSpawner == null)
        {
            Debug.LogError("¡Faltan referencias clave en PatternSequencer! (Tiles: " + allTiles.Count + ")");
            enabled = false;
            return;
        }
        
        // El spawner de barriles debe estar inactivo al inicio
        barrelSpawner.SetActive(false); 
        
        RhythmManager.OnBeat += CheckAndAdvancePattern;
    }

    void OnDestroy()
    {
        RhythmManager.OnBeat -= CheckAndAdvancePattern;
    }

    private void CheckAndAdvancePattern()
    {
        beatCounter++;

        if (beatCounter >= beatsPerPattern)
        {
            // 1. EVALUAR EL PATRÓN ANTERIOR
            EvaluatePlayerPosition();

            // 2. GENERAR EL NUEVO PATRÓN
            GenerateNewPattern();
            
            beatCounter = 0;
        }
    }

    private void GenerateNewPattern()
    {
        // Limpiar patrones anteriores y listas
        foreach (var tile in allTiles)
        {
            tile.SetPattern(false);
        }
        correctPositions.Clear();
        
        // Generar un nuevo patrón aleatorio
        List<GridTile> availableTiles = new List<GridTile>(allTiles);
        for (int i = 0; i < patternLength; i++)
        {
            if (availableTiles.Count == 0) break;

            int randomIndex = Random.Range(0, availableTiles.Count);
            GridTile selectedTile = availableTiles[randomIndex];
            
            selectedTile.SetPattern(true);
            correctPositions.Add(selectedTile.transform.position);
            
            // Eliminar la loseta seleccionada para evitar duplicados en el mismo patrón
            availableTiles.RemoveAt(randomIndex);
        }
    }

    private void EvaluatePlayerPosition()
    {
        // Obtener la posición actual del jugador, redondeada a la cuadrícula más cercana
        Vector3 playerPos = playerTransform.position;
        Vector3 roundedPlayerPos = new Vector3(
            Mathf.Round(playerPos.x),
            0, // Asumimos que el suelo está en y=0
            Mathf.Round(playerPos.z)
        );

        // Verificar si la posición del jugador está en la lista de posiciones correctas
        bool isCorrect = correctPositions.Any(p => 
            Mathf.Abs(p.x - roundedPlayerPos.x) < 0.1f && 
            Mathf.Abs(p.z - roundedPlayerPos.z) < 0.1f);

        if (isCorrect)
        {
            Debug.Log("¡PATRÓN CORRECTO! Castigo desactivado.");
            // Si es correcto, desactivar el spawner de barriles
            barrelSpawner.SetActive(false);
        }
        else
        {
            Debug.Log("¡PATRÓN INCORRECTO! Castigo activado.");
            // Si es incorrecto, activar el spawner de barriles
            barrelSpawner.SetActive(true);
            
            // Opcional: Podrías añadir un castigo temporal con un timer
            // StartCoroutine(ActivatePenaltyForSeconds(3f));
        }
    }
    
    // Podrías necesitar un método en PlayerMove para obtener su posición final de movimiento.
}