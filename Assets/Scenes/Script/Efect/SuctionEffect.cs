using UnityEngine;

namespace Kounosuke
{
    /// <summary>
    /// パーティクルを中心へ吸引する演出
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class SuctionEffect : MonoBehaviour
    {
        [Header("吸引対象")]
        [SerializeField] private Transform center;

        [Header("吸引設定")]
        [SerializeField] private float baseSpeed = 2f;
        [SerializeField] private float maxSpeed = 10f;

        [Range(0f, 1f)]
        [SerializeField] private float intensity = 1f;

        [Header("チャージ演出")]
        [SerializeField] private bool pulseMode = true;
        [SerializeField] private float pulseSpeed = 5f;

        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;

        private float pulse;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();

            particles =
                new ParticleSystem.Particle[
                    Mathf.Max(
                        1,
                        ps.main.maxParticles)];
        }

        private void LateUpdate()
        {
            if (!ps.isPlaying)
                return;

            if (center == null)
                return;

            int count =
                ps.GetParticles(particles);

            float speed =
                CalculateSpeed();

            UpdateParticles(
                count,
                speed);

            ps.SetParticles(
                particles,
                count);
        }

        private float CalculateSpeed()
        {
            if (!pulseMode)
            {
                return Mathf.Lerp(
                    baseSpeed,
                    maxSpeed,
                    intensity);
            }

            pulse +=
                Time.deltaTime *
                pulseSpeed;

            float t =
                Mathf.Sin(pulse) *
                0.5f +
                0.5f;

            return Mathf.Lerp(
                baseSpeed,
                maxSpeed,
                t);
        }

        private void UpdateParticles(
            int count,
            float speed)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 dir =
                    CalculateDirection(i);

                float seed =
                    (particles[i].randomSeed % 1000)
                    * 0.001f;

                float finalSpeed =
                    speed *
                    Mathf.Lerp(
                        0.7f,
                        1.3f,
                        seed);

                particles[i].velocity =
                    dir *
                    finalSpeed;
            }
        }

        private Vector3 CalculateDirection(
            int index)
        {
            Vector3 dir =
                (
                    center.position -
                    particles[index].position
                ).normalized;

            Vector3 tangent =
                Vector3.Cross(
                    dir,
                    Vector3.forward)
                .normalized;

            float seed =
                (particles[index].randomSeed % 1000)
                * 0.001f;

            float swirl =
                Mathf.Sin(
                    Time.time * 3f +
                    seed * 10f);

            return
                (
                    dir +
                    tangent * swirl * 0.6f
                ).normalized;
        }

        public void SetCenter(
            Transform target)
        {
            center = target;
        }

        public void SetIntensity(
            float value)
        {
            intensity =
                Mathf.Clamp01(value);
        }

        public void SetPulseMode(
            bool enable)
        {
            pulseMode = enable;
        }
    }
}