using DG.Tweening;
using Kounosuke;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TitleSlimeJump : TitleMoveSlime
{
    [SerializeField, Header("ジャンプするベクター")] private Vector3 vec_;
    private void Start()
    {
        if (!HasCheck(Object_)) Debug.LogError("Not Found GameObject");
        Push_Sequence();
    }
    public override void Push_Sequence()
    {
        Object_.transform.DOJump(vec_, jumpPower: 3f, numJumps: 2, duration: 3f).SetLoops(-1, LoopType.Yoyo);
    }
}
