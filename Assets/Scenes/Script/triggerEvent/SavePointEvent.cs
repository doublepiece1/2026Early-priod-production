using UnityEngine;

namespace Kounosuke
{
    public class SavePointEvent : TriggerEventBase
    {
        [SerializeField] private ChangeImage ChangeImage;
        private SpriteRenderer SpriteRenderer;

        public override void OnStart()
        {
            base.OnStart();
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            SpriteRenderer.sprite = ChangeImage.GetBeginImage();
        }
        protected override void CollitionEvent()
        {
            SpriteRenderer.sprite = ChangeImage.GetEndImage();
        }
    }
}