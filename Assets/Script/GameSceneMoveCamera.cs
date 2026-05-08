using UnityEngine;
namespace Kounosuke {
    public class GameSceneMoveCamera : MonoBehaviour
    {
        [SerializeField, Header("メインカメラ")] private Camera main_camera;
        [SerializeField, Header("オフセット")] private float camera_offset = 3;
        [SerializeField, Header("追従ターゲット")] private GameObject target_;

        void Update()
        {
            var length = target_.transform.position.x - main_camera.transform.position.x;
            var abs_length = Mathf.Abs(length);
            if (abs_length > camera_offset)
            {
                var f = length > 0 ? 1 : -1;
                var move_vec = new Vector3((abs_length - camera_offset) * f,0,0);
                transform.Translate(move_vec);
            }
        }
    }
}
