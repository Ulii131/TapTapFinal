using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBPMBobbing : MonoBehaviour
{
    public float bpm = 120f;
    public float amplitude = 0.05f;      // Cuánto suben y bajan las manos
    public float smoothSpeed = 5f;       // Qué tan suave se mueve
    
    private float beatInterval;
    private float timer = 0f;
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool up = true;

    void Start()
    {
        startPos = transform.localPosition;
        beatInterval = 60f / bpm;
        targetPos = startPos;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            timer = 0f;
            up = !up;

            float offset = up ? amplitude : -amplitude;
            targetPos = startPos + new Vector3(0, offset, 0);
        }

        // Interpolación suave entre posiciones
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothSpeed);
    }
}
