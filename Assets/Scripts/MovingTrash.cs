using UnityEngine;

public class MovingTrash : MonoBehaviour
{
    public float speed = 2f;
    public float changeDirectionTime = 2f;
    private Vector3 movementDirection;

    void Start()
    {
        InvokeRepeating("ChangeDirection", 0, changeDirectionTime);
    }

    void Update()
    {
        transform.Translate(movementDirection * speed * Time.deltaTime);
    }

    void ChangeDirection()
    {
        movementDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}