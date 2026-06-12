using UnityEngine;
using UnityEngine.Events;

namespace Kounosuke
{
    public class TriggerEventBase : MonoBehaviour
    {
        [SerializeField, Header("タグ")] protected string TagName_;
        [SerializeField, Header("衝突後後処理関数 (メイン処理後の後処理用)")] protected UnityEvent CallBack_;

        virtual protected void Start()
        {
            if (TagName_ == null) {
                Debug.LogError("Not Set TagName_");
            }
        }

        protected virtual void CollitionEvent()
        {
            Debug.Log("trigger tag");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(TagName_))
            {
                CollitionEvent();
                CallBack_?.Invoke();
            }
        }
    }
}
