using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public GameObject attackEffect;
    public GameObject projectilePrefab; // 投げる弾のプレハブ
    public float projectileSpeed = 10f;
    public int projectileDamage = 5;    // お宝投げの威力
    public GameObject beamPrefab;       // 宝箱で手に入る貫通ビームのプレハブ
    public float beamSpeed = 15f;
    public int beamDamage = 1;
    public float beamFireRate = 0.15f;  // ビーム連射の間隔（秒）
    private float beamCooldown = 0f;     // 次に撃てるまでの残り時間
    // ビーム武器を持っているか（一度拾うとずっと持続：static で保持）
    public static bool hasBeamWeapon = false;
    public int maxHP = 5;
    public int currentHP;
    public Image hpBar;
    public Vector2 lastMoveDir = Vector2.right;
    // お宝の所持数（シーンをまたいで保持するため static で持つ）
    public static int carriedTreasure = 0;
    // HPもシーンをまたいで保持する（-1 = 未初期化＝満タンから開始）
    public static int savedHP = -1;
    public int treasureCount
    {
        get { return carriedTreasure; }
        set { carriedTreasure = value; }
    }
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isInvincible = false; // 追加
    private Animator anim;             // 歩行アニメ用
    private SpriteRenderer spriteRenderer; // 向き反転用

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryTakeDamageFrom(collision.collider);
    }

    // 接触し続けている間もダメージ判定（無敵時間中はスキップされる）
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryTakeDamageFrom(collision.collider);
    }

    // 敵またはボスに当たったらダメージ
    private void TryTakeDamageFrom(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.GetComponentInParent<BossEnemy>() != null)
        {
            TakeDamage(1);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // HPをステージ間で引き継ぐ。未初期化なら満タンから開始
        if (savedHP < 0) savedHP = maxHP;
        currentHP = savedHP;
        if (hpBar != null) hpBar.fillAmount = (float)currentHP / maxHP;
    }

    IEnumerator SmoothHPDecrease()
    {
        float targetFill = (float)currentHP / maxHP;
        while (hpBar.fillAmount > targetFill)
        {
            hpBar.fillAmount -= Time.deltaTime * 2f;
            yield return null;
        }
        hpBar.fillAmount = targetFill;
    }

    // ▼ 追加：点滅しながら無敵時間
    IEnumerator InvincibleTime()
    {
        isInvincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 6; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        sr.enabled = true;
        isInvincible = false;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // 追加：無敵中はスキップ

        currentHP -= damage;
        savedHP = currentHP; // 引き継ぎ用に保存
        Debug.Log("プレイヤーHP: " + currentHP);
        StartCoroutine(SmoothHPDecrease());
        StartCoroutine(InvincibleTime()); // 追加

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("プレイヤーが倒れた！");
        savedHP = -1; // 死亡したら次回は満タンから
        StartCoroutine(DieSequence()); // 追加
    }

    // ▼ 追加：死亡演出→ゲームオーバーシーンへ
    IEnumerator DieSequence()
    {
        isInvincible = true; // 死亡中に追加ダメージ防止
        hpBar.fillAmount = 0f;
        // 操作を止める
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        this.enabled = false; // Update停止
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("GameOver"); // ← シーン名を合わせる
    }

    void Update()
    {
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y = 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y = -1;
            if (Keyboard.current.aKey.isPressed) moveInput.x = -1;
            if (Keyboard.current.dKey.isPressed) moveInput.x = 1;

            // スペース：お宝を投げる
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ThrowTreasure();
            }

            // F：貫通ビーム（押しっぱなしで連射）
            beamCooldown -= Time.deltaTime;
            if (hasBeamWeapon && Keyboard.current.fKey.isPressed && beamCooldown <= 0f)
            {
                FireBeam();
                beamCooldown = beamFireRate;
            }
        }

        if (moveInput.magnitude > 1f)
            moveInput = moveInput.normalized;

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput;

        // 歩行アニメ：移動入力があれば歩く、なければ待機
        bool moving = moveInput != Vector2.zero;
        if (anim != null) anim.SetBool("IsMoving", moving);
        // 向き：左右入力に応じてスプライトを反転
        if (spriteRenderer != null && moveInput.x != 0f)
            spriteRenderer.flipX = moveInput.x < 0f;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }

    void ThrowTreasure()
    {
        if (treasureCount <= 0)
        {
            Debug.Log("お宝がない！攻撃できない");
            return;
        }
        treasureCount--;
        Debug.Log("お宝を投げた！残り：" + treasureCount);

        // 弾を生成して直線に飛ばす
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + (Vector3)(lastMoveDir * 0.5f);
            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
            {
                p.speed = projectileSpeed;
                p.damage = projectileDamage;
                p.Launch(lastMoveDir);
            }
        }
    }

    // 貫通ビームを発射（お宝消費なし）
    void FireBeam()
    {
        if (beamPrefab == null) return;

        Vector3 spawnPos = transform.position + (Vector3)(lastMoveDir * 0.5f);
        GameObject beam = Instantiate(beamPrefab, spawnPos, Quaternion.identity);
        Projectile p = beam.GetComponent<Projectile>();
        if (p != null)
        {
            p.speed = beamSpeed;
            p.damage = beamDamage;
            p.piercing = true; // 貫通させる
            p.Launch(lastMoveDir);
        }
    }

    IEnumerator ShowAttackEffect()
    {
        attackEffect.SetActive(true);
        attackEffect.transform.localPosition = lastMoveDir * 0.5f;
        yield return new WaitForSeconds(0.1f);
        attackEffect.SetActive(false);
    }
}