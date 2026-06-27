using UnityEngine;

namespace Kounosuke
{
    /// <summary>
    /// グラップルロープ描画専用
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class GrappleRopeRenderer : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            Hide();
        }

        /// <summary>
        /// 発射中のロープ描画
        /// </summary>
        public void DrawShot(
            Vector2 start,
            Vector2 direction,
            float length)
        {
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(
                1,
                start + direction * length
            );
        }

        /// <summary>
        /// 接続済みロープ描画
        /// </summary>
        public void DrawConnected(
            Vector2 start,
            Vector2 grapplePoint)
        {
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, grapplePoint);
        }

        /// <summary>
        /// ロープ非表示
        /// </summary>
        public void Hide()
        {
            lineRenderer.positionCount = 0;
        }
    }
}