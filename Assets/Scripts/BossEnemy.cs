using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    public int maxHP = 50;
    private int currentHP;
    public float moveSpeed = 2f;
    public Image hpBar;                 // ボスのHPバー（Fill）

    [Header("炎弾攻撃")]
    public GameObject fireballPrefab;   // 炎弾プレハブ
    public float fireIntervalMin = 1.5f;// 発射間隔の最小（ランダム）
    public float fireIntervalMax = 4.0f;// 発射間隔の最大（ランダム）
    public float fireballSpeed = 6f;
    public int radialCount = 12;        // 全方位弾幕の弾数
    public int fanCount = 5;            // 自機狙い扇の弾数
    public float fanSpread = 40f;       // 扇の広がり（度）
    public float spawnOffset = 0.7f;    // ボス中心から離して発射する距離

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer[] sprites;  // 子パーツも含めた見た目（点滅用）

    void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // ��]�h�~
        // �v���C���[��T��
        player = GameObject.FindWithTag("Player").transform;

        // 自分と子オブジェクトの SpriteRenderer をすべて取得
        sprites = GetComponentsInChildren<SpriteRenderer>();

        // HPバーを満タンにする
        if (hpBar != null) hpBar.fillAmount = 1f;

        // 炎弾攻撃を開始
        StartCoroutine(FireLoop());
    }

    void FixedUpdate()
    {
        if (player == null) return;
        // �v���C���[�Ɍ������Ĉړ�
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("�{�XHP�F" + currentHP);

        // HPバーを更新
        if (hpBar != null)
            hpBar.fillAmount = (float)currentHP / maxHP;

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            // 前の点滅が残っていたら止めてからやり直す（連続ヒットでも確実に光る）
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(DamageFlash());
        }
    }

    private Coroutine flashRoutine;

    // ダメージを受けたとき点滅する（表示/非表示を切り替え：子パーツも一緒に）
    IEnumerator DamageFlash()
    {
        if (sprites == null) yield break;

        for (int n = 0; n < 3; n++)
        {
            SetVisible(false);
            yield return new WaitForSeconds(0.06f);
            SetVisible(true);
            yield return new WaitForSeconds(0.06f);
        }
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        foreach (var s in sprites)
            if (s != null) s.enabled = visible;
    }

    void Die()
    {
        Debug.Log("�{�X��|�����I");
        SceneManager.LoadScene("GameClear");
    }

    // ===== 炎弾攻撃 =====
    IEnumerator FireLoop()
    {
        // 開始直後の数秒は撃たない（接近の猶予）
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            float wait = Random.Range(fireIntervalMin, fireIntervalMax);
            yield return new WaitForSeconds(wait);

            if (fireballPrefab == null || player == null) continue;

            // パターンをランダムに選択（追尾はしない：発射時の方向で固定）
            int pattern = Random.Range(0, 4);
            switch (pattern)
            {
                case 0: FireAimed(); break;                               // 自機狙い1発
                case 1: FireFan(); break;                                 // 自機狙い扇
                case 2: FireRadial(); break;                              // 全方位弾幕
                default: yield return StartCoroutine(FireSpiral()); break;// 回転弾幕
            }
        }
    }

    Vector2 DirToPlayer()
    {
        return ((Vector2)player.position - (Vector2)transform.position).normalized;
    }

    Vector2 AngleToDir(float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    void SpawnFireball(Vector2 dir)
    {
        Vector3 pos = transform.position + (Vector3)(dir * spawnOffset);
        GameObject fb = Instantiate(fireballPrefab, pos, Quaternion.identity);
        Fireball f = fb.GetComponent<Fireball>();
        if (f != null)
        {
            f.speed = fireballSpeed;
            f.Launch(dir);
        }
    }

    // 自機狙い1発（現在位置へまっすぐ）
    void FireAimed()
    {
        SpawnFireball(DirToPlayer());
    }

    // 自機狙いの扇状
    void FireFan()
    {
        Vector2 baseDir = DirToPlayer();
        float baseAng = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        for (int i = 0; i < fanCount; i++)
        {
            float t = (fanCount <= 1) ? 0.5f : (float)i / (fanCount - 1);
            float ang = baseAng - fanSpread * 0.5f + fanSpread * t;
            SpawnFireball(AngleToDir(ang));
        }
    }

    // 全方位弾幕
    void FireRadial()
    {
        float offset = Random.Range(0f, 360f);
        for (int i = 0; i < radialCount; i++)
        {
            float ang = offset + 360f * i / radialCount;
            SpawnFireball(AngleToDir(ang));
        }
    }

    // 回転しながら撃つ弾幕（東方風）
    IEnumerator FireSpiral()
    {
        int shots = 14;
        float ang = Random.Range(0f, 360f);
        for (int i = 0; i < shots; i++)
        {
            SpawnFireball(AngleToDir(ang));
            SpawnFireball(AngleToDir(ang + 180f));
            ang += 28f;
            yield return new WaitForSeconds(0.08f);
        }
    }
}