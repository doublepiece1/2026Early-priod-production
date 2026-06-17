using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kounosuke
{
    public class PlayerRespon : GimmickBase
    {
        [Header("プレイヤーHp設定")]
        [SerializeField, Tooltip("Hp")] private int Hp = 0;
        [SerializeField, Tooltip("MaxHp")] private int MaxHp = 0;
        [SerializeField] private float invincibleTime = 1.0f;
        private bool isInvincible = false;


        private Vector3 startPosition;
        private TarzanAction tarzan;
        private void Awake() {
            tarzan = GetComponent<TarzanAction>();
        }

        /// <summary>
        /// 初期位置セット関数
        /// </summary>
        public void SetPos() {
            startPosition = transform.position;
        }
        void Start() {
            SetPos();
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

        public void SetHp(int hp)
        {
            Hp = hp;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("DeadZone"))
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null) {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                tarzan?.Die();
            }

            if (collision.gameObject.CompareTag("Enemy"))
            {
                TakeDamage(1);
            }
        }

        private void TakeDamage(int damage)
        {
            if (isInvincible) return;

            Hp -= damage;

            if (Hp <= 0)
            {
                FindAnyObjectByType<GameManager>().PlayerDeath(null).Forget();
                return;
            }

            InvincibleRoutine().Forget();
        }

        private async UniTaskVoid InvincibleRoutine()
        {
            isInvincible = true;
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

            isInvincible = false;
        }
    }
}