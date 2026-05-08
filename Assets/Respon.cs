using UnityEngine;
using UnityEngine.SceneManagement;


public class Respon : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}
