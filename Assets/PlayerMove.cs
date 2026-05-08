using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    private PlayerInputActions inputActions;
    [SerializeField, Header("スプライトレンダラー")] private SpriteRenderer renderer;
    private Vector2 moveInput;
    public float speed = 5f;
    public float JumpForce = 5f;
    public float bounceForce = 8f;
    private bool isGrounded = true;
    private Rigidbody2D rb;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
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
        var vec_x = rb.linearVelocityX;
        if (moveInput.x == 0)
        {
            if (vec_x > 0)
            {
                renderer.flipX = false;
            }
            else if (vec_x < 0)
            {
                renderer.flipX = true;
            }
        }
        else
        {
            if (moveInput.x > 0)
            {
                renderer.flipX = false;
            }
            else if (moveInput.x < 0)
            {
                renderer.flipX = true;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach(ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    Bounce();
                    break;
                }
            }

        }
    }

    private void Bounce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
    }
    private void Jump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }
}

