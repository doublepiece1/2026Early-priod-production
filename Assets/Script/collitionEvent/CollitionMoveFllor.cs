using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;

namespace Kounosuke
{

    public class CollitionMoveFllor : MonoBehaviour
    {
        [SerializeField,Header("移動地点の目印のVector3 + 移動にかかる時間")] private Vector4[] Move_Points;
        [SerializeField, Header("最終的に停止して、消滅するまでの時間")] private float stopTime = 1;
        private Vector3 start_pos;

        private Sequence moveSequence;

        private void Start()
        {
            start_pos = transform.position;
            ResetMoveFloor();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player")) {
                return;
            }
            Debug.Log("あいうえお");
            ContactPoint2D contact = collision.GetContact(0);

            MoveAction();
            if (contact.normal.y > 0.5f)  {
                Debug.Log("Player Collision Up");
                
            }
        }

        private void MoveAction()
        {
            if (Move_Points == null || Move_Points.Length == 0) {
                return;
            }

            moveSequence?.Kill();
            moveSequence = DOTween.Sequence();

            //ポイント追加
            foreach (Vector4 point in Move_Points) {
                var time = point.w;
                var pos = new Vector3(point.x, point.y, point.z);
                moveSequence.Append(transform.DOMove(pos, time).SetEase(Ease.Linear));
            }

            //  ここに数秒間停止する処理を追加
            moveSequence.AppendInterval(stopTime);

            //終了時取得
            moveSequence.OnComplete(() => {
                this.gameObject.SetActive(false);
            });
        }

        public void ResetMoveFloor()
        {
            transform.position = start_pos;
        }
    }
}