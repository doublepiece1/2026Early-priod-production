
using UnityEngine;

namespace Kounosuke
{
    public class HisaAnimationController : MonoBehaviour
    {
        [SerializeField, Header("アニメーター")] private Animator m_Animator;
        [SerializeField, Header("リジットボディ")] Rigidbody rb;

        void Update()
        {
            if (rb != null) {
                Debug.Log("Not Found rb");
                return;
            }

            int a = (int)rb.linearVelocity.y;
            m_Animator.SetInteger("yVelocity", a);
        }
    }
}