using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class Fade : MonoBehaviour
{
    [SerializeField,Header("遷移画像")] Material fadeImage;
    [SerializeField, Header("遷移時間")] float fadeDuration = 1f;

    private void Start()
    {
        fadeImage.SetFloat("_FadeValue", 0);
    }

    public void FadeImage(bool fade)
    {
        fadeImage.DOFloat((fade ? 1 : 0), "_FadeValue", fadeDuration);
        Debug.Log("Fade");
    }
}