using Kounosuke;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kounosuke
{
    public interface CoinInterface : IEventSystemHandler
    {
        void OnStart(GameManager game);
        void OnReset();
    }
    public class TarggerCoinScripts : TriggerEventBase, CoinInterface
    {
        private GameManager gameManager;
        [SerializeField, InspectorName("スコアアップ量")] int score = 100;
        [SerializeField, InspectorName("コインの見た目オブジェクト")] GameObject coinBody;
        Collider2D coinBodyCollider;

        protected override void Start()
        {
            coinBodyCollider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// ゲームマネージャーからのScene初期化関数
        /// </summary>
        /// <param name="manager"></param>
        public void OnStart(GameManager game) {
            gameManager = game;
            coinBody.SetActive(true);
        }

        /// <summary>
        /// 衝突時処理関数
        /// </summary>
        protected override void CollitionEvent() {
            gameManager.ScoreUp(score);
            gameManager.GetCoin();
            coinBody.SetActive(false);
            coinBodyCollider.enabled = false;
        }

        /// <summary>
        /// コインリセット関数
        /// </summary>
        public void OnReset() {
            if (!coinBody.activeSelf) {
                coinBody.SetActive(true);
                coinBodyCollider.enabled = true;
            }
        }
    }
}