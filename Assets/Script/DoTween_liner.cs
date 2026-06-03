using DG.Tweening;
using UnityEngine;

namespace Kounosuke
{
    public class DoTween_liner : MonoBehaviour
    {
        [SerializeField, Header("移動するベクトル")] private Vector3 move_vec = new Vector3();
        [SerializeField, Header("一回の移動時間")] private float time_ = 1;
        [SerializeField, Header("ループ回数")] private int loop_count = -1;
        [SerializeField, Header("ループモード")] private LoopType loop_mode = LoopType.Yoyo;
        [SerializeField, Header("移動方法")] private Ease ease_mode = Ease.Linear;
        Vector3 start_pos;
        private void Start()
        {
            start_pos = transform.position;
            this.transform.DOMove(start_pos + move_vec, time_).SetLoops(loop_count, loop_mode).SetEase(Ease.Linear);
        }
    }
}