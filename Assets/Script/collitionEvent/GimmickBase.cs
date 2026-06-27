using UnityEngine;
using UnityEngine.EventSystems;

namespace Kounosuke
{
    //受信関数
    public interface GimmickInterface : IEventSystemHandler {
        void OnStart();
        void OnReset();
    }

    //ゲームシーンギミック基底関数
    public class GimmickBase : MonoBehaviour, GimmickInterface
    {
        public virtual void OnStart()
        {

        }
        public virtual void OnReset()
        {

        }
    }
}