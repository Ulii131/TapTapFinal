using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BPMErrorFeedback : MonoBehaviour
{
    public Image screenOverlay;          // Imagen roja transparente
    public Image bpmIndicator;           // El indicador de ritmo
    public float flashDuration = 0.2f;   // Tiempo de parpadeo
    private bool isFlashing = false;

    private Color transparentRed = new Color(1f, 0f, 0f, 0f);
    private Color visibleRed = new Color(1f, 0f, 0f, 0.4f);

    public void TriggerFlash()
    {
        if (!isFlashing)
            StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        isFlashing = true;
        screenOverlay.color = visibleRed;
        yield return new WaitForSeconds(flashDuration);
        screenOverlay.color = transparentRed;
        isFlashing = false;
    }

    // Método que podés llamar desde el código de movimiento
    public bool IsOnBeat()
    {
        return bpmIndicator.color == Color.green;
    }
}
