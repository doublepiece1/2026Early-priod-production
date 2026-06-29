using UnityEngine;

namespace Kounosuke
{
    public class TitleScrollbar : MonoBehaviour
    {
        [SerializeField, Header("このボタンを使用するか")] private bool IsUsed_ = true;
        [SerializeField, Header("マウス選択時の色")] private Color HighlightedColor_ = Color.white;
        [SerializeField, Header("パッド選択時の色")] private Color SelectedColor_ = Color.white;

        private void Awake()
        {
            if (!IsUsed_)
            {
                this.gameObject.SetActive(false);
                return;
            }
            var scrollbar = GetComponent<UnityEngine.UI.Scrollbar>();
            if (scrollbar != null)
            {
                scrollbar.colors = new UnityEngine.UI.ColorBlock()
                {
                    normalColor = scrollbar.colors.normalColor,
                    highlightedColor = HighlightedColor_,
                    pressedColor = scrollbar.colors.pressedColor,
                    selectedColor = SelectedColor_,
                    disabledColor = scrollbar.colors.disabledColor,
                    colorMultiplier = scrollbar.colors.colorMultiplier,
                    fadeDuration = scrollbar.colors.fadeDuration
                };
            }
        }
    }
}