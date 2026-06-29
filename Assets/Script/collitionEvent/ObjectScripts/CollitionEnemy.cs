using System.Collections;
using UnityEngine;

namespace Kounosuke
{
    public class CollitionEnemy : GimmickBase
    {
        private bool isDead = false;

        [SerializeField] private float blowPower = 10f;
        [SerializeField] private GameObject Body;

        protected Rigidbody2D rb;
        private Collider2D col;
        Vector3 startPos;
        Coroutine Coroutine;

        protected void Start()
        {
            
        }
        public override void OnStart()
        {
            base.OnStart();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            startPos = transform.position;
        }

        public override void OnReset()
        {
            base.OnReset();
            isDead = false;
            transform.position = startPos;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.rotation = Quaternion.identity;

            rb.bodyType = RigidbodyType2D.Kinematic;
            col.enabled = true;
            Body.SetActive(true);

            if(Coroutine != null)
            {
                StopCoroutine(Coroutine);
                Coroutine = null;
            }
        }

        

        /// <summary>
        /// è’ìÀéûèàóùä÷êî
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isDead) return;
            if (collision.gameObject.CompareTag("DeadZone"))
            {
                isDead = true;
                rb.bodyType = RigidbodyType2D.Kinematic;
                col.enabled = true;
                transform.position = startPos;
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.TryGetComponent(out BoostController boostController))
                {
                    if (boostController.IsInvincible)
                    {
                        OnCollitionEvent();
                        isDead = true;
                        rb.linearVelocity = Vector2.zero;

                        Vector2 direction = (transform.position - collision.transform.position).normalized;
                        direction.y += 0.5f;
                        direction.Normalize();

                        rb.bodyType = RigidbodyType2D.Dynamic;
                        col.enabled = false;

                        rb.linearVelocity = Vector2.zero;
                        rb.AddForce(direction * blowPower, ForceMode2D.Impulse);
                        rb.AddTorque(10f, ForceMode2D.Impulse);
                        Coroutine = StartCoroutine(wditdead());
                    }
                }
            }
        }

        protected virtual void OnCollitionEvent()
        {

        }

        IEnumerator wditdead()
        {
            yield return new WaitForSeconds(3f);
            Body.SetActive(false);
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}