using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class DigController : MonoBehaviour
{
    [Header("Dig settings")]
    public Tilemap wallTilemap;        // diggable wall tilemap
    public float digRange = 1.2f;      // dig reach
    public GameObject treasurePrefab;  // dropped treasure

    [Header("Treasure drop")]
    [Range(0f, 1f)]
    public float dropRate = 0.4f;      // chance a wall drops treasure

    [Header("Dig as weak attack / effects")]
    public int digDamage = 1;            // damage dealt by digging
    public float digAttackRadius = 0.6f; // attack radius at the dig point
    public GameObject digEffect;         // crumbling-block effect
    public GameObject pickaxePrefab;     // pickaxe swing visual
    public float pickaxeOffset = 0.5f;   // distance from the player

    private PlayerMove playerMove;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Dig();
        }
    }

    void Dig()
    {
        Vector2 dir = playerMove.lastMoveDir;
        Vector3 digPos = transform.position + (Vector3)(dir * digRange);

        // Pickaxe swing visual (always)
        SwingPickaxe(dir);
        // Dig also works as a weak attack (always)
        AttackEnemies(digPos);

        // Dig the wall tile if present
        Vector3Int cellPos = wallTilemap.WorldToCell(digPos);
        if (wallTilemap.GetTile(cellPos) != null)
        {
            wallTilemap.SetTile(cellPos, null);
            Vector3 center = wallTilemap.GetCellCenterWorld(cellPos);
            if (digEffect != null) Instantiate(digEffect, center, Quaternion.identity);
            TryDropTreasure(center);
        }
    }

    // Spawn a short pickaxe-swing visual in the dig direction
    void SwingPickaxe(Vector2 dir)
    {
        if (pickaxePrefab == null) return;
        GameObject p = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
        p.transform.SetParent(transform);
        p.transform.localPosition = (Vector3)(dir.normalized * pickaxeOffset);
        PickaxeSwing sw = p.GetComponent<PickaxeSwing>();
        if (sw != null) sw.Init(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    // Weak attack: damage enemies/boss around the dig point
    void AttackEnemies(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, digAttackRadius);
        foreach (Collider2D h in hits)
        {
            Enemy e = h.GetComponentInParent<Enemy>();
            if (e != null) { e.TakeDamage(digDamage); continue; }
            BossEnemy b = h.GetComponentInParent<BossEnemy>();
            if (b != null) b.TakeDamage(digDamage);
        }
    }

    void TryDropTreasure(Vector3 position)
    {
        if (treasurePrefab == null) return;
        if (Random.value <= dropRate)
        {
            Instantiate(treasurePrefab, position, Quaternion.identity);
        }
    }
}
