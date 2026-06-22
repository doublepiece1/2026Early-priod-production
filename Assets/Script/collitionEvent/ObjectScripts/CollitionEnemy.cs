using Unity.VisualScripting;
using UnityEngine;

namespace Kounosuke
{
    public class CollitionEnemy : GimmickBase
    {
        
        /// <summary>
        /// Õ“Ëˆ—ŠÖ”
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {

            }
        }

    }
}