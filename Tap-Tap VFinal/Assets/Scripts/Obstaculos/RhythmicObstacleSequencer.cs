using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmicObstacleSequencer : MonoBehaviour
{
    [Header("Cubos a animar (ordenados)")]
    public List<Transform> obstacles = new List<Transform>();
    // [Header("Configuración de ritmo")] <-- BPM ELIMINADO
    // public float bpm = 110f; 

    [Header("Movimiento vertical")]
    public float moveHeight = 2f; // cuánto sube el cubo
    public float moveSpeed = 5f; // velocidad de subida/bajada

    [Header("Duración arriba")]
    public float stayUpDuration = 0.2f;

    // private float beatInterval; <-- ELIMINADO
    private int currentIndex = 0;
    // private float timer = 0f; <-- ELIMINADO

    void Start()
    {
        if (obstacles.Count == 0)
        {
            Debug.LogWarning("No hay obstáculos asignados en RhythmicObstacleSequencer.");
            return;
        }
        // Suscribirse al evento que se dispara en cada beat
        RhythmManager.OnBeat += MoveNextObstacle;
        
        // NOTA: ELIMINAR EL CÓDIGO DE CÁLCULO DE BEAT EN START()
    }

    void OnDestroy()
    {
        RhythmManager.OnBeat -= MoveNextObstacle;
    }

    // Este método se llama *exactamente* en cada beat
    void MoveNextObstacle()
    {
        // Iniciar la coroutine para el obstáculo actual
        StartCoroutine(MoveObstacle(obstacles[currentIndex]));
        
        // Pasar al siguiente obstáculo en la lista
        currentIndex = (currentIndex + 1) % obstacles.Count;
    }

    // ELIMINAR COMPLETAMENTE EL MÉTODO Update() ORIGINAL

    // Coroutine se mantiene igual
    IEnumerator MoveObstacle(Transform obstacle)
    {
        Vector3 startPos = obstacle.position;
        Vector3 upPos = startPos + Vector3.up * moveHeight;

        // Subir
        while (Vector3.Distance(obstacle.position, upPos) > 0.01f)
        {
            obstacle.position = Vector3.MoveTowards(obstacle.position,
                upPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Esperar arriba (la duración se mantiene basada en segundos)
        yield return new WaitForSeconds(stayUpDuration);
        
        // Bajar
        while (Vector3.Distance(obstacle.position, startPos) > 0.01f)
        {
            obstacle.position = Vector3.MoveTowards(obstacle.position,
                startPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}