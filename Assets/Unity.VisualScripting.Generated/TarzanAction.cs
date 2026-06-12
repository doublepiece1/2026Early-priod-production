using UnityEngine;

public class TarzanAction : MonoBehaviour
{
    [Header("ワイヤー設定")]
    public LayerMask grappleLayer; 
    public float maxDistance = 3f; //糸が届く距離
    public float releaseBoost = 5f;//吹っ飛び速度

    public float ropeShotSpeed = 60f;

    [Header("振り子設定")]
    public float gravity = 13f;//数字がでかいほど爽快感が増す
    public float airResistance = 0.1f;//空気抵抗

    [Header("描画設定")]
    public Transform handAncorPoint;

    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    private bool isShooting = false;
    private bool isGrappling = false;


    private Vector2 grapplePoint;
    private Vector2 shotDirection;
    private float currentRopeLength;
    private float ropeLength;
    private float angle;
    private float angleVelocity;


    private bool isAlive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
       
        if (!isAlive) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (isShooting || isGrappling)
        {
            Vector2 startPos = (handAncorPoint != null) ? (Vector2)handAncorPoint.position : (Vector2)transform.position;
            lineRenderer.SetPosition(0, startPos);

            if (isShooting)
            {
                Vector2 currentTipPos = startPos + shotDirection * currentRopeLength;
                lineRenderer.SetPosition(1, currentTipPos);
            }
            else 
            {
                lineRenderer.SetPosition(1, grapplePoint);
            }
            
        }
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        if (isGrappling) 
        {
            Vector2 connectionToPlayer = (Vector2)transform.position - grapplePoint;

            angle = Mathf.Atan2(connectionToPlayer.x, -connectionToPlayer.y);

            float angleAcceleration = -gravity * Mathf.Sin(angle) / ropeLength;

            angleVelocity += angleAcceleration * Time.fixedDeltaTime;
            angleVelocity *= (1f - airResistance * Time.fixedDeltaTime);
            angle += angleVelocity * Time.fixedDeltaTime;

            Vector2 newOffset = new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle)) * ropeLength;
            Vector2 newPosition = grapplePoint + newOffset;

            Vector2 tangentVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (angleVelocity * ropeLength);

            rb.position = newPosition;
            rb.linearVelocity = tangentVelocity;

            float degreeAngle = angle * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, degreeAngle);



        }
    }

    void StartGrapple()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;

            ropeLength = Vector2.Distance(transform.position, grapplePoint);

            Vector2 connectionToPlayer = (Vector2)transform.position - grapplePoint;
            angle = Mathf.Atan2(connectionToPlayer.x, -connectionToPlayer.y);

            angleVelocity = rb.linearVelocity.x / ropeLength;

            isGrappling = true;
            lineRenderer.positionCount = 2;
        }
    }

    void StopGrapple()
    {
        if (isGrappling)
        {
            isGrappling = false;
            lineRenderer.positionCount = 0;

            transform.rotation = Quaternion .identity;
            if (rb.linearVelocity.magnitude > 1f) 
            {
                rb.AddForce(rb.linearVelocity.normalized * releaseBoost, ForceMode2D.Impulse);
            }
        }
    }
    public void Die()
    {
        if (!isAlive) return; 
        isAlive = false; 
        StopGrapple();
        rb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
        rb.linearVelocity = Vector2.zero; ; 
        Debug.Log("プレイヤーが死亡したので操作を停止します");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGrappling && ((1 << collision.gameObject.layer) & grappleLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts) 
            {
                if (Mathf.Abs(contact.normal.x) > 0.7f) //ここで角度を色々いじる
                {
                    StopGrapple();
                    break;
                }
            }
        }
    }
}