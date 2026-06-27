using System.Runtime.CompilerServices;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField, Header("カメラ")] private Camera camera_;
    void Start()
    {
        var sr_ = GetComponent<SpriteRenderer>();
        camera_ = Camera.main;

        float height = camera_.orthographicSize * 2;
        float width = height * camera_.aspect;

        float spriteWidth = sr_.bounds.size.x;
        float spriteHeight = sr_.bounds.size.y;

        transform.localScale = new Vector3(
            width / spriteWidth,
            height / spriteHeight,
            1
        );
    }
}
