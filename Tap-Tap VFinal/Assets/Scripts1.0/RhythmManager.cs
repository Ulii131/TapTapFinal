using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class RhythmManager : MonoBehaviour
{
    // Evento estático al que todos los scripts del juego se suscribirán
    public static event Action OnBeat; 

    [Header("Configuración de Música")]
    public AudioSource musicSource; 
    [SerializeField] private float bpm = 110f; // Beats por minuto de la canción
    
    [Header("Ajustes de Sincronización")]
    [Range(0.01f, 0.5f)] // Permite ajustar el valor de 10ms a la mitad de un beat
    [Tooltip("El margen de tiempo (en segundos) antes y después de un beat donde la acción es válida.")]
    [SerializeField] private double timingWindow = 0.15; // ¡ESTE ES EL VALOR CLAVE A AJUSTAR!
    
    [Tooltip("Tiempo de anticipación antes del primer beat para empezar a disparar eventos.")]
    [SerializeField] private float startOffset = 0f; 

    private float beatInterval; // Tiempo exacto entre beats (60/BPM)
    private double nextBeatTime; // Tiempo dsp (audio clock) del siguiente beat

    void Start()
    {
        if (musicSource == null)
        {
            Debug.LogError("RhythmManager: ¡Falta asignar la fuente de audio! Asigna una AudioSource con la música del nivel.");
            return;
        }

        // 1. Cálculo del intervalo de beat
        beatInterval = 60f / bpm;
        
        // 2. Establecer el tiempo del primer beat
        // Se añade un offset si la canción no empieza justo en el beat 1
        nextBeatTime = AudioSettings.dspTime + beatInterval - startOffset; 
        
        // Iniciar la música
        musicSource.Play();
    }

    void Update()
    {
        // 3. Comprobar si el reloj de audio ha superado el tiempo del próximo beat
        if (AudioSettings.dspTime >= nextBeatTime)
        {
            // Disparar el evento para notificar a todos
            OnBeat?.Invoke(); 
            
            // Recalcular el próximo beat sumando el intervalo
            nextBeatTime += beatInterval;
        }
    }

    // Método público que PlayerMove, PlayerShooter, etc., llamarán para verificar el input
    public bool IsTimeToMove()
    {
        // Calcula el tiempo restante hasta el próximo beat
        double timeUntilNextBeat = nextBeatTime - AudioSettings.dspTime;
        
        // Calcula el tiempo que ha pasado desde el beat anterior
        double timeSinceLastBeat = beatInterval - timeUntilNextBeat;

        // Se verifica si el tiempo restante al beat SIGUIENTE es menor a la ventana
        bool isNearNext = timeUntilNextBeat <= timingWindow;
        
        // Se verifica si el tiempo transcurrido desde el beat ANTERIOR es menor a la ventana
        bool isNearPrevious = timeSinceLastBeat <= timingWindow;

        // Devuelve true si estamos dentro del margen de tolerancia antes O después del beat
        return isNearPrevious || isNearNext;
    }
}