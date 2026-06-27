using DG.Tweening;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
            moveSequence = DOTween.Sequence();
            var v2 = new Vector2(transform.position.x, transform.position.y);
            moveSequence.Append(rb.DOMove(v2 + TargetPoint, time).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Yoyo);
        }

        public override void OnReset()
        {
            base.OnReset();
            moveSequence?.Kill();
            OnStart();
        }

        protected override void OnCollitionEvent()
        {
            moveSequence?.Kill();
        }
    }
}