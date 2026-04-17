using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    public float speed = 5f;
    public float JumpForce = 5f;

    private bool isGrounded = true;
    private Rigidbody rb;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

        // 移動
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // ジャンプ
        inputActions.Player.Jump.performed += ctx => Jump();

    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        // 左右移動（X方向のみ）
        transform.Translate(new Vector3(moveInput.x, 0, 0) * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            isGrounded = true;
        }
    }

    private void Jump()
    {
        if (!isGrounded) return;

        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        isGrounded = false;
    }
}


