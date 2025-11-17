using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    void Start()
    {
        // 1. Mostrar el cursor (Cursor.visible = true)
        Cursor.visible = true; 
        
        // 2. Liberar el cursor para que pueda moverse por la pantalla (CursorLockMode.None)
        Cursor.lockState = CursorLockMode.None;
        
        // Opcional: Reanudar Time.timeScale a 1f si no lo hiciste antes de cargar la escena.
        // if (Time.timeScale == 0f) 
        // {
        //     Time.timeScale = 1f;
        // }
    }
}