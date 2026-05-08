using UnityEngine;

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
        }
    }
}
