using UnityEngine;
using System.Collections;

public class BarrelProjectile : MonoBehaviour
{
    public int damageAmount = 10; 
    
    public float blinkDuration = 0.5f; 

    public Color blinkColor = Color.red; 
    
    // Variables de Movimiento y Material
    private Vector3 startPos;
    private Vector3 targetPos;
    private float arcHeight;
    private float journeyDuration;
    private float startTime;
    
    private Renderer barrelRenderer;
    private Material barrelMaterial;
    private Color originalEmissionColor; // Almacena el color original de Emisión
    
    private bool isFlying = false;
    private bool hasDealtDamage = false; 

    void Awake()
    {
        // Obtener el Renderer y el material
        barrelRenderer = GetComponent<Renderer>();
        if (barrelRenderer != null)
        {
            // Instanciar material para evitar cambiar el material del prefab
            barrelMaterial = barrelRenderer.material; 
            
            // Intentar obtener el color de Emisión original (si el shader lo soporta)
            if (barrelMaterial.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = barrelMaterial.GetColor("_EmissionColor");
            }
            else
            {
                // Si no hay emisión, usamos el color negro como base para la emisión
                originalEmissionColor = Color.black; 
            }
        }
    }

    public void Launch(Vector3 target, float height, float duration)
    {
        startPos = transform.position;
        targetPos = target;
        arcHeight = height;
        journeyDuration = duration;
        startTime = Time.time;
        isFlying = true;

        // INICIAR EL EFECTO DE PARPADEO AL LANZAR
        StartCoroutine(BlinkEffect());
        
        // Destruir el barril después de que el viaje termine
        Destroy(gameObject, journeyDuration + 1.0f);
    }

    void Update()
    {
        if (!isFlying) return;

        // 1. Calcular el tiempo transcurrido (t es de 0.0 a 1.0)
        float timeElapsed = Time.time - startTime;
        float t = timeElapsed / journeyDuration;

        // Si ya llegó al destino, detener el movimiento
        if (t >= 1.0f)
        {
            isFlying = false;
            // No se destruye aquí, ya está programada la autodestrucción en Launch()
        }
        
        // El movimiento solo ocurre si isFlying es verdadero
        if (isFlying)
        {
            // 2. Movimiento Horizontal (Lineal)
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);

            // 3. Movimiento Vertical (Arco Parabólico)
            float currentY = 4 * arcHeight * t * (1 - t);

            // Aplicar el nuevo Y
            transform.position = new Vector3(currentPos.x, currentPos.y + currentY, currentPos.z);
        }
    }
    
    // Coroutine para el parpadeo del barril
    private IEnumerator BlinkEffect()
    {
        while (isFlying)
        {
            // Encender la Emisión
            SetBlinkState(true);
            yield return new WaitForSeconds(blinkDuration);

            // Apagar la Emisión
            SetBlinkState(false);
            yield return new WaitForSeconds(blinkDuration);
        }
        // Asegurarse de que el material vuelva a su estado original al terminar
        SetBlinkState(false); 
    }
    
    private void SetBlinkState(bool isBlinking)
    {
        if (barrelMaterial == null || !barrelMaterial.HasProperty("_EmissionColor")) return;
        
        if (isBlinking)
        {
            // Configurar color brillante (multiplicamos para intensidad HDR)
            barrelMaterial.SetColor("_EmissionColor", blinkColor * 5f); 
            barrelMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            // Devolver el color de Emisión original (apagando el brillo si era negro)
            barrelMaterial.SetColor("_EmissionColor", originalEmissionColor);
            
            // Si el color original es negro o casi, desactivamos la keyword para mejor rendimiento.
            if (originalEmissionColor == Color.black || originalEmissionColor.maxColorComponent < 0.1f) 
            {
                 barrelMaterial.DisableKeyword("_EMISSION");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. **DESTRUCCIÓN POR ESCUDO**
        if (other.CompareTag("PlayerShield")) 
        {
            Debug.Log("Barril bloqueado por el escudo. Destruyendo barril.");
            
            StopAllCoroutines(); 
            SetBlinkState(false);
            Destroy(gameObject);
            return; 
        }
        

        if (hasDealtDamage || !other.CompareTag("Player"))
        {
            return;
        }

        // Obtener el componente de salud del jugador
        SimpleHealthSystem playerHealth = other.GetComponent<SimpleHealthSystem>();

        if (playerHealth != null)
        {
            // Aplicar el daño y marcar la bandera
            playerHealth.TakeDamage(damageAmount);
            hasDealtDamage = true; 
            
            Debug.Log($"El barril impactó al jugador. Daño aplicado: {damageAmount}.");
            
            // Si golpea al jugador, se destruye.
            StopAllCoroutines(); 
            SetBlinkState(false);
            Destroy(gameObject); 
        }
    }
}