using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kounosuke
{
    /// <summary>
    /// シーン遷移制御用クラス
    /// </summary>
    public class SceneFlowManager : MonoBehaviour
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        static SceneFlowManager instance;

        /// <summary>
        /// インスタンス生成関数
        /// </summary>
        private void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// シングルトンインスタンス取得関数
        /// </summary>
        public static SceneFlowManager Instance() {
            return instance;
        }

        /// <summary>
        /// タイトルシーン移動関数
        /// </summary>
        public void MoveTitleScene() {
            SceneManager.LoadScene("TitleScene");
        }

        /// <summary>
        /// ゲームシーン移動関数
        /// </summary>
        public void MoveGameScene() {
            SceneManager.LoadScene("PracticeScene");
        }

        /// <summary>
        /// 選択シーン移動
        /// </summary>
        public void MoveSelectScene() {
            SceneManager.LoadScene("SelectStageScene");
        }
    }
}