using UnityEngine;
using DG.Tweening;

namespace Kounosuke {
    public class TitleMoveSlime : MonoBehaviour
    {
        [SerializeField, Header("動かしたいオブジェクト")] protected GameObject Object_;

        protected static bool HasCheck(GameObject game) { return game != null; }

        void Start()
        {
           
        }

        virtual public void Push_Sequence()
        {

        }
    }
}