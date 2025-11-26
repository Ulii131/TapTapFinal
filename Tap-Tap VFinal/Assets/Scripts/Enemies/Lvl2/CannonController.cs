using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour
{

    public GameObject barrelPrefab;
    public Transform playerTarget; 
    
    public int fireIntervalBeats = 4;
    private int currentBeatCounter = 0;

    public float maxArcHeight = 10f; 
    public float timeToImpact = 1.5f;

    public float anticipationOffsetZ = 5f; 

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

        // Se calcula la posición del jugador, pero se le suma el offset para que el barril caiga delante de él.
        Vector3 playerPos = playerTarget.position;
        // El cañón dispara hacia el punto donde el jugador ESTARÁ.
        Vector3 impactTarget = new Vector3(
            playerPos.x,
            0f, // La caída siempre es a Y=0 (el suelo)
            playerPos.z + anticipationOffsetZ 
        );
        
        //  INSTANCIAR EL PROYECTIL
        GameObject newBarrelGO = Instantiate(barrelPrefab, transform.position, Quaternion.identity);
        BarrelProjectile barrelScript = newBarrelGO.GetComponent<BarrelProjectile>();

        //  LANZAR EL PROYECTIL
        if (barrelScript != null)
        {
            // Usar el tiempo total de impacto para que el jugador sepa cuánto dura la amenaza.
            barrelScript.Launch(impactTarget, maxArcHeight, timeToImpact);
        }
    }
}