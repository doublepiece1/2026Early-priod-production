using UnityEngine;
using DG.Tweening;

namespace Kounosuke
{

    public class CollitionMoveFllor : MonoBehaviour
    {
        [SerializeField,Header("移動地点の目印")] private Vector3[] Move_Points;
        [SerializeField,Header("一回の移動時間")] private float moveDuration = 1.0f;
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
                
            ContactPoint2D contact = collision.GetContact(0);

            if (contact.normal.y > 0.5f)  {
                Debug.Log("Player Collision Up");
                MoveAction();
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
            foreach (Vector3 point in Move_Points) {
                moveSequence.Append(transform.DOMove(point, moveDuration).SetEase(Ease.Linear));
            }

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