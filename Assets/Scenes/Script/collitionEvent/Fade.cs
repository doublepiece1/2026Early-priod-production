using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class Fade : MonoBehaviour
{
    [SerializeField,Header("遷移画像")] Material fadeImage;
    [SerializeField, Header("遷移時間")] float fadeDuration = 1f;
    [SerializeField, Header("開始状態")] bool is_stert = false;
    Tween fadeTween;
    int Fadevalue = 0;
    private void Awake()
    {
        Fadevalue = is_stert ? 1 : 0;
        fadeImage.SetFloat("_FadeValue", Fadevalue);
    }
    public void SetFadeMaterial(Material fade)
    {
        fadeImage = fade;
        GetComponent<SpriteRenderer>().material = fadeImage;
    }
    public void FadeImage(bool fade)
    {
        fadeTween?.Kill();
        fadeTween = fadeImage.DOFloat(fade ? 1 : 0, "_FadeValue", fadeDuration);
    }
}