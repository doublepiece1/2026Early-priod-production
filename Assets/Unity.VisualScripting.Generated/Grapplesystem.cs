using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Grapplerope))]

public class Grapplesystem : MonoBehaviour
{
    [Header("グラップル設定")]
    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private float releaseDistance = 1.2f;

    [Header("スイング物理")]
    [Tooltip("ロープの硬さ")]
    [SerializeField] private float ropeTension = 80f;
    [Tooltip("ロープを縮める速度（毎秒）")]
    [SerializeField] private float reelSpeed = 2f;
    [Tooltip("最小ロープ長")]
    [SerializeField] private float minRopeLength = 1.5f;
    [Tooltip("スイング中の重力スケール（小さいほどロープに乗ってる感が出る）")]
    [SerializeField] private float swingGravityScale = 0.4f;
    [Tooltip("スイング加速倍率")]
    [SerializeField] private float boostMultiplier = 3f;
    [Tooltip("スイング中の最大速度")]
    [SerializeField] private float maxSwingSpeed = 40f;

    [Header("飛翔（リリース後）")]
    [Tooltip("解除時の速度ブースト倍率")]
    [SerializeField] private float releaseBoost = 1.8f;
    [Tooltip("飛翔中の重力スケール（小さいほど遠くまで飛ぶ）")]
    [SerializeField] private float flyGravityScale = 0.7f;
    [Tooltip("飛翔中の空気抵抗（0推奨）")]
    [SerializeField] private float flyDrag = 0f;

    [Header("発射元")]
    [SerializeField] private Transform shootPoint;

    [Header("当たり判定")]
    [SerializeField] private LayerMask grappleLayer = ~0;
    [SerializeField] private float spreadAngle = 8f;
    [SerializeField] private bool showDebugRay = true;

    // ─────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Grapplerope rope;

    private bool isGrappling;
    private Vector2 anchorPoint;
    private float currentRopeLength;

    private float defaultGravityScale;
    private float defaultDrag;

    private InputAction upAction, downAction, rightAction, leftAction;
    private bool upHeld, downHeld, rightHeld, leftHeld;

    private bool Is_cooldown = false;
    [SerializeField, Header("クールタイム（秒）")] private float cool_time = 0f;

    // ─────────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rope = GetComponent<Grapplerope>();
        if (shootPoint == null) shootPoint = transform;

        defaultGravityScale = rb.gravityScale;
        defaultDrag = rb.linearDamping;

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

    private void OnKeyChanged()
    {
        if (!upHeld && !downHeld && !rightHeld && !leftHeld)
        {
            if (isGrappling) Release();
            return;
        }

        if (!Is_cooldown)
        {
            Vector2 dir = CalcDirection();
            if (dir != Vector2.zero)
                TryShoot(dir);
        }
    }

    private Vector2 CalcDirection()
    {
        float v = (upHeld ? 1f : 0f) + (downHeld ? -1f : 0f);
        float h = (rightHeld ? 1f : 0f) + (leftHeld ? -1f : 0f);
        return new Vector2(h, v).normalized;
    }

    private void TryShoot(Vector2 direction)
    {
        if (isGrappling) ReleaseWithoutBoost(); 


        if (isGrappling) Release();

        Vector2 origin = shootPoint.position;

        Collider2D[] myColliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in myColliders) col.enabled = false;
        bool hit = ShootRays(origin, direction, out Vector2 hitPoint);
        foreach (var col in myColliders) col.enabled = true;

        if (hit)
        {
            anchorPoint = hitPoint;
            isGrappling = true;
            currentRopeLength = Vector2.Distance(transform.position, anchorPoint);

            rb.gravityScale = swingGravityScale;
            rb.linearDamping = 0f;

            rope.StartRope(shootPoint, anchorPoint);
        }
        else
        {
            rope.StartMiss(shootPoint, origin + direction * maxDistance);
        }
    }

    private bool ShootRays(Vector2 origin, Vector2 dir, out Vector2 hitPoint)
    {
        hitPoint = Vector2.zero;
        Vector2[] directions = new Vector2[]
        {
            dir,
            Rotate(dir,  spreadAngle),
            Rotate(dir, -spreadAngle),
        };

        float closestDist = float.MaxValue;
        bool anyHit = false;

        foreach (Vector2 rayDir in directions)
        {
            if (showDebugRay)
                Debug.DrawRay(origin, rayDir * maxDistance, Color.yellow, 0.5f);

            RaycastHit2D info = Physics2D.Raycast(origin, rayDir, maxDistance, grappleLayer);
            if (info.collider != null)
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

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    // ── 毎フレーム ───────────────────────────────────────────
    private void FixedUpdate()
    {
        if (!isGrappling) return;
        ApplySwing();
    }

    private void ApplySwing()
    {
        Vector2 playerPos = transform.position;
        Vector2 toAnchor = anchorPoint - playerPos;
        float dist = toAnchor.magnitude;

        currentRopeLength = Mathf.Max(
            minRopeLength,
            currentRopeLength - reelSpeed * Time.fixedDeltaTime
        );

        if (currentRopeLength <= minRopeLength && dist < minRopeLength + 0.1f)
        {
            Release();
            return;
        }

        if (dist > currentRopeLength)
        {
            Vector2 ropeDir = toAnchor / dist;
            float radialVel = Vector2.Dot(rb.linearVelocity, ropeDir);

            if (radialVel < 0)
            {
                rb.AddForce(ropeDir * (-radialVel / Time.fixedDeltaTime) * rb.mass,
                            ForceMode2D.Force);
            }

            transform.position = (Vector2)transform.position
                                + ropeDir * (dist - currentRopeLength);
        }

        Vector2 ropeDirection = (anchorPoint - (Vector2)transform.position).normalized;
        Vector2 tangent = new Vector2(-ropeDirection.y, ropeDirection.x);
        float gravityTangential = Vector2.Dot(Physics2D.gravity, tangent);
        rb.AddForce(tangent * gravityTangential * boostMultiplier * rb.mass,
                    ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxSwingSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSwingSpeed;

        rope.UpdateRope(shootPoint, anchorPoint);
    }

    // ── 解除 ─────────────────────────────────────────────────
    private void Release()
    {
        if (!isGrappling) return;

        isGrappling = false;
        rb.linearVelocity = rb.linearVelocity * releaseBoost;
        rb.gravityScale = flyGravityScale;
        rb.linearDamping = flyDrag;

        rope.StopRope();
        StartCoroutine(Is_coolDown(cool_time));
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isGrappling)
        {
            rb.gravityScale = defaultGravityScale;
            rb.linearDamping = defaultDrag;

            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator Is_coolDown(float time_)
    {
        Is_cooldown = true;
        yield return new WaitForSeconds(time_);
        Debug.Log("Grapple cooldown end");
        Is_cooldown = false;
    }

    private void ReleaseWithoutBoost()
    {
        if (!isGrappling) return;

        isGrappling = false;

        // 既存の速度をリセット
        rb.linearVelocity = Vector2.zero;

        // 重力・空気抵抗をデフォルトに戻す
        rb.gravityScale = defaultGravityScale;
        rb.linearDamping = defaultDrag;

        rope.StopRope();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = shootPoint ? shootPoint.position : transform.position;
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        float len = 3f;
        Vector2[] dirs = {
            Vector2.up, Vector2.down, Vector2.right, Vector2.left,
            new Vector2( 1, 1).normalized, new Vector2(-1, 1).normalized,
            new Vector2( 1,-1).normalized, new Vector2(-1,-1).normalized
        };
        foreach (var d in dirs)
            Gizmos.DrawRay(origin, (Vector3)d * len);

        if (!isGrappling) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, (Vector3)anchorPoint);
        Gizmos.DrawWireSphere((Vector3)anchorPoint, 0.3f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector3)anchorPoint, currentRopeLength);
    }
}