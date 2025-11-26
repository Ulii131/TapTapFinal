using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Header("Animation")]
    public float maxScale = 1.5f;
    public float scaleDuration = 0.12f;
    public float lifeTime = 0.35f;
    public bool fadeOut = true;

    [Header("Render")]
    public bool useChildRenderer = true; // si el renderer está en el Quad hijo
    public int forcedRenderQueue = 4000; // opcional: asegurar render encima

    Renderer rend;
    Material runtimeMat; // copia simple para poder ajustar alpha si shader lo soporta
    string colorProp = null;

    void Awake()
    {
        rend = useChildRenderer ? GetComponentInChildren<Renderer>() : GetComponent<Renderer>();
        if (rend != null && rend.sharedMaterial != null)
        {
            runtimeMat = new Material(rend.sharedMaterial);
            rend.material = runtimeMat;
            // detectar propiedades color comunes
            if (runtimeMat.HasProperty("_Color")) colorProp = "_Color";
            else if (runtimeMat.HasProperty("_BaseColor")) colorProp = "_BaseColor";
            else colorProp = null;

            // asegurar que se renderice por encima si hace falta
            runtimeMat.renderQueue = forcedRenderQueue;
        }
    }

    // Called right after Instantiate to start the effect
    public void Play(Transform enemyTransform, Vector3 contactPoint, float offsetForward = 0.6f, float offsetUp = 0.25f)
    {
        // Posicionar delante del enemigo si lo recibimos
        Vector3 pos = contactPoint;
        if (enemyTransform != null)
            pos = enemyTransform.position + enemyTransform.forward * offsetForward + Vector3.up * offsetUp;

        transform.position = pos;

        // Billboard hacia la cámara principal si existe
        if (Camera.main != null)
        {
            Vector3 dir = Camera.main.transform.position - transform.position;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        StartCoroutine(RunRoutine());
    }

    IEnumerator RunRoutine()
    {
        // Escalado rápido
        float t = 0f;
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / scaleDuration);
            transform.localScale = Vector3.one * (p * maxScale);
            yield return null;
        }
        transform.localScale = Vector3.one * maxScale;

        // Vida y fade
        float elapsed = 0f;
        Color baseColor = Color.white;
        bool canFade = (runtimeMat != null && colorProp != null && fadeOut);
        if (canFade)
        {
            try { baseColor = runtimeMat.GetColor(colorProp); }
            catch { canFade = false; }
        }

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;
            if (canFade)
            {
                float a = Mathf.Lerp(1f, 0f, elapsed / lifeTime);
                Color c = baseColor; c.a = a;
                runtimeMat.SetColor(colorProp, c);
            }
            yield return null;
        }

        // limpiar y destruir
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (runtimeMat != null)
            Destroy(runtimeMat);
    }
}