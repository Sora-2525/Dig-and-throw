using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;   // 一定時間で自動消滅
    public int damage = 1;
    public float rotationSpeed = 720f; // 1秒あたりの回転角度（度）
    public bool piercing = false;      // true なら敵を貫通する（ビーム用）
    public GameObject hitEffect;       // 攻撃がヒットしたときに出すエフェクト

    private Vector2 direction;
    private BossEnemy hitBoss;          // 同じボスに多重ヒットしないように記録

    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 直進
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        // 回転させながら飛ばす
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ボスにはダメージ（当たり判定が子オブジェクトにある場合も親を探す）
        BossEnemy boss = other.GetComponentInParent<BossEnemy>();
        if (boss != null)
        {
            // 同じボスには1回だけ（パーツが複数あっても多重ヒットしない）
            if (boss == hitBoss) return;
            hitBoss = boss;
            boss.TakeDamage(damage);
            SpawnHitEffect();
            if (!piercing) Destroy(gameObject); // 貫通中は消えない
            return;
        }

        // 通常の敵にダメージ
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            SpawnHitEffect();
            if (!piercing) Destroy(gameObject); // 貫通中は消えない
            return;
        }

        // 壁に当たったら消える（貫通でも壁では止まる）
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    // ヒット位置にエフェクトを生成
    void SpawnHitEffect()
    {
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);
    }
}
