using UnityEngine;

namespace Kounosuke
{
    /// <summary>
    /// 吸引＋チャージ演出付きVFX制御
    /// </summary>
    public class EfectBase : MonoBehaviour
    {
        //==================================================
        // ■ 吸引対象
        //==================================================
        [Header("吸引対象")]
        public Transform center;

        //==================================================
        // ■ 吸引パラメータ
        //==================================================
        [Header("吸引設定")]
        [SerializeField] private float baseSpeed = 2f;
        [SerializeField] private float maxSpeed = 10f;

        [Range(0f, 1f)]
        public float intensity = 0f;

        //==================================================
        // ■ チャージ演出
        //==================================================
        [Header("チャージ演出")]
        [SerializeField] private bool chargeMode = false;
        [SerializeField] private float pulseSpeed = 5f;

        //==================================================
        // ■ 内部データ
        //==================================================
        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;
        private bool isActive = false;

        private float pulse;

        //==================================================
        // ■ Unity Lifecycle
        //==================================================

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();

            int max = Mathf.Max(1, ps.main.maxParticles);
            particles = new ParticleSystem.Particle[max];
        }

        private void LateUpdate()
        {
            if (center == null || !isActive)
                return;

            int count = ps.GetParticles(particles);

            float speed = CalculateSpeed();
            ApplyParticles(count, speed);

            ps.SetParticles(particles, count);
        }

        //==================================================
        // ■ 計算系
        //==================================================

        /// <summary>
        /// 現在の速度を計算（通常 or チャージ）
        /// </summary>
        private float CalculateSpeed()
        {
            if (chargeMode)
            {
                pulse += Time.deltaTime * pulseSpeed;
                float t = Mathf.Sin(pulse) * 0.5f + 0.5f;
                return Mathf.Lerp(baseSpeed, maxSpeed, t);
            }

            return Mathf.Lerp(baseSpeed, maxSpeed, intensity);
        }

        //==================================================
        // ■ 適用処理
        //==================================================

        /// <summary>
        /// パーティクルに力を適用
        /// </summary>
        private void ApplyParticles(int count, float speed)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = GetDirection(i);

                float seed = particles[i].randomSeed % 1000 * 0.001f;

                float finalSpeed = speed * Mathf.Lerp(0.7f, 1.3f, seed);

                particles[i].velocity = dir * finalSpeed;
            }
        }

        /// <summary>
        /// 吸引＋渦方向ベクトル取得
        /// </summary>
        private Vector3 GetDirection(int i)
        {
            Vector3 dir = (center.position - particles[i].position).normalized;

            Vector3 tangent = Vector3.Cross(dir, Vector3.forward).normalized;

            // 粒子ごとのズレ（重要）
            float seed = particles[i].randomSeed % 1000 * 0.001f;

            // 時間＋個体差で渦を作る
            float swirl = Mathf.Sin(Time.time * 3f + seed * 10f);

            return (dir + tangent * swirl * 0.6f).normalized;
        }

        //==================================================
        // ■ 外部制御
        //==================================================

        /// <summary>
        /// チャージモード切替
        /// </summary>
        public void SetChargeMode(bool enable)
        {
            chargeMode = enable;

            if (enable)
            {
                isActive = true;
                pulse = 0f;
            }
            else
            {
                isActive = false;
                pulse = 0f;

                ClearParticles();
            }
        }

        private void ClearParticles()
        {
            int count = ps.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                particles[i].velocity = Vector3.zero;
            }

            ps.SetParticles(particles, count);
        }

        void SetActive(bool enable)
        {
            isActive = enable;

            if (!enable)
            {
                int count = ps.GetParticles(particles);

                for (int i = 0; i < count; i++)
                {
                    particles[i].velocity = Vector3.zero;
                }

                ps.SetParticles(particles, count);
            }
        }
    }
}