using UnityEngine;
namespace Kounosuke {
    public class GameSceneMoveCamera : MonoBehaviour
    {
        [SerializeField, Header("メインカメラ")] private Camera main_camera;
        [SerializeField, Header("X軸オフセット")] private float camera_offsetx = 3;
        [SerializeField, Header("Y軸オフセット")] private Vector2 camera_offsety = new Vector2(3, 3);
        [SerializeField, Header("最低値")] private float min_y = -2f;
        [SerializeField, Header("追従ターゲット")] private GameObject target_;

        void Update()
        {
            float lengthX = target_.transform.position.x - main_camera.transform.position.x;
            float abs_lengthX = Mathf.Abs(lengthX);

            var move_vec = new Vector3();
            if (abs_lengthX > camera_offsetx)
            {
                float f = lengthX > 0 ? 1 : -1;
                move_vec += new Vector3((abs_lengthX - camera_offsetx) * f,0,0);
            }

            float lengthY = target_.transform.position.y -main_camera.transform.position.y;

            if (lengthY > camera_offsety.x)
            {
                move_vec.y += lengthY - camera_offsety.x;
            }
            else if (lengthY < -camera_offsety.y)
            {
                move_vec.y += lengthY + camera_offsety.y;
            }

            Vector3 nextPos = Vector3.Lerp(transform.position, transform.position + move_vec, 5f * Time.deltaTime);
            nextPos.y = Mathf.Max(nextPos.y, min_y);

            transform.position = nextPos;
        }
    }
}
