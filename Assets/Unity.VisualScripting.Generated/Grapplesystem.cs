using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Grapplerope))]

public class Grapplesystem : MonoBehaviour
{
    [Header("グラップル設定")]
    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private float pullForce = 20f;
    [SerializeField] private float maxSpeed = 18f;
    [SerializeField] private float releaseDistance = 1.2f;

    [Header("発射元")]
    [SerializeField] private Transform shootPoint;  // 未設定なら自動で中心を使用

    [Header("当たり判定")]
    [Tooltip("糸を引っかけたいレイヤーを設定（デフォルト: Everything）")]
    [SerializeField] private LayerMask grappleLayer = ~0;

    [Tooltip("扇形サブRayの広がり角度（度）。大きいほど広い範囲を検出")]
    [SerializeField] private float spreadAngle = 8f;

    [Tooltip("デバッグ用Rayをゲーム実行中も表示する")]
    [SerializeField] private bool showDebugRay = true;

    // ─────────────────────────────────────────────────────────
    private Rigidbody rb;
    private Grapplerope rope;

    private bool isGrappling;
    private Vector3 anchorPoint;

    // キー状態
    private InputAction upAction, downAction, rightAction, leftAction;
    private bool upHeld, downHeld, rightHeld, leftHeld;

    // デバッグ表示用（最後に発射したRay情報）
    private Vector3 lastRayOrigin;
    private Vector3 lastRayDirection;
    private bool lastRayHit;

    // ─────────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rope = GetComponent<Grapplerope>();
        if (shootPoint == null) shootPoint = transform;

        upAction = new InputAction(binding: "<Keyboard>/upArrow");
        downAction = new InputAction(binding: "<Keyboard>/downArrow");
        rightAction = new InputAction(binding: "<Keyboard>/rightArrow");
        leftAction = new InputAction(binding: "<Keyboard>/leftArrow");

        upAction.performed += _ => { upHeld = true; OnKeyChanged(); };
        downAction.performed += _ => { downHeld = true; OnKeyChanged(); };
        rightAction.performed += _ => { rightHeld = true; OnKeyChanged(); };
        leftAction.performed += _ => { leftHeld = true; OnKeyChanged(); };

        upAction.canceled += _ => { upHeld = false; OnKeyChanged(); };
        downAction.canceled += _ => { downHeld = false; OnKeyChanged(); };
        rightAction.canceled += _ => { rightHeld = false; OnKeyChanged(); };
        leftAction.canceled += _ => { leftHeld = false; OnKeyChanged(); };
    }

    private void OnEnable()
    {
        upAction.Enable(); downAction.Enable();
        rightAction.Enable(); leftAction.Enable();
    }

    private void OnDisable()
    {
        upAction.Disable(); downAction.Disable();
        rightAction.Disable(); leftAction.Disable();
    }

    // ── キー変化時 ───────────────────────────────────────────
    private void OnKeyChanged()
    {
        if (!upHeld && !downHeld && !rightHeld && !leftHeld)
        {
            if (isGrappling) Release();
            return;
        }

        Vector3 dir = CalcDirection();
        if (dir != Vector3.zero)
            TryShoot(dir);
    }

    private Vector3 CalcDirection()
    {
        float v = (upHeld ? 1f : 0f) + (downHeld ? -1f : 0f);
        float h = (rightHeld ? 1f : 0f) + (leftHeld ? -1f : 0f);
        return new Vector3(h, v, 0f).normalized;
    }

    // ── 発射（当たり判定改善版） ──────────────────────────────
    private void TryShoot(Vector3 direction)
    {
        if (isGrappling) Release();

        // 発射元：プレイヤーの Collider を確実に抜けた位置から撃つ
        // 自分自身を無視するために IgnoreRaycastLayer や QueryTriggerInteraction を活用
        Vector3 origin = shootPoint.position;

        // プレイヤー自身を除外するため、自分の全Colliderを一時的に無効化してRaycast
        Collider[] myColliders = GetComponentsInChildren<Collider>();
        foreach (var col in myColliders) col.enabled = false;

        bool hit = ShootRays(origin, direction, out Vector3 hitPoint);

        // Colliderを元に戻す
        foreach (var col in myColliders) col.enabled = true;

        // デバッグ用に保存
        lastRayOrigin = origin;
        lastRayDirection = direction;
        lastRayHit = hit;

        if (hit)
        {
            anchorPoint = hitPoint;
            isGrappling = true;
            rope.StartRope(shootPoint, anchorPoint);
        }
        else
        {
            rope.StartMiss(shootPoint, origin + direction * maxDistance);
        }
    }

    /// <summary>
    /// 中央1本 + 左右に広げた2本の合計3本のRayを飛ばし、
    /// 最も近いヒット点を返す。いずれかがヒットすればtrue。
    /// </summary>
    private bool ShootRays(Vector3 origin, Vector3 dir, out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;

        // 扇形に広げる軸（Z軸固定、XY平面で回転）
        Vector3 axis = Vector3.forward;

        // 3方向のRay（中央・+spreadAngle・-spreadAngle）
        Vector3[] directions = new Vector3[]
        {
            dir,
            Quaternion.AngleAxis( spreadAngle, axis) * dir,
            Quaternion.AngleAxis(-spreadAngle, axis) * dir,
        };

        float closestDist = float.MaxValue;
        bool anyHit = false;

        foreach (Vector3 rayDir in directions)
        {
            if (showDebugRay)
                Debug.DrawRay(origin, rayDir * maxDistance, Color.yellow, 0.5f);

            if (Physics.Raycast(origin, rayDir, out RaycastHit info,
                                maxDistance, grappleLayer,
                                QueryTriggerInteraction.Ignore)) // Triggerは無視
            {
                if (showDebugRay)
                    Debug.DrawRay(origin, rayDir * info.distance, Color.cyan, 0.5f);

                if (info.distance < closestDist)
                {
                    closestDist = info.distance;
                    hitPoint = info.point;
                    anyHit = true;
                }
            }
        }

        return anyHit;
    }

    // ── 引き寄せ ─────────────────────────────────────────────
    private void Update()
    {
        if (isGrappling) ApplyPull();
    }

    private void ApplyPull()
    {
        Vector3 toAnchor = anchorPoint - transform.position;
        float dist = toAnchor.magnitude;

        if (dist < releaseDistance)
        {
            Release();
            return;
        }

        Vector3 pullDir = toAnchor.normalized;
        float currentSpeed = Vector3.Dot(rb.linearVelocity, pullDir);
        if (currentSpeed < maxSpeed)
            rb.AddForce(pullDir * pullForce, ForceMode.Force);

        rope.UpdateRope(shootPoint, anchorPoint);
    }

    // ── 解除 ─────────────────────────────────────────────────
    private void Release()
    {
        isGrappling = false;
        rope.StopRope();
    }

    // ── Gizmos ───────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = shootPoint ? shootPoint.position : transform.position;

        // 8方向の発射ラインをプレビュー
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        float len = 3f;
        Vector3[] previewDirs = {
            Vector3.up, Vector3.down, Vector3.right, Vector3.left,
            new Vector3( 1, 1,0).normalized, new Vector3(-1, 1,0).normalized,
            new Vector3( 1,-1,0).normalized, new Vector3(-1,-1,0).normalized
        };
        foreach (var d in previewDirs)
            Gizmos.DrawRay(origin, d * len);

        // グラップル中のアンカー
        if (!isGrappling) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, anchorPoint);
        Gizmos.DrawWireSphere(anchorPoint, 0.3f);
    }
}
