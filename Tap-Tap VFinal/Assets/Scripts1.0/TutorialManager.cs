using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Necesario para la gestin de escenas

public class TutorialManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("El componente Image que mostrará las páginas del tutorial.")]
    public Image tutorialImage;
    public Button nextButton;
    public Button previousButton;
    public Button exitButton; // ¡NUEVA REFERENCIA PARA EL BOTN X!

    [Header("Contenido del Tutorial")]
    [Tooltip("Arrastra aquí todas tus imágenes PNG importadas como Sprites, en orden.")]
    public List<Sprite> tutorialPages; 
    
    [Header("Configuración de Escenas")]
    [Tooltip("Nombre EXACTO de la escena del Menú Principal (Ej: MainMenu).")]
    public string mainMenuSceneName = "MainMenu"; // Ajusta el nombre si es diferente

    private int currentPageIndex = 0;

    void Start()
    {
        if (tutorialPages.Count == 0 || tutorialImage == null || nextButton == null || previousButton == null || exitButton == null)
        {
            Debug.LogError("TutorialManager: ¡Asegúrate de asignar todas las referencias (Imágenes, Image UI, Botones y el botón de Salida) en el Inspector!");
            return;
        }

        // Asignar los métodos a los eventos de click de los botones
        nextButton.onClick.AddListener(ShowNextPage);
        previousButton.onClick.AddListener(ShowPreviousPage);
        
        // Asignar el nuevo método al botón de salida
        exitButton.onClick.AddListener(ExitTutorial); 

        // Mostrar la primera página al iniciar
        UpdateUI();
    }

    /// <summary>
    /// Avanza a la siguiente página del tutorial.
    /// </summary>
    public void ShowNextPage()
    {
        if (currentPageIndex < tutorialPages.Count - 1)
        {
            currentPageIndex++;
            UpdateUI();
        }
    }

    /// <summary>
    /// Vuelve a la página anterior del tutorial.
    /// </summary>
    public void ShowPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdateUI();
        }
    }

    /// <summary>
    /// Carga la escena del menú principal.
    /// </summary>
    public void ExitTutorial()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("¡El nombre de la escena del Menú Principal no está configurado!");
            return;
        }
        
        // Aseguramos que el tiempo no esté detenido por si vienes de un nivel pausado
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    /// <summary>
    /// Actualiza la imagen mostrada y el estado de los botones.
    /// </summary>
    private void UpdateUI()
    {
        // 1. Mostrar la imagen de la página actual
        tutorialImage.sprite = tutorialPages[currentPageIndex];

        // 2. Controlar la interactividad de los botones
        previousButton.interactable = currentPageIndex > 0;
        nextButton.interactable = currentPageIndex < tutorialPages.Count - 1;
    }
}