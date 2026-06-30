using UnityEngine;

namespace Kounosuke
{
    public class CollitionMoveFllor : GimmickBase
    {
        [SerializeField] private Vector3[] Move_Points;
        [SerializeField] private float stopTime = 1f;
        public Vector2 Delta { get; private set; }
        public Vector2 Velocity { get; private set; }
        private Vector2 prevPos;
        private Rigidbody2D rb;

        private int index = 0;
        private float waitTimer = 0f;

        private Vector2 from;
        private Vector2 to;
        private float moveTime = 1f;
        private float t = 0f;

        private bool moving = false;
        private bool waiting = false;
        private bool started = false;

        Vector3 start_pos;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // =========================
        // プレイヤー接触で起動
        // =========================
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player"))
                return;

            StartMove();
        }

        public override void OnStart()
        {
            base.OnStart();
            start_pos = transform.position;
        }

        public override void OnReset()
        {
            transform.position = start_pos;
            started = false;
            waiting = false;
            started = false;
            index = 0;
        }

        private void StartMove()
        {
            if (started) return;

            started = true;
            SetupNext();
        }

        private void FixedUpdate()
        {
            if (!started) return;
            if (Move_Points == null || Move_Points.Length == 0) return;

            if (waiting)
            {
                waitTimer -= Time.fixedDeltaTime;

                if (waitTimer <= 0f)
                {
                    waiting = false;
                    SetupNext();
                }
                return;
            }

            if (!moving)
                return;

            t += Time.fixedDeltaTime / moveTime;

            Vector2 newPos = Vector2.Lerp(from, to, t);

            // 今フレーム床が移動する量
            Delta = newPos - rb.position;

            rb.MovePosition(newPos);

            if (t >= 1f)
            {
                SetupNext();
            }
        }

        private void SetupNext()
        {
            if (index >= Move_Points.Length)
            {
                waiting = true;
                waitTimer = stopTime;
                moving = false;
                return;
            }

            from = rb.position;

            Vector3 p = Move_Points[index];
            to = new Vector2(p.x, p.y);
            moveTime = p.z;

            t = 0f;
            moving = true;

            index++;
        }
    }
}