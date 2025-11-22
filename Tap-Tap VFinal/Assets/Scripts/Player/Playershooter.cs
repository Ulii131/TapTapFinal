using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuraci�n")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private Animator anim_Protagonista;
    [SerializeField] private GameObject shield;
    Vector3 posInicialShield;

    private RhythmManager rhythmManager;

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager == null) Debug.LogError("PlayerShooter: No se encontró RhythmManager.");

        //Guardamos la posición inicial del escudo
        posInicialShield = shield.transform.localPosition;
    }

    void Update()
    {
        // Usar el RhythmManager para validar si el click es 'On Beat'
        if (Input.GetMouseButtonDown(1) && rhythmManager != null && rhythmManager.IsTimeToMove())
        {
            anim_Protagonista.Play("Attack_Protagonista");
            Shoot();

        }

        if (Input.GetMouseButtonUp(0) && rhythmManager != null && rhythmManager.IsTimeToMove())
        {
            anim_Protagonista.Play("Block_Protagonista");
            StartCoroutine(ShieldActive(0.3f));
        }
    }

    void Shoot()
    {

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<PlayerBullet>().Initialize(firePoint.forward, bulletSpeed);


    }

    IEnumerator ShieldActive(float seconds)
    {
        shield.SetActive(true);

        float initialSeconds = 0f;
        Vector3 moveUpShield = new Vector3(0, 17, 0);

        while (initialSeconds <= seconds)
        {
            shield.transform.localPosition = Vector3.Lerp(posInicialShield, posInicialShield + moveUpShield, initialSeconds / seconds);

            initialSeconds += Time.deltaTime;
            yield return null;
        }

        shield.transform.localPosition = posInicialShield;

        shield.SetActive(false);
    }
}