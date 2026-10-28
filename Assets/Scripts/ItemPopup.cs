using UnityEngine;

// 宝箱などから出たアイテムが上にふわっと上がりながらフェードして消える演出
public class ItemPopup : MonoBehaviour
{
    public float rise = 1.2f;       // 上に上がる距離
    public float duration = 1.2f;   // 表示時間（秒）

    private SpriteRenderer sr;
    private Vector3 startPos;
    private float t;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / duration);
        // イーズアウト気味に上昇
        float e = 1f - (1f - k) * (1f - k);
        transform.position = startPos + Vector3.up * (rise * e);
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f - k; // 徐々に透明へ
            sr.color = c;
        }
        if (k >= 1f) Destroy(gameObject);
    }
}
