using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDeflector : MonoBehaviour
{
    [Header("Configuraci�n")]
    [SerializeField] private float deflectionRadius = 5f; 
    [SerializeField] private LayerMask bulletLayer;

    private RhythmManager rhythmManager;

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager == null) Debug.LogError("BulletDeflector: No se encontró RhythmManager.");
    }

    private void Update()
    {
        // Usar el RhythmManager para validar si el click es 'On Beat'
        if (Input.GetMouseButtonDown(1) && rhythmManager != null && rhythmManager.IsTimeToMove())
        {
            DeflectBullets();
        }
    }

    private void DeflectBullets()
    {
        Collider[] bullets = Physics.OverlapSphere(transform.position, deflectionRadius, bulletLayer);

        foreach (Collider bullet in bullets)
        {
            Destroy(bullet.gameObject);
            Debug.Log("Bala destruida: " + bullet.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deflectionRadius);
    }
}
