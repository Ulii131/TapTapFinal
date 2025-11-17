using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BeatIndicator : MonoBehaviour

{

    [Header("Referencias UI")]

    [Tooltip("La imagen del Canvas que acta como indicador del beat.")]

    public Image beatImage;



    [Header("Configuracin de Color")]

    [Tooltip("Color que se muestra cuando el beat golpea (ej. Rojo Brillante).")]

    public Color beatColor = Color.red;

   

    [Tooltip("Color normal (ej. Blanco o Transparente).")]

    public Color defaultColor = Color.white;



    [Header("Ritmo y Duracin")]

    [Tooltip("Cada cuntos beats debe parpadear el indicador.")]

    public int beatsPerFlash = 1;

    [Tooltip("La duracin del parpadeo (qu tan rpido vuelve al color por defecto).")]

    public float flashDuration = 0.1f;



    private int beatCounter = 0;

   

    void Start()

    {

        if (beatImage == null)

        {

            // Si la imagen no est asignada, intentar buscarla en este mismo GameObject

            beatImage = GetComponent<Image>();

            if (beatImage == null)

            {

                Debug.LogError("BeatIndicator requiere un componente Image en el Canvas.");

                enabled = false;

                return;

            }

        }

       

        // Inicializar con el color por defecto

        beatImage.color = defaultColor;



        // Suscribirse al evento maestro de ritmo

        // Asumiendo que RhythmManager.OnBeat es un evento esttico global.

        RhythmManager.OnBeat += FlashIndicator;

    }



    void OnDestroy()

    {

        // Desuscribirse al destruir

        RhythmManager.OnBeat -= FlashIndicator;

    }



    private void FlashIndicator()

    {

        beatCounter++;



        // Verificar si es el momento de parpadear

        if (beatCounter >= beatsPerFlash)

        {

            // Iniciar la coroutine de parpadeo (para que no bloquee el juego)

            StartCoroutine(DoFlash());

            beatCounter = 0;

        }

    }

   

    private IEnumerator DoFlash()

    {

        // 1. Cambiar al color del beat instantneamente

        beatImage.color = beatColor;



        // 2. Esperar la duracin del parpadeo

        yield return new WaitForSeconds(flashDuration);



        // 3. Volver al color por defecto

        beatImage.color = defaultColor;

    }

}