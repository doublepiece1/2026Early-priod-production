using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kounosuke
{
    public class TitleStartButton : TitleButtonBase
    {
        [SerializeField, Header("遷移するシーン名")] private string Scene_Name;
        protected override void OnClickSettingButton()
        {
            base.OnClickSettingButton();
            Debug.Log("スタートボタンが押されました");
            if (Scene_Name != null) { 
                SceneManager.LoadScene(Scene_Name);
            }
        }
    }
}