using UnityEngine;

public class PlayerRespon : MonoBehaviour
{
    private Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("aaa");
        
        if (collision.gameObject.CompareTag("DeadZone"))
        {
            transform.position = startPosition;

            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}
