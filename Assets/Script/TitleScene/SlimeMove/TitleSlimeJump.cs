using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Kounosuke
{
    public class TitleSlimeJump : MonoBehaviour
    {
        [SerializeField, Header("ジャンプするベクター")] private Vector3 vec_;
        [SerializeField, Header("アニメーター")] private Animator animator_;
        private Rigidbody rb_;
        private void Start()
        {
            rb_ = GetComponent<Rigidbody>();
            vec_ += transform.position;
            if(animator_ != null) {
                animator_.SetTrigger("IsJump");
            }
        }

        public void OnMessage(int value)
        {
            if (animator_ != null) {
                animator_.SetBool("IsGround", false);
            }
            transform.DOMove(vec_, 1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                if (animator_ != null)
                {
                    animator_.SetBool("IsGround", true);
                }
            });
        }
    }

}