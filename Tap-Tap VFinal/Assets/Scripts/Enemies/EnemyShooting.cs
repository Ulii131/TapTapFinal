using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public Transform playerPosition; // Se mantiene por si necesitas rotar el enemigo, pero ya no se usa para el cálculo de la bala.
    
    private int beatCount = 0;
    private const int beatsPerShot = 6;

    void Start()
    {
        if (bulletPrefab == null || spawnPoint == null)
        {
            Debug.LogError("¡Falta asignar una referencia en el Inspector de EnemyShooting!");
            return;
        }

        RhythmManager.OnBeat += CheckAndShoot; 
    }

    void OnDestroy()
    {
        RhythmManager.OnBeat -= CheckAndShoot;
    }

    void CheckAndShoot()
    {
        beatCount++;

        if (beatCount >= beatsPerShot)
        {
            Shoot();
            beatCount = 0;
        }
    }
 
    void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab,
            spawnPoint.position, spawnPoint.rotation);
        
        Bullet bullet = newBullet.GetComponent<Bullet>();
        if (bullet != null)
        {
            // --- CAMBIO CLAVE AQUÍ ---
            // Usar la dirección frontal (eje Z local) del punto de spawn.
            Vector3 forwardDirection = spawnPoint.forward;
            
            // Inicializar la bala con esa dirección fija.
            bullet.Initialize(forwardDirection); 
        }
        
        // La bala hereda la rotación del spawnPoint (mirando al frente del enemigo).
    }
}