using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;

public class TitleEventManager : MonoBehaviour
{
    [SerializeField, Header("メインキャンバス")] private GameObject mainCanvas_;
    [SerializeField,Header("サブキャンバス")] private GameObject subCanvas_;
    [SerializeField, Header("スタートボタン")] private GameObject StartButton_;
    [SerializeField,Header("設定ボタン")] private GameObject SettingButton_;
    [SerializeField,Header("キャンセルアクション")] private InputSystemUIInputModule inputModule_;
    [SerializeField,Header("サブキャンバス切り替えボタン")] private GameObject targetButton_;

    private PlayerInputActions InputActions_;
    private bool isActiveSubCanvas_ = false;
    private InputDevice inputDevice_;

    private void OnEnable()
    {
        InputActions_ = new PlayerInputActions();
        inputModule_.actionsAsset.FindAction("Cancel").performed += OnCancel;
        InputSystem.onEvent += OnDeviceChange;
    }
    void Start()
    {
        if (mainCanvas_ != null) {
            mainCanvas_.SetActive(false);
        }
        if (subCanvas_ != null) {
            subCanvas_.SetActive(isActiveSubCanvas_);
        }
    }

    public void ActiveMainCanvas()
    {
        if (mainCanvas_ != null)
        {
            mainCanvas_.SetActive(true);
        }
    }

    public void ActiveSubCanvas()
    {
        if (subCanvas_ != null) {
            isActiveSubCanvas_ = !isActiveSubCanvas_;
            subCanvas_.SetActive(isActiveSubCanvas_);
            if (StartButton_ != null) {
                StartButton_.GetComponent<Button>().interactable = !isActiveSubCanvas_;
            }   
        }
    }

    void OnDisable()
    {
        inputModule_.actionsAsset.FindAction("Cancel").performed -= OnCancel;
        InputActions_.Dispose();
        InputSystem.onEvent -= OnDeviceChange;
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("キャンセルボタンが押されました");
        ActiveSubCanvas();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SettingButton_);
    }

    private void OnDeviceChange(InputEventPtr change,InputDevice device)
    {
        if (!change.IsA<StateEvent>() && !change.IsA<DeltaStateEvent>())
            return;

        if (device != inputDevice_)
        {
            Debug.Log($"切替: {inputDevice_} → {device}");
            inputDevice_ = device;
            if(inputDevice_ == Keyboard.current)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(StartButton_);
            }
            else if(inputDevice_ == Gamepad.current) {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(StartButton_);
            }
        }
    }
}
