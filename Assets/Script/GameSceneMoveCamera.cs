using UnityEngine;
namespace Kounosuke {
    public class GameSceneMoveCamera : MonoBehaviour
    {
        [SerializeField, Header("メインカメラ")] private Camera main_camera;
        [SerializeField, Header("X軸オフセット")] private float camera_offsetx = 3;
        [SerializeField, Header("Y軸オフセット")] private float camera_offsety = 3; 
        [SerializeField, Header("追従ターゲット")] private GameObject target_;

        void Update()
        {
            float lengthX = target_.transform.position.x - main_camera.transform.position.x;
            float abs_lengthX = Mathf.Abs(lengthX);
           
            if (abs_lengthX > camera_offsetx)
            {
                float f = lengthX > 0 ? 1 : -1;
                var move_vec = new Vector3((abs_lengthX - camera_offsetx) * f,0,0);
                transform.Translate(move_vec);
            }

            float legthY = target_.transform.position.y -main_camera.transform.position.y;
            float abs_lengthY = Mathf.Abs(legthY);

            if (abs_lengthY > camera_offsety)
            {
                float f = legthY > 0 ? 1 :-1;
                var move_vec = new Vector3(0, (abs_lengthY - camera_offsety) * f, 0);
                transform.Translate(move_vec);
            }
            
        }
    }
}
