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
    [SerializeField] private float pullForce = 20f;
    [SerializeField] private float maxSpeed = 18f;
    [SerializeField] private float releaseDistance = 1.2f;

    [Header("発射元")]
    [SerializeField] private Transform shootPoint;

    [Header("当たり判定")]
    [Tooltip("糸を引っかけたいレイヤー（TilemapのLayerを含めること）")]
    [SerializeField] private LayerMask grappleLayer = ~0;

    [Tooltip("扇形サブRayの広がり角度（度）")]
    [SerializeField] private float spreadAngle = 8f;

    [Tooltip("デバッグRayをSceneビューに表示する")]
    [SerializeField] private bool showDebugRay = true;

    // ─────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Grapplerope rope;

    private bool isGrappling;
    private Vector2 anchorPoint;

    // キー状態
    private InputAction upAction, downAction, rightAction, leftAction;
    private bool upHeld, downHeld, rightHeld, leftHeld;

    private bool Is_cooldown = false;
    [SerializeField, Header("クールタイム")] private int cool_time = 0;
    // ─────────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

    // ── 発射（Physics2D.Raycast でタイルマップに当たる） ─────
    private void TryShoot(Vector2 direction)
    {
        if (isGrappling) Release();

        Vector2 origin = shootPoint.position;

        // プレイヤー自身を一時的に無効化して誤検知を防ぐ
        Collider2D[] myColliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in myColliders) col.enabled = false;

        bool hit = ShootRays(origin, direction, out Vector2 hitPoint);

        foreach (var col in myColliders) col.enabled = true;

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
    /// 中央1本 + 左右に spreadAngle 度ずらした2本の計3本の
    /// Physics2D.Raycast を飛ばし、最も近いヒット点を返す。
    /// </summary>
    private bool ShootRays(Vector2 origin, Vector2 dir, out Vector2 hitPoint)
    {
        hitPoint = Vector2.zero;

        // 3方向（中央・右寄り・左寄り）
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

            // ★ Physics2D.Raycast でタイルマップの2Dコライダーに当たる
            RaycastHit2D info = Physics2D.Raycast(
                origin, rayDir, maxDistance, grappleLayer);

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

    // ── ベクトルを角度で回転（2D用） ─────────────────────────
    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos);
    }

    // ── 引き寄せ（毎フレーム） ────────────────────────────────
    private void Update()
    {
        if (isGrappling) ApplyPull();
    }

    private void ApplyPull()
    {
        Vector2 toAnchor = anchorPoint - (Vector2)transform.position;
        float dist = toAnchor.magnitude;

        if (dist < releaseDistance)
        {
            Release();
            return;
        }

        Vector2 pullDir = toAnchor.normalized;
        float currentSpeed = Vector2.Dot(rb.linearVelocity, pullDir);
        if (currentSpeed < maxSpeed)
            rb.AddForce(pullDir * pullForce, ForceMode2D.Force);

        rope.UpdateRope(shootPoint, anchorPoint);
    }

    // ── 解除 ─────────────────────────────────────────────────
    private void Release()
    {
        
        isGrappling = false;
        rope.StopRope();
        StartCoroutine(Is_coolDown(cool_time));
    }

    IEnumerator Is_coolDown(int time_)
    {
        Is_cooldown = true;
        for (int i=0; i<time_; i++)
        {
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Release");
        Is_cooldown = false;
        yield break;
    }

    // ── Gizmos ───────────────────────────────────────────────
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
    }

}
