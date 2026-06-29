using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kounosuke
{
    public class GameManager : MonoBehaviour, IEventSystemHandler
    {
        //==================================================
        // ■ スコア
        //==================================================
        [Header("ゲーム内スコア")]
        [SerializeField] private int score = 0;
        [SerializeField] private int maxCoinCount = 0;
        [SerializeField] private int coinCount = 0; //エサ

        //==================================================
        // ■ タイマー
        //==================================================
        [Header("ゲームタイマー")]
        public float timer {  get;private set; }
        [SerializeField] private float sceneTime;

        private bool isTimerActive = true;

        //==================================================
        // ■ 演出
        //==================================================
        [Header("ゲーム演出")]
        [SerializeField] private Fade fadeObj;
        [SerializeField] private AudioClip BGM;
        private bool isProcessing = false;

        //==================================================
        // ■ キャッシュ
        //==================================================
        private TarggerCoinScripts[] coins;
        private GimmickBase[] gimmicks;

        //==================================================
        // ■ Unity
        //==================================================
        private void Start()
        {
            InitializeGame();
            StartGameFlow().Forget();
        }

        private void Update()
        {
            UpdateTimer();
        }

        //==================================================
        // ■ 初期化
        //==================================================
        private void InitializeGame()
        {
            timer = sceneTime;

            coins = FindObjectsByType<TarggerCoinScripts>(FindObjectsSortMode.None);
            gimmicks = FindObjectsByType<GimmickBase>(FindObjectsSortMode.None);

            maxCoinCount = coins.Length;
            coinCount = 0;
            score = 0;

            foreach (var coin in coins)
                coin.OnStart(this);

            foreach (var gimmick in gimmicks)
                gimmick.OnStart();

            AudioManager.Instance().PlayBGM(BGM);
        }

        //==================================================
        // ■ タイマー
        //==================================================
        private void UpdateTimer()
        {
            if (!isTimerActive || isProcessing)
                return;

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                isTimerActive = false;
                
                return;
            }
        }

        //==================================================
        // ■ ゲームフロー
        //==================================================
        private async UniTask StartGameFlow()
        {
            Debug.Log("Start");
            await UniTask.Delay(1000);
            await FadeAsync(false);
        }

        public async UniTask PlayerDeath(Material fadeMaterial = null)
        {
            if (isProcessing)
                return;

            isProcessing = true;

            await FadeAsync(true, fadeMaterial);
            await UniTask.Delay(3000);

            ResetGame();

            await FadeAsync(false);

            timer -= 15;

            isProcessing = false;
        }

        public void GoalEvent()
        {
            if (isProcessing)
                return;

            GoalFlow().Forget();
        }

        private async UniTask GoalFlow()
        {
            isProcessing = true;

            Debug.Log("Goal！！！！！！！");

            await UniTask.Delay(2000);

            await FadeAsync(true);

            ResetGame();

            await FadeAsync(false);

            isProcessing = false;
        }

        //==================================================
        // ■ リセット
        //==================================================
        private void ResetGame()
        {
            Debug.Log("GameReset");

            score = 0;
            //foreach (var coin in coins)
            //    coin.OnReset();

            foreach (var gimmick in gimmicks)
                gimmick.OnReset();

            timer = sceneTime;
            isTimerActive = true;
        }

        //==================================================
        // ■ コイン・スコア
        //==================================================
        public void GetCoin()
        {
            coinCount++;
        }

        public void ScoreUp(int value)
        {
            score += value;
        }

        //==================================================
        // ■ Fade
        //==================================================
        private async UniTask FadeAsync(bool fadeIn, Material material = null)
        {
            if (material != null)
                fadeObj.SetFadeMaterial(material);

            fadeObj.FadeImage(fadeIn);

            await UniTask.Delay(3000);
        }
    }
}