using UnityEngine;

public class HookMove : MonoBehaviour
{

    [Header("ÉtÉbÉNê›íË")]
    public GameObject hookPrefab;
    public float shootSpeed = 10;
    public float hookLifeime = 2;

    void Update()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector3.right;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector3.down;

        if (dir != Vector3.zero)
        {
            Shoot(dir);
        }

    }

    void Shoot(Vector3 direction) 
    {

        Vector3 spawanPos = transform.position + direction * 1.2f;
        GameObject hook = Instantiate(hookPrefab, spawanPos, Quaternion.identity);

        Rigidbody rb = hook.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direction * shootSpeed;
        }

        Destroy(hook, hookLifeime);

    }
}
