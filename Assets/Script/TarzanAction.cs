using Kounosuke;
using System.Collections;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class TarzanAction : GimmickBase
{
    //==================================================
    // ■ グラップル設定
    //==================================================

    [Header("ワイヤー設定")]
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float ropeShotSpeed = 60f;
    [SerializeField] private float coolTime = 0.5f;
    private float grappleCooldownTimer;
    

    [Header("Gamepad Aim Assist")]
    [SerializeField] private float aimAssistRadius = 0.5f;
    [SerializeField] private float minStickInput = 0.2f;

    //==================================================
    // ■ 移動設定
    //==================================================

    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpPower = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float knockBackX = 8f;
    [SerializeField] private float knockBackY = 5f;

    //==================================================
    // ■ コンポーネント
    //==================================================

    [Header("Components")]
    [SerializeField] private PendulumController pendulum;
    [SerializeField] private BoostController boost;
    [SerializeField] private GrappleRopeRenderer ropeRenderer;

    private Rigidbody2D rb;
    private PlayerInput input;
    private Camera mainCamera;

    //==================================================
    //  ■ SE
    //==================================================

    [SerializeField] private AudioClip grappleSE;
    [SerializeField] private AudioClip jumpSE;

    //==================================================
    // ■ 入力
    //==================================================

    private Vector2 moveInput;
    private bool jumpPressed;
    private bool grapplePressed;
    private bool grappleReleased;
    private bool airMoved;

    //==================================================
    // ■ 状態
    //==================================================

    private enum State
    {
        Grounded,
        Airborne,
        Shooting,
        Grappling,
        Dead,
        Goal
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
        pendulum.OnHalfTurn += PlayGrappleSE;
    }

    private void OnDestroy()
    {
        if (pendulum != null)
            pendulum.OnHalfTurn -= PlayGrappleSE;
    }

    private void PlayGrappleSE()
    {
        AudioManager.Instance().PlaySE(grappleSE);
    }
    public override void OnReset()
    {
        boost.ResetBoost();
    }

    public override void OnGoalEvent()
    {
        base.OnGoalEvent();
        state = State.Goal;
    }   

    private void Update()
    {
        if (state == State.Dead)
            return;

        if (grappleCooldownTimer > 0f)
            grappleCooldownTimer -= Time.deltaTime;

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

            case State.Airborne:

                if (airMoved)
                {
                    GroundMove();
                }
                break;
            case State.Dead:
                break;
            case State.Goal:
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

        if (IsGrounded()) { 
            state = State.Grounded;
            airMoved = true;
        }
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
        if (IsGrounded())
        {
            StopGrapple();
            return;
        }

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

        var speed = state != State.Airborne ? 10 : 5;

        rb.linearVelocity =
            new Vector2(
                Mathf.Lerp(
                    rb.linearVelocity.x,
                    target,
                    speed * Time.fixedDeltaTime),
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

        AudioManager.Instance().PlaySE(jumpSE);
    }

    //==================================================
    // ■ グラップル
    //==================================================

    private void StartRopeShot()
    {
        if (!TryFindTarget(out RaycastHit2D hit))
            return;

        if (grappleCooldownTimer > 0f)
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
        airMoved = false;
    }

    private void StopGrapple()
    {
        ReleaseRope();

        grappleCooldownTimer = coolTime;

        boost.Release();
    }

    private void ReleaseRope()
    {
        pendulum.ResetPendulum();

        state =
            IsGrounded()
            ? State.Grounded
            : State.Airborne;
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

    private bool TryFindTarget(out RaycastHit2D hit)
    {
        bool isGamepad =
            input.currentControlScheme == "Gamepad";

        if (!isGamepad)
        {
            Vector3 mouse =
                mainCamera.ScreenToWorldPoint(
                    input.actions["Look"]
                    .ReadValue<Vector2>());

            Vector2 dir =
                ((Vector2)mouse - rb.position)
                .normalized;

            hit = Physics2D.Raycast(
                rb.position,
                dir,
                maxDistance,
                grappleLayer);

            return hit.collider != null;
        }

        // -------------------------
        // Gamepad Aim Assist
        // -------------------------

        Vector2 stickDir =
            input.actions["Look"]
            .ReadValue<Vector2>();

        if (stickDir.sqrMagnitude <
            minStickInput * minStickInput)
        {
            hit = default;
            return false;
        }

        stickDir.Normalize();

        RaycastHit2D[] hits =
            Physics2D.CircleCastAll(
                rb.position,
                aimAssistRadius,
                stickDir,
                maxDistance,
                grappleLayer);

        if (hits.Length == 0)
        {
            hit = default;
            return false;
        }

        float bestScore = float.MinValue;
        int bestIndex = -1;

        for (int i = 0; i < hits.Length; i++)
        {
            Vector2 toTarget =
                ((Vector2)hits[i].point -
                 rb.position).normalized;

            float directionScore =
                Vector2.Dot(
                    stickDir,
                    toTarget);

            float distanceScore =
                1f -
                (hits[i].distance /
                 maxDistance);

            float score =
                directionScore * 0.8f +
                distanceScore * 0.2f;

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        hit = hits[bestIndex];
        return true;
    }

    private bool IsGrounded()
    {
        var ans = Physics2D.Raycast(
            rb.position,
            Vector2.down,
            0.4f,
            groundLayer);

        var lans = Physics2D.Raycast(
            new Vector2(rb.position.x - 0.5f, rb.position.y),
            Vector2.down,
            0.4f,
            groundLayer);
        var rans = Physics2D.Raycast(
            new Vector2(rb.position.x + 0.5f, rb.position.y),
            Vector2.down,
            0.4f,
            groundLayer);

        return (ans || lans || rans);
    }

    //==================================================
    // ■ ダメージ
    //==================================================

    public void NockBack(float vec)
    {
        ReleaseRope();

        rb.linearVelocity = new Vector2(
            vec * knockBackX,
            knockBackY);

        HitStop(0.3f);
        state = State.Airborne;
    }

    private IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0.05f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    public void Die()
    {
        Debug.Log("DIe");
        if (state == State.Dead)
            return;

        if (boost.IsInvincible)
            return;

        ReleaseRope();

        state = State.Dead;

        rb.linearVelocity = Vector2.zero;
    }

    public void ResetPlayer() {
        state = State.Grounded;
    }
}