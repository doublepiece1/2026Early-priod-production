using Kounosuke;
using UnityEngine;

public class GoalTrigger : GimmickBase
{

    [SerializeField] private GameObject clearImage;

    public override void OnStart()
    {
        base.OnStart();
        clearImage.SetActive(false);
    }

    public override void OnReset()
    {
        base.OnReset();
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
