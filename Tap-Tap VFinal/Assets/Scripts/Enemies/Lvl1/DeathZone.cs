using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{

    public string gameOverSceneName = "GameOverScene";

    public string playerTag = "Player";

    /// <summary>
    /// Se llama cuando un Collider entra en el trigger de este objeto.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 1. Verificar si el objeto que cayó tiene la etiqueta de Jugador
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"El jugador ({other.gameObject.name}) ha caído en la zona de muerte. Activando Game Over.");
            
            // 2. Ejecutar la acción de Game Over
            TriggerGameOver();
        }
    }

    /// <summary>
    /// Carga la escena de Game Over.
    /// </summary>
    private void TriggerGameOver()
    {
        // NOTA: Para que esto funcione, la escena "GameOverScene" debe estar
        // añadida a la configuración de Build Settings (File > Build Settings).
        try
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar la escena de Game Over: '{gameOverSceneName}'. Asegúrate de que la escena exista y esté añadida a Build Settings. Error: {e.Message}");
        }
    }
}