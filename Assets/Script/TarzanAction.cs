using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TarzanAction : MonoBehaviour
{
    [Header("ワイヤー設定")]
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxDistance = 3f; //糸が届く距離
    [SerializeField] private float releaseBoost = 5f;//吹っ飛び速度

    [SerializeField] private float ropeShotSpeed = 60f;

    [Header("振り子設定")]
    [SerializeField] private float gravity = 13f;//数字がでかいほど爽快感が増す
    [SerializeField] private float airResistance = 0.1f;//空気抵抗

    [Header("描画設定")]
    [SerializeField] private Transform handAncorPoint;

    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpPower = 8f;
    [SerializeField] private LayerMask groundLayer;

    //  参照
    private Camera mainCamera;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    //  入力
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool grapplePressed;
    private bool grappleReleased;

    private enum GrappleState
    {
        Grounded,
        Airborne,
        Grappling,
        Shooting,
        Dead
    }

    private GrappleState state = GrappleState.Grounded;

    private Vector2 grapplePoint;
    private Vector2 shotDirection;
    private float currentRopeLength;
    private float ropeLength;
    private float angle;
    private float angleVelocity;

    /// <summary>
    /// 参照取得関数
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        playerInput = GetComponent<PlayerInput>();

        mainCamera = Camera.main;
        lineRenderer.positionCount = 0;
    }

    /// <summary>
    /// Update関数
    /// </summary>
    private void Update()
    {
        if (state == GrappleState.Dead) return;

        ReadInput();

        switch (state)
        {
            case GrappleState.Grounded:
                UpdateGrounded();
                break;

            case GrappleState.Airborne:
                UpdateAirborne();
                break;

            case GrappleState.Shooting:
                UpdateShooting();
                break;

            case GrappleState.Grappling:
                UpdateGrappling();
                break;
        }

        UpdateRopeVisual();
    }

    private void ReadInput()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        grapplePressed = playerInput.actions["Atack"].WasPressedThisFrame();
        grappleReleased = playerInput.actions["Atack"].WasReleasedThisFrame();
        jumpPressed = playerInput.actions["Jump"].WasPressedThisFrame();
    }

    private void UpdateGrounded()
    {
        if (grapplePressed)
            StartRopeShot();

        if (jumpPressed)
            TryJump();

        // 地面から離れたら空中へ
        if (!IsGrounded())
            state = GrappleState.Airborne;
    }

    private void UpdateAirborne()
    {
        if (grapplePressed)
            StartRopeShot();

        // 着地したら地上へ
        if (IsGrounded())
            state = GrappleState.Grounded;
    }

    private void UpdateShooting()
    {
        UpdateRopeShot();

        if (grappleReleased)
            StopGrapple();

        if (IsGrounded())
        {
            state = GrappleState.Grounded;
        }
    }

    private void UpdateGrappling()
    {
        if (grappleReleased)
            StopGrapple();
    }

    /// <summary>
    /// グラップル処理更新関数
    /// </summary>
    private void FixedUpdate()
    {
        switch (state)
        {
            case GrappleState.Grounded:
                GroundMove();
                break;

            case GrappleState.Airborne:
                break;

            case GrappleState.Grappling:
                SwingMove();
                UpdatePendulumMotion();
                break;
        }
    }

    /// <summary>
    /// 描画関数
    /// </summary>
    private void UpdateRopeVisual()
    {
        if (state != GrappleState.Shooting && state != GrappleState.Grappling)
        {
            return;
        }

        Vector2 startPos = GetRopeStartPosition();

        lineRenderer.SetPosition(0, startPos);

        if (state == GrappleState.Shooting)
        {
            Vector2 tipPos =
                startPos + shotDirection * currentRopeLength;

            lineRenderer.SetPosition(1, tipPos);
        }
        else
        {
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }


    private void GroundMove()
    {
        float targetSpeed = moveInput.x * moveSpeed;

        rb.linearVelocity = new Vector2(
            Mathf.Lerp(
                rb.linearVelocity.x,
                targetSpeed,
                10f * Time.fixedDeltaTime),
            rb.linearVelocity.y);
    }

    private void SwingMove()
    {
        angleVelocity += moveInput.x * 1.5f * Time.fixedDeltaTime;
    }

    private void TryJump()
    {
        if (!IsGrounded())
            return;

        Vector2 v = rb.linearVelocity;
        v.y = jumpPower;
        rb.linearVelocity = v;

        state = GrappleState.Airborne;
    }

    /// <summary>
    /// 描画開始位置取得関数
    /// </summary>
    /// <returns></returns>
    private Vector2 GetRopeStartPosition()
    {
        return handAncorPoint != null
            ? (Vector2)handAncorPoint.position
            : rb.position;
    }

    /// <summary>
    /// グラップル処理関数
    /// </summary>
    private void UpdatePendulumMotion()
    {
        if (state != GrappleState.Grappling)
            return;

        CalculatePendulum();

        ApplyPendulumMovement();

        ApplyVisualRotation();
    }

    /// <summary>
    /// 振り子計算関数
    /// </summary>
    private void CalculatePendulum()
    {
        Vector2 offset =
            rb.position - grapplePoint;

        angle =
            Mathf.Atan2(offset.x, -offset.y);

        float angularAcceleration =
            -gravity * Mathf.Sin(angle) / ropeLength;

        angleVelocity +=
            angularAcceleration *
            Time.fixedDeltaTime;

        angleVelocity *=
            (1f - airResistance * Time.fixedDeltaTime);

        angleVelocity = Mathf.Clamp(angleVelocity, -1.4f, 1.4f);

        angle +=
            angleVelocity *
            Time.fixedDeltaTime;
    }

    /// <summary>
    /// ポジション更新関数
    /// </summary>
    private void ApplyPendulumMovement()
    {
        Vector2 offset =
            new Vector2(
                Mathf.Sin(angle),
                -Mathf.Cos(angle))
            * ropeLength;

        Vector2 targetPosition =
            grapplePoint + offset;

        Vector2 tangentVelocity =
            new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle))
            * (angleVelocity * ropeLength);

        rb.position = targetPosition;
        rb.linearVelocity = tangentVelocity;
    }

    /// <summary>
    /// 角度更新関数
    /// </summary>
    private void ApplyVisualRotation()
    {
        float degree =
            angle * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0, 0, degree);
    }


    private void StartRopeShot()
    {
        if (state != GrappleState.Grounded && state != GrappleState.Airborne)
        {
            return;
        }

        if (!TryFindGrapplePoint(out RaycastHit2D hit))
            return;

        grapplePoint = hit.point;

        Vector2 startPos = GetRopeStartPosition();

        shotDirection =
            (grapplePoint - startPos).normalized;

        currentRopeLength = 0f;

        state = GrappleState.Shooting;

        lineRenderer.positionCount = 2;
    }

    private void UpdateRopeShot()
    {
        if (state != GrappleState.Shooting)
            return;

        currentRopeLength +=
            ropeShotSpeed *
            Time.deltaTime;

        float targetDistance =
            Vector2.Distance(
                GetRopeStartPosition(),
                grapplePoint);

        if (currentRopeLength >= targetDistance)
        {
            BeginGrapple(grapplePoint);
        }
    }

    /// <summary>
    /// グラップル位置取得関数
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    private bool TryFindGrapplePoint(out RaycastHit2D hit)
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(playerInput.actions["Look"].ReadValue<Vector2>());

        Vector2 direction =
            (mouseWorld - transform.position)
            .normalized;

        hit = Physics2D.Raycast(
            transform.position,
            direction,
            maxDistance,
            grappleLayer);

        return hit.collider != null;
    }

    /// <summary>
    /// グラップル開始処理関数
    /// </summary>
    /// <param name="point"></param>
    private void BeginGrapple(Vector2 point)
    {
        grapplePoint = point;

        ropeLength =
            Vector2.Distance(
                rb.position,
                grapplePoint);

        InitializePendulum();

        state = GrappleState.Grappling;

        lineRenderer.positionCount = 2;
    }
    
    /// <summary>
    /// 初期化関数
    /// </summary>
    private void InitializePendulum()
    {
        Vector2 offset = rb.position - grapplePoint;

        angle = Mathf.Atan2(offset.x, -offset.y);

        Vector2 tangent = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        float tangentialSpeed = Vector2.Dot(rb.linearVelocity, tangent);

        angleVelocity = tangentialSpeed / ropeLength;
    }

    /// <summary>
    /// グラップル終了関数
    /// </summary>
    private void StopGrapple()
    {
        bool wasGrappling =
        state == GrappleState.Grappling;

        if (!wasGrappling &&
            state != GrappleState.Shooting)
            return;

        ReleaseRope();

        if (wasGrappling)
            ApplyReleaseBoost();
    }

    /// <summary>
    /// グラップル
    /// </summary>
    private void ReleaseRope()
    {
        state = IsGrounded()
        ? GrappleState.Grounded
        : GrappleState.Airborne;

        lineRenderer.positionCount = 0;

        transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 終了時に加速する処理
    /// </summary>
    private void ApplyReleaseBoost()
    {
        if (rb.linearVelocity.magnitude <= 0.5f)
            return;

        rb.AddForce(
            rb.linearVelocity.normalized *
            releaseBoost,
            ForceMode2D.Impulse);
    }

    /// <summary>
    /// 死亡処理関数
    /// </summary>
    public void Die()
    {
        if (state == GrappleState.Dead)
            return;

        if (state == GrappleState.Grappling ||
            state == GrappleState.Shooting)
        {
            ReleaseRope();
        }

        state = GrappleState.Dead;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * 8f,
            ForceMode2D.Impulse);

        Debug.Log("プレイヤーが死亡したので操作を停止します");
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(
            rb.position,
            Vector2.down,
            0.4f,
            groundLayer
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!ShouldReleaseGrapple(collision))
            return;

        StopGrapple();
    }

    private bool ShouldReleaseGrapple(Collision2D collision)
    {
        if (state != GrappleState.Grappling)
            return false;

        if (((1 << collision.gameObject.layer) & grappleLayer) == 0)
            return false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.7f)
                return true;
        }

        return false;
    }
}