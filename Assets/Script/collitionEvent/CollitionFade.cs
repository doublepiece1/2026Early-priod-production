using UnityEngine;

namespace Kounosuke
{
    public class CollitionFade : ColitionEventBase
    {
        [SerializeField,Header("ゲームマネージャー")]private GameManager gameManager;
        [SerializeField, Header("フェードするマテリアル")] private Material material;
        protected override void CollitionEvent()
        {
            Debug.Log("Fade action");
            gameManager.CallFade(material);
        }
    }
}