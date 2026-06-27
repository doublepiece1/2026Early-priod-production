using UnityEngine;
using UnityEngine.Events;

namespace Kounosuke
{
    public class TriggerEventBase : GimmickBase
    {
        [SerializeField, Header("タグ")] protected string TagName_;
        [SerializeField, Header("衝突後後処理関数 (メイン処理後の後処理用)")] protected UnityEvent CallBack_;
        private bool is_tach = false;

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

        public override void OnStart()
        {
            is_tach = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (is_tach)
            {
                return;
            }
            if (collision.gameObject.CompareTag(TagName_))
            {
                CollitionEvent();
                CallBack_?.Invoke();
                is_tach = true;
            }
        }
    }
}
