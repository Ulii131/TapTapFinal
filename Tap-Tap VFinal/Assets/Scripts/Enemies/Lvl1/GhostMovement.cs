using System.Collections;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public Transform[] positions; // Lugares donde se moverá el fantasma
    public float moveDuration = 0.25f; // Para movimiento suave (si fadeTeleport es falso)
    public bool fadeTeleport = true; // Si querés efecto de desvanecer
    public float fadeDuration = 0.15f; // Duración del desvanecimiento/aparición

    // Eliminamos la referencia a BeatIndcator

    private int currentIndex = 0;
    private Renderer rend;
    private Material mat;
    private bool isMoving = false;
    
    // Nueva Referencia al RhythmManager (Aunque no se usa directamente)
    // private RhythmManager rhythmManager; 

    void Start()
    {
        if (positions == null || positions.Length == 0)
        {
            Debug.LogWarning($"{name}: No hay posiciones asignadas.");
            enabled = false;
            return;
        }

        // Ya no necesitamos buscar el BeatIndcator
        // rhythmManager = FindObjectOfType<RhythmManager>(); 

        // 1. Obtener el material para el efecto fade
        rend = GetComponent<Renderer>();
        // Importante: Si el material es compartido, necesitas instanciarlo para que solo afecte a este fantasma
        mat = rend.material; 
        
        transform.position = positions[currentIndex].position;
        
        // 2. Suscribirse al evento de beat
        RhythmManager.OnBeat += MoveNextPositionOnBeat;

        // NOTA: Eliminamos la coroutine MoveOnBeat() ya que el evento hace su trabajo.
    }

    void OnDestroy()
    {
        // 3. Desuscribirse al destruir para evitar errores
        RhythmManager.OnBeat -= MoveNextPositionOnBeat;
    }

    // Este método se llama *exactamente* en cada beat gracias a la suscripción
    void MoveNextPositionOnBeat()
    {
        // Solo permitir el movimiento si no está ya en curso
        if (!isMoving)
        {
            StartCoroutine(MoveToNextPosition());
        }
    }
    
    // ------------------- COROUTINES DE MOVIMIENTO Y FADE -------------------

    IEnumerator MoveToNextPosition()
    {
        isMoving = true;
        int nextIndex = (currentIndex + 1) % positions.Length;
        Vector3 startPos = transform.position;
        Vector3 endPos = positions[nextIndex].position;

        if (fadeTeleport)
        {
            // Desvanecer (1 a 0)
            yield return StartCoroutine(Fade(1, 0));
            
            // Teletransporte
            transform.position = endPos;
            
            // Aparecer (0 a 1)
            yield return StartCoroutine(Fade(0, 1));
        }
        else
        {
            // Movimiento suave (Lerp)
            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
                yield return null;
            }
            transform.position = endPos;
        }

        currentIndex = nextIndex;
        isMoving = false;
    }

    IEnumerator Fade(float from, float to)
    {
        // Asegurar que el material tiene la propiedad _Color
        if (!mat.HasProperty("_Color"))
        {
            // Si no tiene _Color, intenta usar _BaseColor (para URP/HDRP)
            if (!mat.HasProperty("_BaseColor"))
                yield break;
        }

        Color c = mat.color;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            
            // Asignar el nuevo color con transparencia
            mat.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        // Asegurar el valor final
        mat.color = new Color(c.r, c.g, c.b, to);
    }
    
    // NOTA: ELIMINAMOS LA COROUTINE MoveOnBeat() ORIGINAL
}