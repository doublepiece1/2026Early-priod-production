using UnityEngine;
using UnityEngine.InputSystem;

namespace Kounosuke
{
    public class TitleSceneManager : MonoBehaviour
    {
        void Start()
        {
            var playerInput = GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("UI");
        }
    }
}