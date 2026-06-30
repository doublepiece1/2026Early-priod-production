using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kounosuke
{
    public class PlayerRespon : GimmickBase
    {
        [Header("プレイヤーHp設定")]
        [Tooltip("Hp")] public int Hp { get; private set; } = 0;
        [SerializeField, Tooltip("MaxHp")] private int MaxHp = 3;
        [SerializeField] private float invincibleTime = 1.0f;
        public bool isInvincible = false;
        private bool colitionenemy = false;

        private Vector3 startPosition;
        private TarzanAction tarzan;
        private BoostController boostController;

        private void Awake() {
            tarzan = GetComponent<TarzanAction>();
            boostController = GetComponent<BoostController>();
        }

        /// <summary>
        /// 初期位置セット関数
        /// </summary>
        public void SetPos(Transform pos) {
            startPosition = pos.position;
        }

        public override void OnStart()
        {
            base.OnStart();
            SetPos(transform);
            SetHp(MaxHp);
        }

        /// <summary>
        /// リセット関数
        /// </summary>
        public override void OnReset() {
            transform.position = startPosition;
            if (tarzan != null) {
                tarzan.ResetPlayer();
            }
            SetHp(MaxHp);
        }
        public override void OnGoalEvent()
        {
            base.OnGoalEvent();

        }

        public void SetHp(int hp)
        {
            Hp = hp;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("DeadZone"))
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
                tarzan?.Die();
            }
            if (collision.gameObject.CompareTag("Ground")) {
                tarzan?.CollitionWall();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (colitionenemy) return;
                if (isInvincible)
                {
                    boostController.AddBoost();
                    return;
                }
                var vec = transform.position - collision.gameObject.transform.position;
                NockBack(vec.x);
                TakeDamage(1);
            }
        }

        private void TakeDamage(int damage)
        {
            Hp -= damage;

            if (Hp <= 0)
            {
                tarzan?.Die();
                FindAnyObjectByType<GameManager>().PlayerDeath(null).Forget();
                return;
            }

            InvincibleRoutine().Forget();
        }
        private void NockBack(float vecx)
        {
            tarzan?.NockBack(vecx);
        }

        private async UniTaskVoid InvincibleRoutine()
        {
            isInvincible = true;
            colitionenemy = true;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            var second = (int)(invincibleTime / 10 * 1000);
            Debug.Log(second);
            // ここで点滅とか入れてもOK
            for (int i = 0; i < 5; i++)
            {
                sr.enabled = false;
                await UniTask.Delay(second);
                sr.enabled = true;
                await UniTask.Delay(second);
            }
            colitionenemy = false;
            isInvincible = false;
        }
    }
}