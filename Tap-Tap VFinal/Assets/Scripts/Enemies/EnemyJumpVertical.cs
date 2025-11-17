using System.Collections;
using UnityEngine;

public class EnemyJumpVertical : MonoBehaviour
{
    public float stepSize = 1f;
    public float jumpInterval = 0.5f;

    private bool goingUp = true;

    void Start()
    {
        StartCoroutine(JumpVertical());
    }

    IEnumerator JumpVertical()
    {
        while (true)
        {
            float elapsed = 0f;
            float duration = jumpInterval;

            float startY = transform.position.y;
            float targetY = startY + (goingUp ? stepSize : -stepSize);

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float newY = Mathf.Lerp(startY, targetY, t);

                // Combinar con posición actual (manteniendo X y Z)
                Vector3 currentPos = transform.position;
                transform.position = new Vector3(currentPos.x, newY, currentPos.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Asegura posición final exacta
            Vector3 finalPos = transform.position;
            transform.position = new Vector3(finalPos.x, targetY, finalPos.z);

            goingUp = !goingUp;
        }
    }
}
