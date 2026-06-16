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

        private IEffect chargeEffect;
        private Rigidbody2D rb;

        private float boostGauge;

        public bool IsReady { get; private set; }
        public bool IsInvincible { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            chargeEffect =
                chargeEffectBehaviour as IEffect;
        }
        public void Charge(float speed)
        {
            if (IsReady)
                return;

            chargeEffect?.Show();

            boostGauge +=
                speed *
                boostGainMultiplier *
                Time.fixedDeltaTime;

            if (boostGauge < boostMax)
                return;

            boostGauge = boostMax;
            IsReady = true;

            //boostReadyEffect?.Play();
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

            StartCoroutine(
                InvincibleRoutine());
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

        private void ResetBoost()
        {
            boostGauge = 0f;
            IsReady = false;

            chargeEffect?.Hide();
        }

        private IEnumerator InvincibleRoutine()
        {
            IsInvincible = true;

            yield return
                new WaitForSeconds(
                    invincibleTime);

            IsInvincible = false;
        }
    }
}