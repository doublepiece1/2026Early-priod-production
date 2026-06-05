using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

namespace Kounosuke
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField, Header("スコア")] private int Score_ = 0;
        [SerializeField, Header("FadeObj")] private Fade fade_obj;
        [SerializeField, Header("")] private List<GameObject> GimmickObj = new List<GameObject>();
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
            foreach(var item in GimmickObj) {
                ExecuteEvents.Execute<GimmickInterface>(item, null, (handler, eventData) => handler.OnReset());
            }
        }

        public async UniTask PlayerDeath(Material fade_material)
        {
            Fade(true, fade_material);
            await Wait(3000);
            Reset_Game();
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
        public async UniTask CallFade(Material fade_material = null)
        {
            Fade(true, fade_material);
            await Wait(3000);
            Reset_Game();
            Fade(false);
        }
        void Fade(bool fade, Material fade_material = null)
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