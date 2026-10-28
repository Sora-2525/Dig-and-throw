using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int treasureValue = 1; // お宝の個数

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // PlayerMoveのお宝カウントを増やす
            PlayerMove player = other.GetComponent<PlayerMove>();
            if (player != null)
            {
                player.treasureCount += treasureValue;
                Debug.Log("お宝を拾った！所持数：" + player.treasureCount);
            }
            Destroy(gameObject);
        }
    }
}