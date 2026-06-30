using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Kounosuke
{
    public class TitleSlimeJump : MonoBehaviour
    {
        [SerializeField, Header("ジャンプするベクター")] private Vector3 vec_;
        [SerializeField, Header("ジャンプする時間")] private float time_ = 1;
        [SerializeField, Header("アニメーター")] private Animator animator_;
        [SerializeField, Header("停止時間")] private float stopTime_;
        private Rigidbody rb_;
        private void Start()
        {
            rb_ = GetComponent<Rigidbody>();
            vec_ += transform.position;
            if(animator_ != null) {
                transform.DOMove(transform.position, stopTime_).OnComplete(() => { animator_.SetTrigger("IsJump"); });   
            }
        }

        public void OnMessage(int value)
        {
            
            transform.DOMove(vec_, time_).SetLoops(2, LoopType.Yoyo).OnStart(() =>
            {
                if (animator_ != null)
                {
                    animator_.SetBool("IsGround", false);
                }
            }).OnComplete(() => 
            {
                if (animator_ != null)
                {
                    animator_.SetBool("IsGround", true);
                }
            });
        }
    }

}