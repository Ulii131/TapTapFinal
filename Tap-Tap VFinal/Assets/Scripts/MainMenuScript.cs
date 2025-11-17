using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [Header("Configuración de Escenas")]
    [Tooltip("Nombre EXACTO de la escena del tutorial.")]
    public string tutorialSceneName = "TutorialScene"; 

    private void Start()
    {
        audioSource.Play();
    }
    
    // Función existente para iniciar un nivel
    public void EmpezarNivel(string NombreNivel)
    {
        SceneManager.LoadScene(NombreNivel);
    }
    
    // --- ¡NUEVA FUNCIÓN AÑADIDA! ---
    public void LoadTutorial()
    {
        if (string.IsNullOrEmpty(tutorialSceneName))
        {
            Debug.LogError("¡El nombre de la escena del tutorial no está configurado en MainMenuScript!");
            return;
        }
        
        // Carga la escena cuyo nombre hemos definido en el Inspector
        SceneManager.LoadScene(tutorialSceneName);
    }
    // ---------------------------------

    public void Salir()
    {
        Application.Quit();
        Debug.Log("aquí se cierra el juego");
    }

}