using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class RhythmicLightSequencer : MonoBehaviour
{
    public List<Light> lights = new List<Light>(); // Lista de luces a controlar
    public float bpm = 110f;
    public float lightOnDuration = 0.2f; // cuánto tiempo se mantiene encendida cada luz
    private float beatInterval;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        beatInterval = 60f / bpm;

        // Asegurarse de que todas las luces comiencen apagadas
        foreach (Light l in lights)
            l.enabled = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Cada beat: encender siguiente luz
        if (timer >= beatInterval)
        {
            timer -= beatInterval;

            // Apagar todas las luces
            foreach (Light l in lights)
                l.enabled = false;

            // Encender la luz actual
            lights[currentIndex].enabled = true;

            // Programar apagarla tras cierta duración
            StartCoroutine(TurnOffLightAfter(lights[currentIndex], lightOnDuration));

            // Avanzar al siguiente índice
            currentIndex = (currentIndex + 1) % lights.Count;
        }
    }

    System.Collections.IEnumerator TurnOffLightAfter(Light light, float duration)
    {
        yield return new WaitForSeconds(duration);
        light.enabled = false;
    }
}
