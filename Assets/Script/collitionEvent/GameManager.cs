using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Kounosuke
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField, Header("スコア")] private int Score_ = 0;
        void Start()
        {
            Reset_Game();
        }

        void Reset_Game()
        {
            Score_ = 0;
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
    }

}