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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ÉSÅ[Éã");
        if (collision.gameObject.tag == ("Player"))
        {
            if (clearImage != null)
            {
                clearImage.SetActive(true);

            }
            StartCoroutine(gool());
        }
    }

    IEnumerator gool()
    {
        for(int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
        }
        SceneManager.LoadScene("TitleScene");
        yield break;
        
    }
}
