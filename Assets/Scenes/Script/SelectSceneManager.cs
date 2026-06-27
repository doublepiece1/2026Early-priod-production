using Cysharp.Threading.Tasks;
using Unity.AppUI.UI;
using UnityEngine;

namespace Kounosuke
{
    public class SelectSceneManager : MonoBehaviour
    {
        [Header("ボタン")]
        [SerializeField, Tooltip("")] private Button Stage1Button;

        [SerializeField, InspectorName("FadeObj")] private Fade fade_obj;

        public void ChangeScnene(string sceneName)
        {
            MoveScene().Forget();
        }

        async UniTask MoveScene()
        {
            Fade(true);
            await UniTask.Delay(3000);
            SceneFlowManager.Instance().MoveGameScene();

        }
        public void Fade(bool fade) {
            
            fade_obj.FadeImage(fade);
            //ここでプレイヤーの操作を変更する処理を書く
        }
    }
}
