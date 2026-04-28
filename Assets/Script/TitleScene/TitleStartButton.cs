using UnityEngine;

namespace Kounosuke
{
    public class TitleStartButton : TitleButtonBase
    {
        protected override void OnClickSettingButton()
        {
            base.OnClickSettingButton();
            Debug.Log("スタートボタンが押されました");
        }
    }
}