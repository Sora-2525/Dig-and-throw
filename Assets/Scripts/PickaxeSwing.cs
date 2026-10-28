using UnityEngine;

// つるはしが短時間だけ振られて消える演出。
// DigController が掘る方向に合わせて Init を呼んで生成する。
public class PickaxeSwing : MonoBehaviour
{
    public float duration = 0.22f;
    public float startAngle = 55f;   // 振り始め（方向基準の相対角）
    public float endAngle = -35f;    // 振り終わり
    public float spriteForwardAngle = 135f; // スプライトの「頭」が向く基準角(度)。上向き=90, 左上向き=135

    private float baseAngle;         // 掘る方向の角度（度）
    private float t;

    // 掘る方向の角度を受け取る
    public void Init(float dirAngleDeg)
    {
        baseAngle = dirAngleDeg;
        ApplyAngle(startAngle);
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / duration);
        ApplyAngle(Mathf.Lerp(startAngle, endAngle, k));
        if (k >= 1f) Destroy(gameObject);
    }

    // スプライトは上向き基準。方向へ向け、さらに振り角を加える。
    void ApplyAngle(float swing)
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, baseAngle - spriteForwardAngle + swing);
    }
}
