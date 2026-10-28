using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float detectRange = 5f;   // ���̋����ȓ��Ńv���C���[�𔭌�
    public float stopRange = 0.5f;   // ���̋����܂ŋ߂Â�����~�܂�
    public int maxHP = 5;            // 敵のHP
    private int currentHP;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;       // 点滅用
    private Color originalColor;

    void Start()
    {
        currentHP = maxHP;
        player = GameObject.Find("player").transform;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    // ダメージを受ける
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("敵HP: " + currentHP);

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            // 倒れていなければダメージ点滅
            StartCoroutine(DamageFlash());
        }
    }

    // ダメージを受けたとき赤く点滅する
    IEnumerator DamageFlash()
    {
        if (sr == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.07f);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.07f);
        }
        sr.color = originalColor;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(rb.position, player.position);

        if (distance <= detectRange && distance > stopRange)
        {
            // プレイヤーが範囲内にいる場合、物理経由で追いかける（壁で正しく止まる）
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
        }
    }

    // Scene�r���[�Ŕ����͈͂�����
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }
}