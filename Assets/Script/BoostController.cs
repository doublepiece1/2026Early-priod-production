using System.Collections;
using UnityEngine;

namespace Kounosuke
{
    public class BoostController : MonoBehaviour
    {
        [SerializeField] private float boostMax = 100f;
        [SerializeField] private float boostGainMultiplier = 10f;
        [SerializeField] private float releaseBoost = 5f;
        [SerializeField] private float invincibleTime = 1.5f;

        [Header("Effect")]
        [SerializeField] private ParticleSystem boostReadyEffect;
        [SerializeField] private ParticleSystem boostBurstEffect;
        [SerializeField] private ChargeEffectController chargeEffectBehaviour;
        [SerializeField] private TrailRenderer trail;

        [SerializeField] private float speedScale = 10f;

        public float NormalizedGauge => boostGauge / boostMax;
        public float CurrentSpeed { get; private set; }

        private IEffect chargeEffect;
        private Rigidbody2D rb;

        private float boostGauge;

        public bool IsReady { get; private set; }

        [Tooltip("–³“G")]public bool IsInvincible { get; private set; }

        PlayerRespon PlayerRespon;
        private Coroutine invincibleCoroutine;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            PlayerRespon = GetComponent<PlayerRespon>();

            chargeEffect =
                chargeEffectBehaviour as IEffect;
            if (trail != null)
                trail.emitting = false;
        }
        public void Charge(float speed)
        {
            if (IsReady)
            { 
                return;
            }
                

            CurrentSpeed = speed;

            chargeEffect?.Show();

            float gain =
                speed *
                boostGainMultiplier *
                Time.fixedDeltaTime;

            boostGauge += gain;


            if (boostGauge >= boostMax)
            {
                boostGauge = boostMax;
                IsReady = true;
            }
        }

        public void Release()
        {
            if (IsReady)
            {
                ApplyBoost();
            }
            else
            {
                ApplyReleaseBoost();
            }

            ResetBoost();
        }
        private void ApplyBoost()
        {
            Vector2 dir =
                rb.linearVelocity.normalized;

            if (dir == Vector2.zero)
                dir = Vector2.right;

            rb.linearVelocity = Vector2.zero;

            if (boostBurstEffect)
            {
                boostBurstEffect.Play();
            }

            rb.AddForce(
                dir * releaseBoost * 3f,
                ForceMode2D.Impulse);

            if (invincibleCoroutine != null)
            {
                StopCoroutine(invincibleCoroutine);
            }

            invincibleCoroutine = StartCoroutine(InvincibleRoutine());
            if (trail != null)
            {
                trail.Clear();
                //trail.emitting = true;
            }
        }

        public void AddBoost()
        {
            Vector2 dir =rb.linearVelocity.normalized;

            if (dir == Vector2.zero)
                dir = Vector2.right;

            rb.linearVelocity = Vector2.zero;

            if (boostBurstEffect)
            {
                boostBurstEffect.Play();
            }

            rb.AddForce(
                dir * releaseBoost * 3f,
                ForceMode2D.Impulse);

            if (invincibleCoroutine != null)
            {
                StopCoroutine(invincibleCoroutine);
            }

            invincibleCoroutine = StartCoroutine(InvincibleRoutine());
        }

        private void ApplyReleaseBoost()
        {
            if (rb.linearVelocity.magnitude < 0.5f)
                return;

            rb.AddForce(
                rb.linearVelocity.normalized *
                releaseBoost,
                ForceMode2D.Impulse);
        }

        public void ResetBoost()
        {
            boostGauge = 0f;
            IsReady = false;

            chargeEffect?.Hide();
        }

        private IEnumerator InvincibleRoutine()
        {
            IsInvincible = true;
            
            PlayerRespon.isInvincible = true;

            yield return
                new WaitForSeconds(
                    invincibleTime);

            IsInvincible = false;
            PlayerRespon.isInvincible = false;
            invincibleCoroutine = null;

            if (trail != null)
            {
                trail.emitting = false;
            }
        }
    }
}