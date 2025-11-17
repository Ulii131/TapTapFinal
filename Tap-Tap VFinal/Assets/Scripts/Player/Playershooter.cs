using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuraci�n")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    private RhythmManager rhythmManager;

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager == null) Debug.LogError("PlayerShooter: No se encontró RhythmManager.");
    }

    void Update()
    {
        // Usar el RhythmManager para validar si el click es 'On Beat'
        if (Input.GetMouseButtonDown(1) && rhythmManager != null && rhythmManager.IsTimeToMove())
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<PlayerBullet>().Initialize(firePoint.forward, bulletSpeed);
    }
}
