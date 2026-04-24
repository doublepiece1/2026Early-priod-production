using UnityEngine;
using UnityEngine.InputSystem;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField, Header("アニメーター")] private Animator animator;
    private bool IsJump_ = false;

    void Start()
    {
        
    }

    private void Update()
    {
        
    }

    void OnJump(InputValue inputValue)
    {
        if (!IsJump_)
        {
            IsJump_ = true;
        }
    }
}
