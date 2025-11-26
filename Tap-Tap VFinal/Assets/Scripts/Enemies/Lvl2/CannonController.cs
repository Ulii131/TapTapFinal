using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour
{
    // Referencias
    public GameObject barrelPrefab;
    public Transform playerTarget; 
    
    // Configuración Rítmica
    public int fireIntervalBeats = 4;
    private int currentBeatCounter = 0;

    // Configuración Parabólica
    public float maxArcHeight = 10f; 
    public float timeToImpact = 1.5f;

    // Configuración de Objetivo
    public float anticipationOffsetZ = 5f; 
    
    // --- INICIO CÓDIGO DE SONIDO ---

    public AudioClip cannonFireSound;

    public float soundVolume = 1.0f;
    // --- FIN CÓDIGO DE SONIDO ---

    void Awake()
    {
        // Suscribirse al evento de ritmo
        RhythmManager.OnBeat += CheckAndFire; 
    }

    void OnDestroy()
    {
        RhythmManager.OnBeat -= CheckAndFire;
    }

    private void CheckAndFire()
    {
        currentBeatCounter++;
        if (currentBeatCounter >= fireIntervalBeats)
        {
            FireBarrel();
            currentBeatCounter = 0;
        }
    }

    private void FireBarrel()
    {
        if (barrelPrefab == null || playerTarget == null)
        {
            Debug.LogError("CannonController: ¡Faltan referencias!");
            return;
        }
        
        // --- DISPARAR SONIDO DEL CAÑÓN (antes de la lógica de instanciación) ---
        if (cannonFireSound != null)
        {
            // PlayClipAtPoint es ideal para SFX de un solo uso que deben escucharse en la posición del cañón.
            AudioSource.PlayClipAtPoint(cannonFireSound, transform.position, soundVolume);
        }

        // Se calcula la posición del jugador, pero se le suma el offset para que el barril caiga delante de él.
        Vector3 playerPos = playerTarget.position;
        // El cañón dispara hacia el punto donde el jugador ESTARÁ.
        Vector3 impactTarget = new Vector3(
            playerPos.x,
            0f, // La caída siempre es a Y=0 (el suelo)
            playerPos.z + anticipationOffsetZ 
        );
        
        // INSTANCIAR EL PROYECTIL
        GameObject newBarrelGO = Instantiate(barrelPrefab, transform.position, Quaternion.identity);
        BarrelProjectile barrelScript = newBarrelGO.GetComponent<BarrelProjectile>();

        // LANZAR EL PROYECTIL
        if (barrelScript != null)
        {
            // Usar el tiempo total de impacto para que el jugador sepa cuánto dura la amenaza.
            barrelScript.Launch(impactTarget, maxArcHeight, timeToImpact);
        }
    }
}