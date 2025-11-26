using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public float maxScale = 1.5f;
    public float scaleDuration = 0.12f; // tiempo de escalado rápido
    public float lifeTime = 0.20f;      // tiempo total visible (incluye fade)
    public float offsetForward = 0.5f;  // distancia delante del enemigo
    public bool fadeOut = true;

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Play usando el transform del enemigo (coloca delante en su forward)
    public void Play(Transform enemyTransform, Vector3 contactPoint)
    {
        StopAllCoroutines();

        // Posicionar delante del enemigo si enemyTransform no es null,
        // sino usar el contactPoint recibido
        if (enemyTransform != null)
            transform.position = enemyTransform.position + enemyTransform.forward * offsetForward;
        else
            transform.position = contactPoint;

        // Asegurar que mire a la cámara (billboard)
        if (Camera.main != null)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position);

        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        // Instanciar material para no modificar el sharedMaterial
        if (rend != null)
            rend.material = new Material(rend.material);

        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        float t = 0f;
        // escalar rápido
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / scaleDuration);
            transform.localScale = Vector3.one * (p * maxScale);
            yield return null;
        }
        transform.localScale = Vector3.one * maxScale;

        // vida + fade
        float elapsed = 0f;
        Color baseColor = Color.white;
        if (rend != null) baseColor = rend.material.color;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;
            if (fadeOut && rend != null)
            {
                float a = Mathf.Lerp(1f, 0f, elapsed / lifeTime);
                Color c = baseColor;
                c.a = a;
                rend.material.color = c;
            }
            yield return null;
        }

        // Reset alpha por si se reutiliza
        if (rend != null)
        {
            Color c = rend.material.color;
            c.a = 1f;
            rend.material.color = c;
        }

        // Devolver al pool
        if (EffectPool.Instance != null)
            EffectPool.Instance.Return(this);
        else
            gameObject.SetActive(false);
    }
}
