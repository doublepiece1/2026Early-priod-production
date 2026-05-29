using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMove : MonoBehaviour
{
    [Header("移動・ジャンプ")]
    public float moveSpeed = 6f;
    public float airMoveSpeed = 3f;   // 空中横移動の強さ
    public float jumpForce = 13f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("糸（ロープ）")]
    [Tooltip("糸の最大射程")]
    public float maxRopeLength = 12f;
    [Tooltip("引っかかるレイヤー。未設定(Nothing=0)なら全Colliderに引っかかる")]
    public LayerMask ropeAnchorLayer;
    public LineRenderer ropeLineRenderer;

    [Header("振り子・スイング")]
    public float swingForce = 200f;
    public float releaseBoostUp = 0.4f;
    public float minReleaseSpeed = 7f;

    [Header("爽快感 - カメラ")]
    public Camera mainCamera;
    public float normalCamSize = 5f;
    public float fastCamSize = 7f;
    public float camZoomSpeed = 3f;

    [Header("爽快感 - 画面シェイク")]
    public float shakeOnLand = 0.15f;
    public float shakeOnShoot = 0.07f;
    public float shakeDuration = 0.18f;

    [Header("糸の色")]
    public Color ropeNormalColor = new Color(0.55f, 0.30f, 0.05f);
    public Color ropeTautColor = new Color(1.0f, 0.40f, 0.05f);
    public Color ropeFailColor = new Color(1.0f, 0.15f, 0.15f);

    // =========================================================
    //  内部変数
    // =========================================================

    Rigidbody2D rb;
    DistanceJoint2D ropeJoint;

    bool isGrounded;
    bool isRoping;
    Vector2 ropeAnchor;
    float currentRopeLength;

    // 入力（Update で読んで FixedUpdate で使う）
    float inputH;
    bool jumpPressed;
    bool ropeShootPressed;
    bool ropeDetachPressed;

    // 発射失敗演出
    bool shootFailed;
    Vector2 failedTarget;
    float failedTimer;
    const float FAIL_TIME = 0.22f;

    // カメラシェイク
    Vector3 camOriginalLocalPos;
    float shakeTimer;
    float curShakeIntensity;

    // スローモーション
    float slowTimer;
    const float SLOW_DURATION = 0.09f;
    const float SLOW_SCALE = 0.30f;

    // =========================================================
    //  初期化
    // =========================================================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // DistanceJoint2D を自動追加
        ropeJoint = gameObject.AddComponent<DistanceJoint2D>();
        ropeJoint.enabled = false;
        ropeJoint.enableCollision = true;

        // LineRenderer を自動生成
        if (ropeLineRenderer == null)
        {
            ropeLineRenderer = gameObject.AddComponent<LineRenderer>();
            ropeLineRenderer.startWidth = 0.07f;
            ropeLineRenderer.endWidth = 0.03f;
            ropeLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            ropeLineRenderer.positionCount = 2;
            ropeLineRenderer.sortingOrder = 2;
        }
        ropeLineRenderer.enabled = false;

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null) camOriginalLocalPos = mainCamera.transform.localPosition;
    }

    // =========================================================
    //  Update：入力読み取り・演出更新
    // =========================================================

    void Update()
    {
        ReadInput();
        HandleRopeInput();
        UpdateRopeVisual();
        UpdateCamera();
        UpdateShake();
        UpdateSlowMotion();
    }

    // =========================================================
    //  FixedUpdate：物理処理
    // =========================================================

    void FixedUpdate()
    {
        CheckGround();
        ApplyMove();
        ApplyJump();
    }

    // =========================================================
    //  入力読み取り
    // =========================================================

    void ReadInput()
    {
        // ── 横移動 ──
        // New Input System と旧 Input の両方に対応
        inputH = 0f;

        // New Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) inputH -= 1f;
            if (Keyboard.current.dKey.isPressed) inputH += 1f;
        }

        // 旧 Input System（Active Input Handling が Both のとき）
#if ENABLE_LEGACY_INPUT_MANAGER
        inputH += Input.GetAxisRaw("Horizontal");
        inputH = Mathf.Clamp(inputH, -1f, 1f);
#endif

        // ── ジャンプ ──
        jumpPressed = false;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;
#endif

        // ── 糸の発射・切断 ──
        ropeShootPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        ropeDetachPressed = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0)) ropeShootPressed = true;
        if (Input.GetMouseButtonDown(1)) ropeDetachPressed = true;
#endif
    }

    // =========================================================
    //  地面判定
    // =========================================================

    void CheckGround()
    {
        Vector2 pos = groundCheck != null
            ? (Vector2)groundCheck.position
            : (Vector2)transform.position + Vector2.down * 0.5f;

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(pos, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            if (isRoping) DetachRope();
            float impact = Mathf.Clamp(
                rb.linearVelocity.magnitude * 0.015f,
                shakeOnLand * 0.5f,
                shakeOnLand * 2f);
            TriggerShake(impact, shakeDuration);
        }
    }

    // =========================================================
    //  移動（FixedUpdate から呼ぶ）
    // =========================================================

    void ApplyMove()
    {
        if (isRoping)
        {
            // スイング中：横力を加えて振り子を加速
            rb.AddForce(new Vector2(inputH * swingForce * Time.fixedDeltaTime, 0f));
            return;
        }

        if (isGrounded)
        {
            // 地上：速度を直接セット
            rb.linearVelocity = new Vector2(inputH * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // 空中：力で緩やかに制御
            rb.AddForce(new Vector2(inputH * airMoveSpeed * 20f * Time.fixedDeltaTime, 0f));
            float clampX = Mathf.Clamp(rb.linearVelocity.x, -moveSpeed * 1.5f, moveSpeed * 1.5f);
            rb.linearVelocity = new Vector2(clampX, rb.linearVelocity.y);
        }
    }

    // =========================================================
    //  ジャンプ（FixedUpdate から呼ぶ）
    // =========================================================

    void ApplyJump()
    {
        if (!jumpPressed) return;
        jumpPressed = false;

        if (isGrounded && !isRoping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (isRoping)
        {
            ReleaseSwing();
        }
    }

    // =========================================================
    //  糸の操作（Update から呼ぶ）
    // =========================================================

    void HandleRopeInput()
    {
        if (ropeShootPressed) ShootRope();
        if (ropeDetachPressed) DetachRope();
    }

    void ShootRope()
    {
        DetachRope();

        if (mainCamera == null) return;

        Vector2 origin = transform.position;
        Vector3 mouseScreen = Mouse.current != null
            ? new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f)
            : new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - origin);
        float dist = dir.magnitude;
        if (dist < 0.01f) return;
        dir /= dist; // normalize

        // 制限①：プレイヤーより下は不可
        if (mouseWorld.y < origin.y - 0.5f)
        {
            ShowFail(origin + dir * Mathf.Min(dist, maxRopeLength));
            return;
        }

        // 制限②：射程外
        if (dist > maxRopeLength)
        {
            ShowFail(origin + dir * maxRopeLength);
            return;
        }

        // ── Raycast ──
        // ropeAnchorLayer が 0（Nothing）なら全レイヤーを対象にする
        int mask = (ropeAnchorLayer.value == 0)
            ? ~0          // 全レイヤー
            : (int)ropeAnchorLayer;

        // 自分自身に当たらないよう少しずらしてRaycast
        RaycastHit2D hit = Physics2D.Raycast(
            origin + dir * 0.3f,   // 自分のColliderを避けてスタート
            dir,
            maxRopeLength,
            mask);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            AttachRope(hit.point, Vector2.Distance(origin, hit.point));
            TriggerShake(shakeOnShoot, shakeDuration * 0.7f);
        }
        else
        {
            ShowFail(origin + dir * Mathf.Min(dist, maxRopeLength));
        }
    }

    void AttachRope(Vector2 anchor, float length)
    {
        ropeAnchor = anchor;
        currentRopeLength = Mathf.Max(length, 0.5f);
        isRoping = true;

        ropeJoint.connectedAnchor = anchor;
        ropeJoint.distance = currentRopeLength;
        ropeJoint.enabled = true;

        ropeLineRenderer.enabled = true;
        SetRopeColor(ropeNormalColor);
    }

    void DetachRope()
    {
        isRoping = false;
        ropeJoint.enabled = false;
        ropeLineRenderer.enabled = false;
    }

    // =========================================================
    //  振り子リリース
    // =========================================================

    void ReleaseSwing()
    {
        Vector2 vel = rb.linearVelocity;

        if (vel.magnitude < minReleaseSpeed)
        {
            Vector2 toPlayer = ((Vector2)transform.position - ropeAnchor).normalized;
            Vector2 tangent = new Vector2(-toPlayer.y, toPlayer.x);
            if (vel.x < 0f) tangent = -tangent;
            vel = tangent * minReleaseSpeed;
        }

        vel.y += vel.magnitude * releaseBoostUp;
        rb.linearVelocity = vel;

        DetachRope();
        TriggerSlowMotion();
        TriggerShake(shakeOnShoot * 1.5f, shakeDuration);
    }

    // =========================================================
    //  失敗演出
    // =========================================================

    void ShowFail(Vector2 target)
    {
        shootFailed = true;
        failedTarget = target;
        failedTimer = FAIL_TIME;

        ropeLineRenderer.enabled = true;
        ropeLineRenderer.SetPosition(0, transform.position);
        ropeLineRenderer.SetPosition(1, target);
        ropeLineRenderer.startColor = new Color(ropeFailColor.r, ropeFailColor.g, ropeFailColor.b, 0.8f);
        ropeLineRenderer.endColor = new Color(ropeFailColor.r, ropeFailColor.g, ropeFailColor.b, 0.1f);
    }

    // =========================================================
    //  糸の描画・色更新
    // =========================================================

    void UpdateRopeVisual()
    {
        if (isRoping)
        {
            ropeLineRenderer.enabled = true;
            ropeLineRenderer.SetPosition(0, transform.position);
            ropeLineRenderer.SetPosition(1, ropeAnchor);

            float actualDist = Vector2.Distance(transform.position, ropeAnchor);
            bool taut = actualDist >= currentRopeLength * 0.97f;
            SetRopeColor(taut ? ropeTautColor : ropeNormalColor);
        }
        else if (shootFailed)
        {
            failedTimer -= Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(failedTimer / FAIL_TIME);
            ropeLineRenderer.SetPosition(0, transform.position);
            ropeLineRenderer.SetPosition(1, failedTarget);
            ropeLineRenderer.startColor = new Color(ropeFailColor.r, ropeFailColor.g, ropeFailColor.b, alpha * 0.8f);
            ropeLineRenderer.endColor = new Color(ropeFailColor.r, ropeFailColor.g, ropeFailColor.b, 0f);
            if (failedTimer <= 0f)
            {
                shootFailed = false;
                ropeLineRenderer.enabled = false;
            }
        }
    }

    void SetRopeColor(Color c)
    {
        ropeLineRenderer.startColor = c;
        ropeLineRenderer.endColor = new Color(c.r, c.g, c.b, 0.35f);
    }

    // =========================================================
    //  カメラ（速度に応じてズームアウト）
    // =========================================================

    void UpdateCamera()
    {
        if (mainCamera == null) return;
        float speed = rb.linearVelocity.magnitude;
        float targetSize = Mathf.Lerp(normalCamSize, fastCamSize, Mathf.Clamp01(speed / 20f));
        mainCamera.orthographicSize = Mathf.Lerp(
            mainCamera.orthographicSize, targetSize, camZoomSpeed * Time.deltaTime);
    }

    // =========================================================
    //  画面シェイク
    // =========================================================

    void TriggerShake(float intensity, float duration)
    {
        curShakeIntensity = intensity;
        shakeTimer = duration;
    }

    void UpdateShake()
    {
        if (mainCamera == null || shakeTimer <= 0f)
        {
            if (mainCamera != null)
                mainCamera.transform.localPosition = camOriginalLocalPos;
            return;
        }
        shakeTimer -= Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(shakeTimer / shakeDuration);
        Vector3 rand = (Vector3)Random.insideUnitCircle * curShakeIntensity * t;
        mainCamera.transform.localPosition = camOriginalLocalPos + rand;
    }

    // =========================================================
    //  スローモーション
    // =========================================================

    void TriggerSlowMotion()
    {
        Time.timeScale = SLOW_SCALE;
        Time.fixedDeltaTime = 0.02f * SLOW_SCALE;
        slowTimer = SLOW_DURATION;
    }

    void UpdateSlowMotion()
    {
        if (slowTimer <= 0f) return;
        slowTimer -= Time.unscaledDeltaTime;
        if (slowTimer <= 0f)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    // =========================================================
    //  Gizmos（Scene ビュー確認）
    // =========================================================

    void OnDrawGizmosSelected()
    {
        Vector3 checkPos = groundCheck != null
            ? groundCheck.position
            : transform.position + Vector3.down * 0.5f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);

        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, maxRopeLength);
    }
}
