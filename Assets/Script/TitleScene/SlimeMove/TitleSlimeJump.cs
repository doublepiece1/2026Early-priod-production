using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Kounosuke
{
    public class TitleSlimeJump : MonoBehaviour
    {
        [SerializeField, Header("ジャンプするベクター")] private Vector3 vec_;
        private Animator animator_;
        private Rigidbody rb_;
        private void Start()
        {
            animator_ = GetComponent<Animator>();
            rb_ = GetComponent<Rigidbody>();
            vec_ += transform.position;
            animator_.SetTrigger("IsJump");
            
        }

        public void OnMessage(int value)
        {
            Debug.Log("Jump");
            animator_.SetBool("IsGround", false);
            transform.DOMove(vec_, 1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { animator_.SetBool("IsGround", true); });
        }
    }

}