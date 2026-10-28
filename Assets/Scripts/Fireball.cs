using UnityEngine;
using UnityEngine.Tilemaps;

// ボスが撃つ炎弾。追尾せず発射方向へ直進し、着弾で
// ・プレイヤーにダメージ
// ・周辺ブロックを破壊（ボス際スタック攻略の防止）
// ・爆発エフェクトを生成
public class Fireball : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 1;
    public float lifeTime = 6f;
    public float destroyRadius = 1.2f;   // 着弾時に壊すブロック半径（ワールド）
    public GameObject explosionEffect;   // 爆発エフェクト

    private Vector2 dir;
    private bool exploded = false;

    // 発射（方向は発射時に固定＝追尾しない）
    public void Launch(Vector2 direction)
    {
        dir = direction.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;

        // プレイヤーに命中 → ダメージ＋爆発
        if (other.CompareTag("Player"))
        {
            PlayerMove pm = other.GetComponentInParent<PlayerMove>();
            if (pm != null) pm.TakeDamage(damage);
            Explode();
            return;
        }

        // 壁（掘れる壁・外周壁）に命中 → 爆発。床(Untagged)は貫通する
        if (other.CompareTag("Wall"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        DestroyDiggableAround(transform.position);
        Destroy(gameObject);
    }

    // 着弾周辺の「掘れる壁」だけを破壊（床・外周壁・背景は壊さない）
    void DestroyDiggableAround(Vector3 center)
    {
        // プレイヤーが掘れる対象 = DigController.wallTilemap を破壊対象にする
        Tilemap target = null;
        DigController dig = Object.FindFirstObjectByType<DigController>();
        if (dig != null) target = dig.wallTilemap;
        if (target == null) return;

        int r = Mathf.CeilToInt(destroyRadius);
        Vector3Int c = target.WorldToCell(center);
        for (int dx = -r; dx <= r; dx++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                if (new Vector2(dx, dy).magnitude > destroyRadius) continue;
                Vector3Int cell = new Vector3Int(c.x + dx, c.y + dy, 0);
                if (target.HasTile(cell)) target.SetTile(cell, null);
            }
        }
    }
}
