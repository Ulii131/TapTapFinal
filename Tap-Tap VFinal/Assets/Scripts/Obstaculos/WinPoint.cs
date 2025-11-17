using System.Collections;
using System.Collections.Generic; // Asegúrate de que esta línea esté presente si usas Listas/Arrays
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPoint : MonoBehaviour
{
    // Tiempo de espera antes de cargar la siguiente escena
    public float delayTime = 5f; 
    
    [Header("Configuración de Niveles")]
    [Tooltip("Nombra las escenas de nivel en orden (ej: SampleScene, Lvl1, Lvl2, Lvl3)")]
    public string[] levelScenes = { "SampleScene", "Lvl1", "LVL2" }; 
    
    [Header("Audio del Nivel")]
    [Tooltip("Arrastra aquí el AudioSource que reproduce la música del nivel.")]
    public AudioSource levelMusicSource; // <--- Referencia para detener la música
    
    private bool hasWon = false;

    private void OnTriggerEnter(Collider other)
    {
        // Solo reaccionar si es el jugador y no hemos ganado ya
        if (other.CompareTag("Player") && !hasWon)
        {
            hasWon = true;
            // Iniciar la secuencia de victoria con el tiempo de espera
            StartCoroutine(WinSequence(delayTime)); 
        }
    }

    private IEnumerator WinSequence(float delay)
    {
        // 1. DETENER LA MÚSICA DEL NIVEL INMEDIATAMENTE
        if (levelMusicSource != null && levelMusicSource.isPlaying)
        {
            levelMusicSource.Stop();
        }
        
        // Opcional: Pausar el juego o congelar la acción visualmente
        Time.timeScale = 0f; // Pausa completa

        // 2. Mostrar la pantalla de victoria (WinScene)
        // Cargar la escena de victoria encima de la actual.
        SceneManager.LoadScene("WinScene", LoadSceneMode.Additive); 
        
        // 3. Esperar el tiempo de retardo
        // IMPORTANTE: Usamos WaitForSecondsRealtime porque Time.timeScale es 0
        yield return new WaitForSecondsRealtime(delay); 

        // 4. Reanudar el tiempo antes de cargar la nueva escena
        Time.timeScale = 1f;

        // 5. Determinar y cargar el siguiente nivel
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentIndex = -1;

        // Buscar el índice del nivel actual en la lista
        for (int i = 0; i < levelScenes.Length; i++)
        {
            if (levelScenes[i] == currentSceneName)
            {
                currentIndex = i;
                break;
            }
        }

        if (currentIndex != -1 && currentIndex < levelScenes.Length - 1)
        {
            // Cargar el siguiente nivel
            string nextSceneName = levelScenes[currentIndex + 1];
            Debug.Log($"Cargando el siguiente nivel: {nextSceneName}");
            // Usamos LoadScene (sin modo aditivo) para reemplazar el nivel actual y la WinScene
            SceneManager.LoadScene(nextSceneName); 
        }
        else
        {
            // Si es el último nivel o no se encontró, ir al menú principal
            Debug.Log("Juego Completado o error de nivel. Cargando MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}