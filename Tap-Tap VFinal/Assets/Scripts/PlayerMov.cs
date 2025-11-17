using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public float moveDistance = 1f; // Tamaño de cada cuadrícula
    public float moveSpeed = 5f;    // Velocidad de movimiento (suavizado)
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.forward;
            else if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.back;
            else if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
            else if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

            if (direction != Vector3.zero)
            {
                targetPosition += direction * moveDistance;
                StartCoroutine(MoveToPosition(targetPosition));
            }
        }
    }

    System.Collections.IEnumerator MoveToPosition(Vector3 destination)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
    }
}
