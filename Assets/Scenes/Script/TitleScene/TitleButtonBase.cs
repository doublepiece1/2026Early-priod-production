using UnityEngine;

namespace Kounosuke
{
    public class TitleButtonBase : MonoBehaviour
    {
        [SerializeField, Header("このボタンを使用するか")] private bool IsUsed_ = true;
        [SerializeField, Header("マウス選択時の色")] private Color HighlightedColor_ = Color.white;
        [SerializeField, Header("パッド選択時の色")] private Color SelectedColor_ = Color.white;
        protected virtual void Awake() {
            if (!IsUsed_) {
                this.gameObject.SetActive(false);
                return;
            }
            var button = GetComponent<UnityEngine.UI.Button>();
            if (button != null) {
                button.onClick.AddListener(OnClickSettingButton);
                button.colors = new UnityEngine.UI.ColorBlock() {
                    normalColor = button.colors.normalColor,
                    highlightedColor = HighlightedColor_,
                    pressedColor = button.colors.pressedColor,
                    selectedColor = SelectedColor_,
                    disabledColor = button.colors.disabledColor,
                    colorMultiplier = button.colors.colorMultiplier,
                    fadeDuration = button.colors.fadeDuration
                };
            }
        }

        protected virtual void OnClickSettingButton() {
            if (!IsUsed_) {
                return;
            }
        }
    }
}