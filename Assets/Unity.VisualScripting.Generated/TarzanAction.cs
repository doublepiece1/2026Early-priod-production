using UnityEngine;
using UnityEngine.InputSystem;
using Kounosuke;

public class TarzanAction : MonoBehaviour
{
    //==================================================
    // ■ グラップル設定
    //==================================================

    [Header("ワイヤー設定")]
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float ropeShotSpeed = 60f;

    //==================================================
    // ■ 移動設定
    //==================================================

    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpPower = 8f;
    [SerializeField] private LayerMask groundLayer;

    //==================================================
    // ■ コンポーネント
    //==================================================

    [Header("Components")]
    [SerializeField] private PendulumController pendulum;
    [SerializeField] private BoostController boost;
    [SerializeField] private GrappleRopeRenderer ropeRenderer;
    [SerializeField] private TrailRenderer trail;

    private Rigidbody2D rb;
    private PlayerInput input;
    private Camera mainCamera;

    //==================================================
    // ■ 入力
    //==================================================

    private Vector2 moveInput;
    private bool jumpPressed;
    private bool grapplePressed;
    private bool grappleReleased;

    //==================================================
    // ■ 状態
    //==================================================

    private enum State
    {
        Grounded,
        Airborne,
        Shooting,
        Grappling,
        Dead
    }

    private State state = State.Grounded;

    //==================================================
    // ■ グラップル情報
    //==================================================

    private Vector2 grapplePoint;
    private Vector2 shotDir;
    private float currentRopeLength;

    //==================================================
    // ■ Unity
    //==================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        mainCamera = Camera.main;

        if (trail != null)
            trail.emitting = false;
    }

    private void Update()
    {
        if (state == State.Dead)
            return;

        ReadInput();

        UpdateState();

        UpdateRopeVisual();
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Grappling:

                pendulum.Tick(moveInput);

                boost.Charge(
                    pendulum.Speed);

                break;

            case State.Grounded:

                GroundMove();

                break;
        }
    }

    //==================================================
    // ■ 入力
    //==================================================

    private void ReadInput()
    {
        moveInput =
            input.actions["Move"]
            .ReadValue<Vector2>();

        grapplePressed =
            input.actions["Atack"]
            .WasPressedThisFrame();

        grappleReleased =
            input.actions["Atack"]
            .WasReleasedThisFrame();

        jumpPressed =
            input.actions["Jump"]
            .WasPressedThisFrame();
    }

    //==================================================
    // ■ 状態更新
    //==================================================

    private void UpdateState()
    {
        switch (state)
        {
            case State.Grounded:
                UpdateGrounded();
                break;

            case State.Airborne:
                UpdateAirborne();
                break;

            case State.Shooting:
                UpdateShooting();
                break;

            case State.Grappling:
                UpdateGrappling();
                break;
        }

        CheckGroundState();
    }

    private void UpdateGrounded()
    {
        if (grapplePressed)
            StartRopeShot();

        if (jumpPressed)
            Jump();
    }

    private void UpdateAirborne()
    {
        if (grapplePressed)
            StartRopeShot();

        if (IsGrounded())
            state = State.Grounded;
    }

    private void UpdateShooting()
    {
        currentRopeLength +=
            ropeShotSpeed *
            Time.deltaTime;

        if (Vector2.Distance(
            rb.position,
            grapplePoint)
            <= currentRopeLength)
        {
            BeginGrapple();
        }

        if (grappleReleased)
            StopGrapple();
    }

    private void UpdateGrappling()
    {
        if (grappleReleased)
            StopGrapple();
    }

    private void CheckGroundState()
    {
        if (state == State.Grounded &&
            !IsGrounded())
        {
            state = State.Airborne;
        }
    }

    //==================================================
    // ■ 地上移動
    //==================================================

    private void GroundMove()
    {
        float target =
            moveInput.x *
            moveSpeed;

        rb.linearVelocity =
            new Vector2(
                Mathf.Lerp(
                    rb.linearVelocity.x,
                    target,
                    10f * Time.fixedDeltaTime),
                rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (!IsGrounded())
            return;

        rb.linearVelocity =
            new Vector2(
                rb.linearVelocity.x,
                jumpPower);

        state = State.Airborne;
    }

    //==================================================
    // ■ グラップル
    //==================================================

    private void StartRopeShot()
    {
        if (!TryFindTarget(out RaycastHit2D hit))
            return;

        grapplePoint = hit.point;

        shotDir =
            (grapplePoint -
             (Vector2)transform.position)
            .normalized;

        currentRopeLength = 0f;

        state = State.Shooting;
    }

    private void BeginGrapple()
    {
        pendulum.Begin(grapplePoint);

        state = State.Grappling;

        if (trail != null)
        {
            trail.Clear();
            //trail.emitting = true;
        }
    }

    private void StopGrapple()
    {
        ReleaseRope();

        boost.Release();
    }

    private void ReleaseRope()
    {
        pendulum.ResetPendulum();

        state =
            IsGrounded()
            ? State.Grounded
            : State.Airborne;

        if (trail != null)
        {
            //   trail.emitting = false;
        }
    }

    //==================================================
    // ■ ロープ描画
    //==================================================

    private void UpdateRopeVisual()
    {
        Vector2 start = transform.position;

        switch (state)
        {
            case State.Shooting:

                ropeRenderer.DrawShot(
                    start,
                    shotDir,
                    currentRopeLength);

                break;

            case State.Grappling:

                ropeRenderer.DrawConnected(
                    start,
                    grapplePoint);

                break;

            default:

                ropeRenderer.Hide();

                break;
        }
    }

    //==================================================
    // ■ Utility
    //==================================================

    private bool TryFindTarget(
        out RaycastHit2D hit)
    {
        Vector3 mouse =
            mainCamera.ScreenToWorldPoint(
                input.actions["Look"]
                .ReadValue<Vector2>());

        Vector2 dir =
            (mouse - transform.position)
            .normalized;

        hit = Physics2D.Raycast(
            transform.position,
            dir,
            maxDistance,
            grappleLayer);

        return hit.collider != null;
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(
            rb.position,
            Vector2.down,
            0.4f,
            groundLayer);
    }

    //==================================================
    // ■ 死亡
    //==================================================

    public void Die()
    {
        if (state == State.Dead)
            return;

        if (boost.IsInvincible)
            return;

        ReleaseRope();

        state = State.Dead;

        rb.linearVelocity = Vector2.zero;
    }
}