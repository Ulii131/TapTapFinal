using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionSequencer : MonoBehaviour
{
    public List<Renderer> cubes; // Lista de renderers de los cubos
    public float bpm = 110f;
    public float emissionTime = 0.2f;
    public Color emissionColor = Color.white;

    private float beatInterval;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        beatInterval = 60f / bpm;

        // Desactiva emisión en todos los cubos al iniciar
        foreach (Renderer rend in cubes)
        {
            SetEmission(rend, Color.black);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            timer -= beatInterval;

            // Enciende la emisión del cubo actual
            Renderer current = cubes[currentIndex];
            StartCoroutine(BlinkEmission(current));

            // Avanza al siguiente cubo
            currentIndex = (currentIndex + 1) % cubes.Count;
        }
    }

    IEnumerator BlinkEmission(Renderer rend)
    {
        SetEmission(rend, emissionColor);
        yield return new WaitForSeconds(emissionTime);
        SetEmission(rend, Color.black);
    }

    void SetEmission(Renderer rend, Color color)
    {
        Material mat = rend.material; // importante: usa instancia única
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color);
    }
}
