using DG.Tweening;
using UnityEngine;

namespace Kounosuke
{
    public class DoTween_liner : MonoBehaviour
    {
        [SerializeField, Header("")] private Vector3 move_vec = new Vector3();
        [SerializeField, Header("")] private float time_ = 1;
        [SerializeField, Header("")] private int loop_count = -1;
        [SerializeField, Header("")] private LoopType loop_mode = LoopType.Yoyo;
        Vector3 start_pos;
        private void Start()
        {
            start_pos = transform.position;
            this.transform.DOMove(start_pos + move_vec, time_).SetLoops(loop_count, loop_mode);
        }
    }
}