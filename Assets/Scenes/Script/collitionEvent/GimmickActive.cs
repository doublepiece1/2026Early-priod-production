using UnityEngine;
using UnityEngine.Events;

namespace Kounosuke {
    /// <summary>
    /// ギミックを使用する際にアクティブ状態を切り替えれるように親としてくっつけるスクリプト
    /// </summary>
    public class GimmickActive : GimmickBase
    {
        [SerializeField, Header("ギミックのリセットイベント")] private UnityEvent ResetObj;
        [SerializeField, Header("ギミックオブジェクト")] private GameObject GimmickObj;
        
        public override void OnReset()
        {
            if (!GimmickObj.activeSelf) {
                GimmickObj.SetActive(true);
            }
            ResetObj?.Invoke();
        }
    }
}