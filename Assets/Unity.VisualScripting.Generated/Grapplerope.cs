using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Grapplerope : MonoBehaviour
{
    [Header("糸の色・太さ")]
    [SerializeField] private Color ropeColor = Color.white;  // 好きな色に変更してください
    [SerializeField] private float ropeWidth = 0.05f;

    [Header("ロープ動き")]
    [SerializeField] private int segmentCount = 10;
    [SerializeField] private float waveAmplitude = 0.15f;
    [SerializeField] private float extendSpeed = 50f;

    private LineRenderer lr;
    private Coroutine activeCoroutine;

    // ─────────────────────────────────────────────────────────
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        ApplyMaterial();
        lr.useWorldSpace = true;
        lr.positionCount = 0;
    }

    // インスペクターで値を変えたとき即反映
    private void OnValidate()
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (lr != null) ApplyMaterial();
    }

    // ── テクスチャなしマテリアルを生成して適用 ───────────────
    private void ApplyMaterial()
    {
        // "Sprites/Default" はテクスチャなしで単色表示できるUnity標準シェーダー
        // これで紫（マテリアル未設定状態）にならない
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = ropeColor;
        lr.material = mat;

        // 色を Gradient としても設定（LineRenderer はこちらで色を管理）
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(ropeColor, 0f),
                    new GradientColorKey(ropeColor, 1f) },
            new[] { new GradientAlphaKey(ropeColor.a, 0f),
                    new GradientAlphaKey(ropeColor.a, 1f) }
        );
        lr.colorGradient = gradient;

        lr.startWidth = ropeWidth;
        lr.endWidth = ropeWidth;
        lr.textureMode = LineTextureMode.Stretch;
    }

    // ── 公開API ──────────────────────────────────────────────

    public void StartRope(Transform origin, Vector3 target)
    {
        StopActive();
        activeCoroutine = StartCoroutine(ExtendRoutine(origin, target, hit: true));
    }

    public void StartMiss(Transform origin, Vector3 target)
    {
        StopActive();
        activeCoroutine = StartCoroutine(ExtendRoutine(origin, target, hit: false));
    }

    public void UpdateRope(Transform origin, Vector3 target)
    {
        if (lr.positionCount == 0) return;
        DrawRope(origin.position, target);
    }

    public void StopRope()
    {
        StopActive();
        activeCoroutine = StartCoroutine(RetractRoutine());
    }

    // ── コルーチン ───────────────────────────────────────────

    private IEnumerator ExtendRoutine(Transform origin, Vector3 target, bool hit)
    {
        lr.positionCount = segmentCount;
        float totalDist = Vector3.Distance(origin.position, target);
        float elapsed = 0f;

        while (elapsed < totalDist / extendSpeed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed * extendSpeed / totalDist);
            Vector3 tip = Vector3.Lerp(origin.position, target, t);
            DrawRope(origin.position, tip);
            yield return null;
        }

        if (!hit)
        {
            yield return new WaitForSeconds(0.1f);
            yield return RetractRoutine();
        }
    }

    private IEnumerator RetractRoutine()
    {
        float elapsed = 0f;
        float duration = 0.12f;
        int startPts = lr.positionCount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            lr.positionCount = Mathf.Max(0,
                Mathf.RoundToInt(Mathf.Lerp(startPts, 0, elapsed / duration)));
            yield return null;
        }
        lr.positionCount = 0;
    }

    // ── 描画 ─────────────────────────────────────────────────

    private void DrawRope(Vector3 start, Vector3 end)
    {
        if (lr.positionCount < 2) return;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector3 pos = Vector3.Lerp(start, end, t);
            Vector3 perp = GetPerpendicular(end - start);
            float wave = Mathf.Sin(t * Mathf.PI) * waveAmplitude;
            pos += perp * wave;
            lr.SetPosition(i, pos);
        }
    }

    private Vector3 GetPerpendicular(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return Vector3.up;
        Vector3 perp = Vector3.Cross(dir.normalized, Vector3.forward);
        if (perp.sqrMagnitude < 0.01f)
            perp = Vector3.Cross(dir.normalized, Vector3.up);
        return perp.normalized;
    }

    private void StopActive()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
    }
}
