using UnityEngine;

public class TarzanAction : MonoBehaviour
{
    [Header("�ݒ�")]
    public LayerMask grappleLayer; 
    public float maxDistance = 10f; 
    public float swingImpulse = 30f; 

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer lineRenderer;
    private Vector2 grapplePoint;

    private bool isAlive = true;

    void Start()
    {
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

        if (joint != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
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

            joint = gameObject.AddComponent<DistanceJoint2D>();
            joint.autoConfigureDistance = false;
            joint.connectedAnchor = grapplePoint;
            joint.distance = Vector2.Distance(transform.position, grapplePoint);

            joint.maxDistanceOnly = true;

            lineRenderer.positionCount = 2;

            rb.AddForce(direction * swingImpulse, ForceMode2D.Impulse);
        }
    }

    void StopGrapple()
    {
        if (joint != null)
        {
            Destroy(joint);
            lineRenderer.positionCount = 0;
        }
    }
    public void Die()
    {
        if (!isAlive) return; 
        isAlive = false; 
        StopGrapple();   

        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse); 
        

        Debug.Log("�v���C���[�����S���܂����B������~���܂��B");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }
}