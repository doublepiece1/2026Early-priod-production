using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace Kounosuke
{
    public class GameManager : MonoBehaviour, IEventSystemHandler
    {
        [Header("ゲーム内スコア")]
        [SerializeField, Tooltip("スコア")] private int Score_ = 0;
        [SerializeField, Tooltip("Scene内コイン数 (ゲームSceneスタート時自動カウント)")] private int maxCoinCount = 0;
        [SerializeField, Tooltip("取得したコイン数")] private int coinCount = 0;

        [Header("ゲーム演出")]
        [SerializeField, InspectorName("FadeObj")] private Fade fade_obj;
        [SerializeField, Tooltip("残り時間タイマー")] private float timer;
        [SerializeField, Tooltip("ゲームシーンことのタイマー時間")] private float sceneTime;

        [Header("ゲームUI")]
        [SerializeField, Tooltip("タイマーテキスト")] private TextMeshProUGUI timerText;

        [Header("ギミック")]
        [SerializeField] private int a;

        /// <summary>
        /// Sceneスタート関数
        /// </summary>
        void Start()
        {
            timer = sceneTime;
            maxCoinCount = 0;

            //  コイン全取得
            TarggerCoinScripts[] coins = FindObjectsByType<TarggerCoinScripts>(FindObjectsSortMode.None);
            foreach (var coin in coins) {
                coin.OnStart(this);
                maxCoinCount++;
            }

            //  ギミックスタート
            GimmickBase[] gimmicks = FindObjectsByType<GimmickBase>(FindObjectsSortMode.None);
            foreach (var gimmick in gimmicks) {
                gimmick.OnStart();
            }

            wait_start().Forget();
        }

        /// <summary>
        /// スタートフェード制御関数
        /// </summary>
        /// <returns></returns>
        async UniTask wait_start()
        {
            await Wait(1000);
            Fade(false);
        }

        /// <summary>
        /// タイマー用みたいになってる UpDate()
        /// </summary>
        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0) {
                timer = sceneTime;
                Debug.Log("タイマー終了");
            }
            else {
                //  タイマーテキスト変更するかも
                timerText.text = "Time : " + timer.ToString("f1");
            }
        }


        /// <summary>
        /// ゲームリセット関数
        /// </summary>
        void Reset_Game() {
            Debug.Log("GameReset");

            Score_ = 0;            
            coinCount = 0;

            //  コインリセット
            TarggerCoinScripts[] coins = FindObjectsByType<TarggerCoinScripts>(FindObjectsSortMode.None);
            foreach (var coin in coins) {
                coin.OnReset();
            }

            //  ギミックリセット
            GimmickBase[] gimmicks = FindObjectsByType<GimmickBase>(FindObjectsSortMode.None);
            foreach (var gimmick in gimmicks) {
                gimmick.OnReset();
            }
        }

        /// <summary>
        /// プレイヤー死亡時処理関数
        /// </summary>
        /// <param name="fade_material"></param>
        /// <returns></returns>
        public async UniTask PlayerDeath(Material fade_material = null)
        {
            Fade(true, fade_material);
            await Wait(3000);
            Reset_Game();
            Fade(false);
        }

        /// <summary>
        /// ゴール関数
        /// </summary>
        public void GoalEvent() {
            Wait(2000).Forget();
            Debug.Log("Goal！！！！！！！");
            CallFade().Forget();
            //  ゴール時の遷移処理を書く
        }

        /// <summary>
        /// 時間遅延関数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        async UniTask Wait(int value)
        {
            await UniTask.Delay(value);
            Debug.Log("End Wait");
        }

        /// <summary>
        /// コイン取得関数
        /// </summary>
        public void GetCoin() {
            coinCount++;
        }

        /// <summary>
        /// スコアアップ関数    (コイン以外も含む)
        /// </summary>
        /// <param name="value"></param>
        public void ScoreUp(int value)
        {
            Score_ += value;
            Debug.Log(Score_);
        }

        /// <summary>
        /// 一連のFadeアクション用意関数
        /// </summary>
        /// <param name="fade_material"></param>
        /// <returns></returns>
        public async UniTask CallFade(Material fade_material = null)
        {
            Fade(true, fade_material);
            await Wait(3000);
            Reset_Game();
            Fade(false);
        }

        /// <summary>
        /// Fade制御関数
        /// </summary>
        /// <param name="fade"></param>
        /// <param name="fade_material"></param>
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