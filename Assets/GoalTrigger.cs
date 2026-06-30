using Kounosuke;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{

    [SerializeField] private GameObject clearImage;


    private void Start()
    {
        clearImage.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            if (clearImage != null)
            {
                clearImage.SetActive(true);

            }
            //  GameManager�ɒʒm
            FindAnyObjectByType<GameManager>().GoalEvent();
        }
    }
}
