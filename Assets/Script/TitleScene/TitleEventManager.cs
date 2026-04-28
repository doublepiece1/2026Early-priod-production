using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class TitleEventManager : MonoBehaviour
{
    [SerializeField,Header("サブキャンバス")] private GameObject subCanvas_;
    [SerializeField,Header("メインキャンバスのボタン")] private GameObject MainCanvasButton_;
    [SerializeField,Header("キャンセルアクション")] private InputSystemUIInputModule inputModule_;
    [SerializeField,Header("サブキャンバス切り替えボタン")] private GameObject targetButton_;

    private PlayerInputActions InputActions_;
    private bool isActiveSubCanvas_ = false;

    private void OnEnable()
    {
        InputActions_ = new PlayerInputActions();
        inputModule_.actionsAsset.FindAction("Cancel").performed += OnCancel;
    }
    void Start()
    {
        if (subCanvas_ != null) {
            subCanvas_.SetActive(isActiveSubCanvas_);
        }
    }
    
    public void ActiveSubCanvas()
    {
        if (subCanvas_ != null) {
            isActiveSubCanvas_ = !isActiveSubCanvas_;
            subCanvas_.SetActive(isActiveSubCanvas_);
            if (MainCanvasButton_ != null) {
                MainCanvasButton_.GetComponent<Button>().interactable = !isActiveSubCanvas_;
            }   
        }
    }

    void OnDisable()
    {
        inputModule_.actionsAsset.FindAction("Cancel").performed -= OnCancel;
        InputActions_.Dispose();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("キャンセルボタンが押されました");
        ActiveSubCanvas();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(targetButton_);
    }
}
