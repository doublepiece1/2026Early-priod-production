using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kounosuke
{
    public class GoalFlagEvent : MonoBehaviour
    {
        [Header("ゴール時出現イメージ")]
        [SerializeField] private GameObject clearImage;

        private void Start() {
            clearImage.SetActive(false);
        }


        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) {
                clearImage.SetActive(true);

                //clearImage.transform.DOMove();
            }
        }
    }
}
