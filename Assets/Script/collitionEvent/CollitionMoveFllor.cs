using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace Kounosuke
{
    public class CollitionMoveFllor : MonoBehaviour
    {
        [SerializeField,Header("移動地点の目印のVector3 + 移動にかかる時間")] private Vector3[] Move_Points;
        [SerializeField, Header("最終的に停止して、消滅するまでの時間")] private float stopTime = 1;

        private Rigidbody2D rb;
        private Vector3 start_pos;

        private Sequence moveSequence;
        bool is_sequence_playing = false;
        /// <summary>
        /// ギミックスタート関数
        /// </summary>
        public void Start() {
            rb = GetComponent<Rigidbody2D>();
            start_pos = transform.position;
            ResetMoveFloor();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player")) {
                return;
            }
            if(is_sequence_playing)
            {
                return;
            }
            ContactPoint2D contact = collision.GetContact(0);
            if (contact.normal.y < 0.1f) {
                Debug.Log("Player Collision Up");
                MoveAction();
            }
        }

        /// <summary>
        /// 移動本体関数
        /// </summary>
        private void MoveAction()
        {
            if (Move_Points == null || Move_Points.Length == 0) {
                return;
            }
            is_sequence_playing = true;
            moveSequence = DOTween.Sequence();

            //ポイント追加
            foreach (Vector3 point in Move_Points) {
                var time = point.z;
                var pos = new Vector2(point.x, point.y);
                moveSequence.Append(rb.DOMove(pos, time).SetEase(Ease.Linear));
            }

            //  ここに数秒間停止する処理を追加
            moveSequence.AppendInterval(stopTime);

            //終了時取得
            moveSequence.OnComplete(() => {
                this.gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// ギミックリセット関数
        /// </summary>
        public void OnReset() {
            ResetMoveFloor();
        }

        /// <summary>
        /// リセット本体
        /// </summary>
        public void ResetMoveFloor()
        {
            transform.position = start_pos;
            this.gameObject.SetActive(true);
            moveSequence?.Kill();
            is_sequence_playing = false;
        }
    }
}