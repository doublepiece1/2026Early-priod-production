using UnityEngine;

namespace Kounosuke
{
    public class ParticleAttractor : MonoBehaviour
    {
        [Header("吸引対象")]
        [SerializeField] private Transform center;

        [Header("吸引設定")]
        [SerializeField] private float minSpeed = 1f;
        [SerializeField] private float maxSpeed = 10f;

        [Range(0f, 1f)]
        [SerializeField] private float intensity = 1f;

        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;

        private Vector3 lastCenterPosition;

        [SerializeField] BoostController boost;

        public void SetCharge(float normalized, float speed)
        {
            var main = ps.main;

            float t = Mathf.Clamp01(normalized);

            main.simulationSpeed =
                Mathf.Lerp(minSpeed, maxSpeed, t) *
                Mathf.InverseLerp(0, 10f, speed);
        }

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();

            int max = Mathf.Max(1, ps.main.maxParticles);
            particles = new ParticleSystem.Particle[max];

            if (center != null)
                lastCenterPosition = center.position;
        }

        private void LateUpdate()
        {
            if (center == null)
                return;

            Vector3 centerDelta =
                center.position - lastCenterPosition;

            int count = ps.GetParticles(particles);

            float speed = Mathf.Lerp(minSpeed, maxSpeed, intensity);

            SetCharge(boost.NormalizedGauge, boost.CurrentSpeed);

            for (int i = 0; i < count; i++)
            {
                // プレイヤー移動分を補償
                particles[i].position += centerDelta;

                // 吸引
                particles[i].position = Vector3.MoveTowards(
                    particles[i].position,
                    center.position,
                    speed * Time.deltaTime
                );
            }



            ps.SetParticles(particles, count);

            lastCenterPosition = center.position;
        }

        public void SetCenter(Transform target)
        {
            center = target;
        }

        public void SetIntensity(float value)
        {
            intensity = Mathf.Clamp01(value);
        }
    }
}