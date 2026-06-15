using Kounosuke;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// グラップル移動＋スイング＋ブーストシステム統合版
/// </summary>
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
    // ■ 振り子パラメータ
    //==================================================
    [Header("振り子設定")]
    [SerializeField] private float gravity = 13f;
    [SerializeField] private float airResistance = 0.1f;
    [Header("スイング操作")]
    [SerializeField] private float swingAccel = 8f;

    //==================================================
    // ■ ブースト
    //==================================================
    [Header("ブースト")]
    [SerializeField] private float boostMax = 100f;
    [SerializeField] private float boostGainMultiplier = 10f;
    [SerializeField] private float releaseBoost = 5f;

    private float boostGauge;
    private bool isBoostReady;
    private bool isInvincible;

    //==================================================
    // ■ エフェクト
    //==================================================
    [Header("エフェクト")]
    [SerializeField] private EfectBase chargeEffect;
    [SerializeField] private ParticleSystem boostReadyEffect;
    [SerializeField] private ParticleSystem boostBurstEffect;
    [SerializeField] private TrailRenderer trail;

    //==================================================
    // ■ 参照
    //==================================================
    private Rigidbody2D rb;
    private Camera mainCamera;
    private LineRenderer lineRenderer;
    private PlayerInput input;

    //==================================================
    // ■ 入力
    //==================================================
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool grapplePressed;
    private bool grappleReleased;

    //==================================================
    // ■ 状態管理
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
    // ■ グラップルデータ
    //==================================================
    private Vector2 grapplePoint;
    private Vector2 shotDir;
    private float ropeLength;
    private float currentRopeLength;

    //==================================================
    // ■ 振り子状態
    //==================================================
    private float angle;
    private float angleVelocity;

    //==================================================
    // ■ Unity Lifecycle
    //==================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        input = GetComponent<PlayerInput>();
        mainCamera = Camera.main;

        lineRenderer.positionCount = 0;

        if (trail != null)
            trail.emitting = false;
    }

    private void Update()
    {
        if (state == State.Dead) return;

        ReadInput();
        HandleState();
        UpdateRopeVisual();
    }

    private void FixedUpdate()
    {
        if (state == State.Grappling)
        {
            SimulatePendulumPhysics();
            ApplyPendulumToRigidbody();
            ApplySwingVisual();
            ChargeBoost();
            ApplySwingInput();
        }
        else if (state == State.Grounded)
        {
            GroundMove();
        }
    }

    //==================================================
    // ■ 入力
    //==================================================

    private void ReadInput()
    {
        moveInput = input.actions["Move"].ReadValue<Vector2>();
        grapplePressed = input.actions["Atack"].WasPressedThisFrame();
        grappleReleased = input.actions["Atack"].WasReleasedThisFrame();
        jumpPressed = input.actions["Jump"].WasPressedThisFrame();
    }

    private void HandleState()
    {
        switch (state)
        {
            case State.Grounded:
                if (grapplePressed) StartRopeShot();
                if (jumpPressed) Jump();
                break;

            case State.Airborne:
                if (grapplePressed) StartRopeShot();
                if (IsGrounded()) state = State.Grounded;
                break;

            case State.Shooting:
                UpdateRopeShot();
                if (grappleReleased) StopGrapple();
                break;

            case State.Grappling:
                if (grappleReleased) StopGrapple();
                break;
        }

        if (!IsGrounded() && state == State.Grounded)
            state = State.Airborne;
    }

    //==================================================
    // ■ 移動
    //==================================================

    private void GroundMove()
    {
        float target = moveInput.x * moveSpeed;

        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, target, 10f * Time.fixedDeltaTime),
            rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (!IsGrounded()) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        state = State.Airborne;
    }

    //==================================================
    // ■ グラップル
    //==================================================

    private void StartRopeShot()
    {
        if (!TryFindTarget(out RaycastHit2D hit)) return;

        grapplePoint = hit.point;
        shotDir = (grapplePoint - (Vector2)transform.position).normalized;

        currentRopeLength = 0;
        state = State.Shooting;

        lineRenderer.positionCount = 2;
    }

    private void UpdateRopeShot()
    {
        currentRopeLength += ropeShotSpeed * Time.deltaTime;

        if (Vector2.Distance(rb.position, grapplePoint) <= currentRopeLength)
        {
            BeginGrapple();
        }
    }

    private void BeginGrapple()
    {
        ropeLength = Vector2.Distance(rb.position, grapplePoint);

        Vector2 offset = rb.position - grapplePoint;
        angle = Mathf.Atan2(offset.x, -offset.y);

        Vector2 tangent = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        angleVelocity = Vector2.Dot(rb.linearVelocity, tangent) / ropeLength;

        state = State.Grappling;
    }

    private void StopGrapple()
    {
        ReleaseRope();

        if (isBoostReady)
            ApplyBoost();
        else
            ApplyReleaseBoost();
    }

    private void ReleaseRope()
    {
        state = IsGrounded() ? State.Grounded : State.Airborne;

        lineRenderer.positionCount = 0;
        transform.rotation = Quaternion.identity;

        if (trail != null)
            trail.emitting = false;

        if (chargeEffect != null)
            chargeEffect.SetChargeMode(false);
    }

    //==================================================
    // ■ 振り子（コア）
    //==================================================

    private void SimulatePendulumPhysics()
    {
        Vector2 offset = rb.position - grapplePoint;

        angle = Mathf.Atan2(offset.x, -offset.y);

        float accel = -gravity * Mathf.Sin(angle) / ropeLength;

        angleVelocity += accel * Time.fixedDeltaTime;
        angleVelocity *= (1f - airResistance * Time.fixedDeltaTime);

        angleVelocity = Mathf.Clamp(angleVelocity, -1.4f, 1.4f);

        angle += angleVelocity * Time.fixedDeltaTime;
    }

    /// <summary>
    /// グラップル中の左右入力をスイング加速に変換
    /// </summary>
    private void ApplySwingInput()
    {
        if (ropeLength <= 0f) return;

        // 接線方向（振り子の進行方向）
        Vector2 tangent = new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        );

        float input = moveInput.x;

        float direction = Mathf.Sign(Vector2.Dot(tangent, Vector2.right));

        var addSpeed = tangent * input * swingAccel * direction * Time.fixedDeltaTime;
        rb.linearVelocity += addSpeed;
    }

    private void ApplyPendulumToRigidbody()
    {
        Vector2 offset = new Vector2(
            Mathf.Sin(angle),
            -Mathf.Cos(angle)
        ) * ropeLength;

        Vector2 target = grapplePoint + offset;

        Vector2 tangentVel = new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ) * (angleVelocity * ropeLength);

        rb.position = target;
        rb.linearVelocity = tangentVel;
    }

    private void ApplySwingVisual()
    {
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }

    //==================================================
    // ■ ブースト
    //==================================================

    private void ChargeBoost()
    {
        if (isBoostReady) return;

        float speed = rb.linearVelocity.magnitude;
        boostGauge += speed * boostGainMultiplier * Time.fixedDeltaTime;

        if (chargeEffect != null)
            chargeEffect.SetChargeMode(true);

        if (boostGauge >= boostMax)
        {
            boostGauge = boostMax;
            isBoostReady = true;
            isInvincible = true;

            if (boostReadyEffect != null)
                boostReadyEffect.Play();

            if (chargeEffect != null)
                chargeEffect.SetChargeMode(false);
        }
    }

    private void ApplyBoost()
    {
        Vector2 dir = rb.linearVelocity.normalized;
        if (dir == Vector2.zero) dir = Vector2.right;

        rb.linearVelocity = Vector2.zero;

        if (boostBurstEffect != null)
            boostBurstEffect.Play();

        rb.AddForce(dir * releaseBoost * 3f, ForceMode2D.Impulse);

        isBoostReady = false;
        boostGauge = 0;
        StartCoroutine(InvincibleTime());
    }

    private void ApplyReleaseBoost()
    {
        if (rb.linearVelocity.magnitude < 0.5f) return;

        rb.AddForce(rb.linearVelocity.normalized * releaseBoost, ForceMode2D.Impulse);
    }

    private IEnumerator InvincibleTime()
    {
        yield return new WaitForSeconds(1.5f);
        isInvincible = false;
    }

    //==================================================
    // ■ ユーティリティ
    //==================================================

    private void UpdateRopeVisual()
    {
        if (state != State.Shooting && state != State.Grappling) return;

        Vector2 start = transform.position;

        lineRenderer.SetPosition(0, start);

        if (state == State.Shooting)
            lineRenderer.SetPosition(1, start + shotDir * currentRopeLength);
        else
            lineRenderer.SetPosition(1, grapplePoint);
    }

    private bool TryFindTarget(out RaycastHit2D hit)
    {
        Vector3 mouse = mainCamera.ScreenToWorldPoint(input.actions["Look"].ReadValue<Vector2>());
        Vector2 dir = (mouse - transform.position).normalized;

        hit = Physics2D.Raycast(transform.position, dir, maxDistance, grappleLayer);

        return hit.collider != null;
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(rb.position, Vector2.down, 0.4f, groundLayer);
    }

    //==================================================
    // ■ 死亡
    //==================================================

    public void Die()
    {
        if (state == State.Dead) return;

        ReleaseRope();
        state = State.Dead;

        rb.linearVelocity = Vector2.zero;
    }
}