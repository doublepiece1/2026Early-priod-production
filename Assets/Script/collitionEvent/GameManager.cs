using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Kounosuke
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField, Header("スコア")] private int Score_ = 0;
        [SerializeField, Header("FadeObj")] private Fade fade_obj;
        void Start()
        {
            Reset_Game();
            wait_start();
        }
        async UniTask wait_start()
        {
            await Wait(1000);
            Fade(false);
        }
        void Reset_Game() {
            Score_ = 0;
        }


        public async UniTask PlayerDeath(Material fade_material)
        {
            Fade(true, fade_material);
            await Wait(3000);
            Fade(false);
        }

        public void GoalEvent()
        {
            Wait(2000);
            Debug.Log("Goal！！！！！！！");
        }
        async UniTask Wait(int value)
        {
            await UniTask.Delay(value);
            Debug.Log("End Wait");
        }

        public void ScoreUp(int value)
        {
            Score_ += value;
            Debug.Log(Score_);
        }

        private void Fade(bool fade, Material fade_material = null)
        {
            if (fade_material != null)
            {
                fade_obj.SetFadeMaterial(fade_material);
            }
            fade_obj.FadeImage(fade);
            //ここでプレイヤーの操作を変更する処理を書く
        }
    }

}