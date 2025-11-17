using System.Collections;
using UnityEngine;

public class HandRaiser : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public GameObject leftShield; // Escudo que aparece al levantar la mano izquierda

    public float raiseHeight = 0.2f;
    public float raiseDuration = 0.1f;
    public float returnDuration = 0.15f;

    private Vector3 leftInitialPos;
    private Vector3 rightInitialPos;

    private RhythmManager rhythmManager; // <-- CAMBIADO

    void Start()
    {
        if (leftHand == null || rightHand == null)
        {
            Debug.LogError("No se asignaron las manos en el Inspector.");
            return;
        }

        rhythmManager = FindObjectOfType<RhythmManager>(); // <-- CAMBIADO

        leftInitialPos = leftHand.localPosition;
        rightInitialPos = rightHand.localPosition;

        if (leftShield != null)
        {
            leftShield.SetActive(false); // Desactivamos el escudo al inicio
        }
    }

    void Update()
    {
        // ----------------------------------------------------
        // VERIFICACIÓN CENTRAL DEL BEAT:
        if (rhythmManager == null || !rhythmManager.IsTimeToMove()) 
            return;
        // ----------------------------------------------------
        
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RaiseAndReturn(leftHand, leftInitialPos, true));
        }

        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(RaiseAndReturn(rightHand, rightInitialPos, false));
        }
    }

    // Coroutine se mantiene igual
    IEnumerator RaiseAndReturn(Transform hand, Vector3 startPos, bool isLeftHand)
    {
        Vector3 raisedPos = startPos + new Vector3(0f, raiseHeight, 0f);
        float elapsed = 0f;

        // Activar escudo si es la mano izquierda
        if (isLeftHand && leftShield != null)
        {
            leftShield.SetActive(true);
            Debug.Log("🛡 Escudo izquierdo ACTIVADO");
        }

        // Subir
        while (elapsed < raiseDuration)
        {
            hand.localPosition = Vector3.Lerp(startPos, raisedPos,
                elapsed / raiseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hand.localPosition = raisedPos;
        
        // Bajar
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            hand.localPosition = Vector3.Lerp(raisedPos, startPos,
                elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hand.localPosition = startPos;

        // Desactivar escudo
        if (isLeftHand && leftShield != null)
        {
            leftShield.SetActive(false);
            Debug.Log("🛡 Escudo izquierdo DESACTIVADO");
        }
    }
}