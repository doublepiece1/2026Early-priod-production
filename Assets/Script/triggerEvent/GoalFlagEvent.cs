using DG.Tweening;
using UnityEngine;

namespace Kounosuke
{
    public class GoalFlagEvent : MonoBehaviour
    {
        [Header("ゴール時出現イメージ")]
        [SerializeField] private GameObject clearImage;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Camera camera;

        private void Start() {
            clearImage.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player"))
            {
                clearImage.SetActive(true);

                clearImage.transform.DOMove(transform.position, 3).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    //  ゴール演出終了時に、ゲームマネージャーのゴール処理を呼び出す
                    gameManager.GoalEvent();
                });
            }
        }
    }
}
