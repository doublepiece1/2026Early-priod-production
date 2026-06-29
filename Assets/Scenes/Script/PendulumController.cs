using UnityEngine;

namespace Kounosuke
{
    public class PendulumController : MonoBehaviour
    {
        [Header("振り子設定")]
        [SerializeField] private float gravity = 13f;
        [SerializeField] private float airResistance = 0.1f;

        [Header("スイング設定")]
        [SerializeField] private float swingAccel = 8f;
        [SerializeField] private float maxAngleVelocity = 1.5f;

        [Header("ロープ長")]
        [SerializeField] private float ropeAdjustSpeed = 5f;
        [SerializeField] private float minRopeLength = 1f;
        [SerializeField] private float maxRopeLength = 10f;
        [SerializeField] private float ropeRetractBoost = 2f;

        [Header("入力加速")]
        [SerializeField] private float inputVelocityDecay = 3f;
        private float retractVelocity;

        private float inputVelocity;

        private Rigidbody2D rb;

        private Vector2 grapplePoint;

        private float ropeLength;
        private float angle;
        private float angleVelocity;

        public float RopeLength => ropeLength;
        public float Speed => rb.linearVelocity.magnitude;

        [SerializeField]
        private float seMinSpeed = 5f;

        public System.Action OnHalfTurn;
        private float seCooldown;
        [SerializeField] private float seCooldownTime = 0.3f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Begin(Vector2 hookPoint)
        {
            grapplePoint = hookPoint;

            ropeLength = Vector2.Distance(rb.position, grapplePoint);

            Vector2 offset = rb.position - grapplePoint;

            angle = Mathf.Atan2(offset.x, -offset.y);
            Vector2 tangent = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            angleVelocity = Vector2.Dot(rb.linearVelocity, tangent) / ropeLength;
        }

        public void Tick(Vector2 moveInput)
        {
            AdjustRopeLength(moveInput.y);
            Simulate();
            ApplyInput(moveInput.x);
            ApplyToRigidbody();
        }

        private void AdjustRopeLength(float input)
        {
            if (Mathf.Abs(input) < 0.1f) {
                return;
            }
            var multvalue = 1.0f;
            if (Mathf.Abs(angle) > 1) {
                multvalue = 0.5f;
            }
            else if(Mathf.Abs(angle) > 2) {
                return;
            }

            float oldLength = ropeLength;

            ropeLength -= input * ropeAdjustSpeed * Time.fixedDeltaTime * multvalue;
            ropeLength = Mathf.Clamp(ropeLength,minRopeLength,maxRopeLength);

            if (ropeLength < oldLength) {
                Vector2 toHook = (grapplePoint - rb.position).normalized;
                rb.linearVelocity += toHook * ropeRetractBoost;
            }
        }

        private void Simulate()
        {
            Vector2 offset = rb.position - grapplePoint;
            angle = Mathf.Atan2(offset.x, -offset.y);
            float accel = -gravity * Mathf.Sin(angle) / ropeLength * Time.fixedDeltaTime;
            angleVelocity += accel;

            seCooldown -= Time.fixedDeltaTime;

            if (Mathf.Abs(angle) < 0.2f && Speed >= seMinSpeed && seCooldown<0) {
                OnHalfTurn?.Invoke();
            }

            angleVelocity *=1f -airResistance *Time.fixedDeltaTime;
            angleVelocity = Mathf.Clamp(angleVelocity,-maxAngleVelocity,maxAngleVelocity);

            angle += angleVelocity * Time.fixedDeltaTime;
        }

        private void ApplyInput(float input)
        {
            inputVelocity += input * swingAccel * Time.fixedDeltaTime;
            inputVelocity *= Mathf.Exp(-inputVelocityDecay * Time.fixedDeltaTime);
        }

        private void ApplyToRigidbody()
        {
            Vector2 offset = new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle)) * ropeLength;

            Vector2 target = grapplePoint + offset;
            Vector2 tangent = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            retractVelocity *= Mathf.Exp(-6f * Time.fixedDeltaTime);

            Vector2 tangentVel = tangent * (angleVelocity * ropeLength + inputVelocity + retractVelocity);

            rb.position = target;
            rb.linearVelocity = tangentVel;
        }

        public void ResetPendulum()
        {
            ropeLength = 0f;
            angle = 0f;
            angleVelocity = 0f;
            inputVelocity = 0f;
            retractVelocity = 0f;
        }

        
    }
}