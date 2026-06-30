using UnityEngine;
namespace Kounosuke
{
    public class GameSceneMoveCamera : MonoBehaviour
    {
        [System.Serializable]
        public class CameraZoomPoint
        {
            [Header("このX座標")]
            public float x;

            [Header("この地点のカメラサイズ")]
            public float size;
        }

        [Header("メインカメラ")]
        [SerializeField] private Camera main_camera;

        [Header("追従ターゲット")]
        [SerializeField] private Transform target_;

        [Header("X軸オフセット")]
        [SerializeField] private float camera_offsetx = 3f;

        [Header("追加オフセット")]
        [SerializeField] private float add_camera_offsetx = 3f;

        [Header("Y軸オフセット")]
        [SerializeField] private Vector2 camera_offsety = new Vector2(3, 3);

        [Header("最低Y座標")]
        [SerializeField] private float min_y = -2f;

        [Header("ズーム速度")]
        [SerializeField] private float zoomSpeed = 3f;

        [Header("ズームポイント（X順に並べる）")]
        [SerializeField] private CameraZoomPoint[] zoomPoints;

        private void Update()
        {
            UpdateCameraSize();
            FollowTarget();
        }

        /// <summary>
        /// カメラサイズ更新
        /// </summary>
        private void UpdateCameraSize()
        {
            if (zoomPoints == null || zoomPoints.Length == 0)
                return;

            float playerX = target_.position.x;

            float targetSize = GetCameraSize(playerX);

            main_camera.orthographicSize = Mathf.Lerp(
                main_camera.orthographicSize,
                targetSize,
                zoomSpeed * Time.deltaTime);
        }

        /// <summary>
        /// プレイヤー追従
        /// </summary>
        private void FollowTarget()
        {
            float lengthX = target_.position.x - main_camera.transform.position.x+ add_camera_offsetx;
            float abs_lengthX = Mathf.Abs(lengthX);

            Vector3 move_vec = Vector3.zero;

            if (abs_lengthX > camera_offsetx)
            {
                move_vec.x = (abs_lengthX - camera_offsetx) * Mathf.Sign(lengthX);
            }

            float lengthY = target_.position.y - main_camera.transform.position.y;

            if (lengthY > camera_offsety.x)
            {
                move_vec.y = lengthY - camera_offsety.x;
            }
            else if (lengthY < -camera_offsety.y)
            {
                move_vec.y = lengthY + camera_offsety.y;
            }

            Vector3 nextPos = Vector3.Lerp(
                transform.position,
                transform.position + move_vec,
                5f * Time.deltaTime);

            nextPos.y = Mathf.Max(nextPos.y, min_y);

            transform.position = nextPos;
        }

        /// <summary>
        /// プレイヤーX座標からカメラサイズを取得
        /// </summary>
        private float GetCameraSize(float playerX)
        {
            // 最初より左
            if (playerX <= zoomPoints[0].x)
                return zoomPoints[0].size;

            // 最後より右
            if (playerX >= zoomPoints[zoomPoints.Length - 1].x)
                return zoomPoints[zoomPoints.Length - 1].size;

            // 補間
            for (int i = 0; i < zoomPoints.Length - 1; i++)
            {
                CameraZoomPoint a = zoomPoints[i];
                CameraZoomPoint b = zoomPoints[i + 1];

                if (playerX >= a.x && playerX <= b.x)
                {
                    float t = Mathf.InverseLerp(a.x, b.x, playerX);
                    return Mathf.Lerp(a.size, b.size, t);
                }
            }

            return main_camera.orthographicSize;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (zoomPoints == null)
                return;

            Gizmos.color = Color.cyan;

            foreach (var p in zoomPoints)
            {
                Gizmos.DrawLine(
                    new Vector3(p.x, -100, 0),
                    new Vector3(p.x, 100, 0));
            }
        }
        private void OnValidate()
        {
            if (zoomPoints == null) return;

            System.Array.Sort(zoomPoints, (a, b) => a.x.CompareTo(b.x));
        }
#endif
    }
}