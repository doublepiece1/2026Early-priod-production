using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Kounosuke
{
    public class GameUIManager : GimmickBase
    {
        [Header("UI")]

        [SerializeField] private TextMeshProUGUI TimeText;
        [SerializeField] private Image[] Herts;

        [Header("Image")]
        [SerializeField] private Sprite RadHert;
        [SerializeField] private Sprite BlackHert;

        private int hert_value = 0;
        GameManager manager;
        PlayerRespon respon;

        private void Start()
        {
            manager = gameObject.GetComponent<GameManager>();
            respon = FindFirstObjectByType<PlayerRespon>();
        }
        public override void OnReset()
        {
            foreach(var item in Herts)
            {
                item.sprite = RadHert;
            }
        }

        private void FixedUpdate()
        {
            TimeText.text = $"Time : {manager.timer:0.0}";
            ChengeHert(respon.Hp);
        }

        void ChengeHert(int value)
        {
            if (hert_value == value) {
                return;
            }
            else if (value <= 0|| Herts.Length <= value)
            {
                return;
            }
            
            hert_value = value;
            Herts[hert_value].sprite = BlackHert;
        }
    }
}