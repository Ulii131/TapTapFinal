using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float duration = 0.1f; // El rayo solo dura 0.1 segundos

    void Start()
    {
        Destroy(gameObject, duration);
    }
}