using UnityEngine.Events;

namespace Kounosuke
{
    public class TitleStartButton : TitleButtonBase
    {
        public UnityEvent event_;
        protected override void OnClickSettingButton()
        {
            base.OnClickSettingButton();
            event_?.Invoke();
            gameObject.SetActive(false);
        }
    }
}