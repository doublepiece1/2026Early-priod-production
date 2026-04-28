using System;
using UnityEngine;

namespace Kounosuke
{
    public class TitleSettingButton : TitleButtonBase
    {
        [SerializeField,Header("イベントマネージャー")]private TitleEventManager eventManager_;
        protected override void OnClickSettingButton()
        {
            base.OnClickSettingButton();
            if (eventManager_ != null)
            {
                eventManager_.ActiveSubCanvas();
            }
        }
    }
}