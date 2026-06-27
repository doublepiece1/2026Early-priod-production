using UnityEngine;

public class AddforceTest : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    void Start()
    {
        rb.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
    }

    void Update()
    {
        
    }
}
