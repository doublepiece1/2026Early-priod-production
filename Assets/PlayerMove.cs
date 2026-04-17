using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    public float speed = 5f;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
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
        // ¶‰EˆÚ“®iX•ûŒü‚Ì‚İj
        transform.Translate(new Vector3(moveInput.x, 0, 0) * speed * Time.deltaTime);
    }
}
