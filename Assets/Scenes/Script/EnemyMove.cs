using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("ˆÚ“®‹——£")]
    [SerializeField] private float moveDistance = 5f;

    private Vector3 startPosition;
    private int direction = 1;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        float distance = transform.position.x - startPosition.x;

        if (distance >= moveDistance)
        {
            direction = -1;
        }
        else if (distance <= -moveDistance)
        {
            direction = 1;
        }

    }
    public void ResetEnemy() 
    {
        transform.position = startPosition;
        direction = 1;

        gameObject.SetActive(true);
    }
    public void Defeat() 
    {
        gameObject.SetActive(false);
    }
}
