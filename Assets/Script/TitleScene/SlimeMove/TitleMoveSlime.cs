using UnityEngine;
using DG.Tweening;

namespace Kounosuke {
    public class TitleMoveSlime : MonoBehaviour
    {
        [SerializeField, Header("着地地点のベクター")] private Vector3 pos_;
        [SerializeField, Header("ジャンプするスピード")] private float speed_;
        [SerializeField, Header("ジャンプする回数")] private int count_;
        [SerializeField, Header("ジャンプする時間")] private float time_;
        [SerializeField, Header("ジャンプする種類")] private Ease ease_;
        void Start() {
            transform.DOJump(pos_, speed_, count_, time_).SetEase(ease_).OnComplete(() =>
            {
               GameObject.Find("EventSystem").GetComponent<TitleEventManager>().ActiveMainCanvas();
            });
        }
    }
}