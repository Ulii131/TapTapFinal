using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public HealthBar HealthBar;
    int tiempo = 60;

    void Start()
    {
        HealthBar.SetMaxHealth(tiempo);
        StartCoroutine(Contar());
    }

    IEnumerator Contar()
    {
        while (tiempo > 0)
        {
            tiempo--;
            HealthBar.SetHealth(tiempo);
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("TimeOutScene");
    }

}
