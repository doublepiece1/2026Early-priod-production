using UnityEngine;

namespace Kounosuke
{
    public class ParticleAttractor : MonoBehaviour
    {
        [Header("吸引対象")]
        [SerializeField] private Transform center;

        [Header("吸引設定")]
        [SerializeField] private float baseSpeed = 2f;
        [SerializeField] private float maxSpeed = 10f;

        [Range(0f, 1f)]
        [SerializeField] private float intensity = 1f;

        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();

            int max = Mathf.Max(1, ps.main.maxParticles);
            particles = new ParticleSystem.Particle[max];
        }

        private void LateUpdate()
        {
            if (center == null)
                return;

            int count = ps.GetParticles(particles);

            float speed =
                Mathf.Lerp(baseSpeed, maxSpeed, intensity);

            for (int i = 0; i < count; i++)
            {
                Vector3 dir =
                    (center.position - particles[i].position)
                    .normalized;

                particles[i].velocity = dir * speed;
            }

            ps.SetParticles(particles, count);
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