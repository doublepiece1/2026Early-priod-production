using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Kounosuke
{
    public class CollitionFall : MonoBehaviour
    {
        [SerializeField] private float waitTime;
        private Rigidbody2D rb;
        private Collider2D collider2D;
        private Vector3 start_pos;
        private bool isFalling = false;

        public void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            collider2D = GetComponent<Collider2D>();
            start_pos = transform.position;
            ResetMoveFloor();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isFalling)
            {
                return;
            }
            if (!collision.gameObject.CompareTag("Player"))
            {
                return;
            }
            foreach (var contact in collision.contacts)
            {
                Debug.Log(contact.normal);

                if (Vector2.Dot(contact.normal, Vector2.down) > 0.8f)
                {
                    Fall().Forget();
                    break;
                }
            }
        }

        private async UniTaskVoid Fall()
        {
            await Wait((int)waitTime * 1000);
            collider2D.isTrigger = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1;
            await Wait((int)waitTime * 1000 * 3);
            rb.gravityScale = 0;
            gameObject.SetActive(false);
        }
        private async UniTask Wait(int time)
        {
            await UniTask.Delay(time);
        }

        public void OnReset()
        {
            ResetMoveFloor();
        }

        public void ResetMoveFloor()
        {
            collider2D.isTrigger = false;
            transform.position = start_pos;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}