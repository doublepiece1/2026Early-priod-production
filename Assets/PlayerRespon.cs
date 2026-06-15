using Kounosuke;
using UnityEngine;

namespace Kounosuke
{
    public class PlayerRespon : GimmickBase
    {
        private Vector3 startPosition;

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
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("DeadZone")) {
                
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null) {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
            }
        }
    }
}