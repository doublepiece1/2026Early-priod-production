using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kounosuke
{
    public class TitleStartButton : TitleButtonBase
    {
        public string Title;
        protected override void OnClickSettingButton()
        {
            base.OnClickSettingButton();
            if (Title != null)
            {
                SceneManager.LoadScene(Title);
            }
        }
    }
}