using DG.Tweening;
using UnityEngine;

namespace Kounosuke
{
    public class MovingEnemy : CollitionEnemy
    {
        [SerializeField, Header("初期地点と往復する目印となるポイント(初期地点からどのくらい動くか)")] private Vector2 TargetPoint;
        [SerializeField, Header("往復する時間")] private float time = 1;
        private Sequence moveSequence;
        public override void OnStart()
        {
            base.OnStart();
            Move();
        }

        private void Move()
        {
            Debug.Log("EnemyMove");
            moveSequence = DOTween.Sequence();
            Vector2 v2 = rb.position;

            moveSequence.Append(rb.DOMove(v2 + TargetPoint, time)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        public override void OnReset()
        {
            
            moveSequence?.Kill();
            base.OnReset();
            Move();
        }

        protected override void OnCollitionEvent()
        {
            moveSequence?.Kill();
        }
    }
}