using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.1f;
    public float magnitude = 0.1f;

    [Header("BPM Bounce Settings")]
    public float bpm = 120f;
    public float amplitude = 0.05f;
    public float smoothSpeed = 5f;

    private Vector3 originalPos;
    private Vector3 bounceStartPos;
    private Vector3 bounceTargetPos;

    private float beatInterval;
    private float bounceTimer = 0f;
    private bool bounceUp = true;

    private bool isShaking = false;

    void Awake()
    {
        originalPos = transform.localPosition;
        bounceStartPos = originalPos;
        beatInterval = 60f / bpm;
        bounceTargetPos = bounceStartPos;
    }

    void Update()
    {
        // BPM bounce solo si no está temblando
        if (!isShaking)
        {
            bounceTimer += Time.deltaTime;

            if (bounceTimer >= beatInterval)
            {
                bounceTimer = 0f;
                bounceUp = !bounceUp;

                float offset = bounceUp ? amplitude : -amplitude;
                bounceTargetPos = bounceStartPos + new Vector3(0f, offset, 0f);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, bounceTargetPos, Time.deltaTime * smoothSpeed);
        }
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = bounceTargetPos; // Volver al punto del ritmo, no al centro
        isShaking = false;
    }
}
