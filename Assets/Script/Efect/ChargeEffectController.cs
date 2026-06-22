using UnityEngine;

namespace Kounosuke
{
    public class ChargeEffectController :MonoBehaviour,IEffect
    {
        [SerializeField]
        private ParticleSystem targetEffect;

        private bool isVisible = true;

        private void Awake()
        {
            if (targetEffect == null)
            {
                targetEffect =
                    GetComponent<ParticleSystem>();
            }

            Hide();
        }

        public void Show()
        {
            if (isVisible)
                return;

            isVisible = true;

            targetEffect.Clear();
            targetEffect.Play();
        }

        public void Hide()
        {
            if (!isVisible)
                return;

            isVisible = false;

            targetEffect.Stop(
                true,
                ParticleSystemStopBehavior
                    .StopEmittingAndClear);
        }
    }
}