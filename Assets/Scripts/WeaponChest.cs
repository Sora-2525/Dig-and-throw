using UnityEngine;
using System.Collections;

public class WeaponChest : MonoBehaviour
{
    public Sprite openedSprite;       // 開いた宝箱の絵（任意）
    [Header("入手時に出す操作説明UI")]
    public GameObject controlHintUI;  // 「Fキーでビーム攻撃！」などのUI
    public float hintDuration = 4f;   // 表示しておく秒数（0以下ならずっと表示）

    [Header("開封演出")]
    public GameObject openEffect;     // 開封時のパーティクル
    public GameObject itemPopup;      // 飛び出す魔法アイテム（本/杖）
    public Vector3 popupOffset = new Vector3(0f, 0.3f, 0f); // 演出の出現位置オフセット

    private bool opened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;
        if (!other.CompareTag("Player")) return;

        opened = true;

        // ビーム武器を入手（以降ずっと使える）
        PlayerMove.hasBeamWeapon = true;
        Debug.Log("強い武器（貫通ビーム）を手に入れた！");

        // 開封エフェクトと魔法アイテムの出現演出
        Vector3 spawnPos = transform.position + popupOffset;
        if (openEffect != null) Instantiate(openEffect, spawnPos, Quaternion.identity);
        if (itemPopup != null) Instantiate(itemPopup, spawnPos, Quaternion.identity);

        // 操作説明UIを表示
        if (controlHintUI != null)
        {
            controlHintUI.SetActive(true);
            if (hintDuration > 0f)
                StartCoroutine(HideHintAfterDelay());
        }

        // 開いた絵に差し替え（設定されていれば）
        if (openedSprite != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = openedSprite;
        }

        // 当たり判定だけ消す（絵は残す）
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    IEnumerator HideHintAfterDelay()
    {
        yield return new WaitForSeconds(hintDuration);
        if (controlHintUI != null)
            controlHintUI.SetActive(false);
    }
}
