using UnityEngine;
using TMPro;

public class TreasureUI : MonoBehaviour
{
    public TextMeshProUGUI treasureText;
    public PlayerMove player;

    void Update()
    {
        treasureText.text = "所持数 x " + player.treasureCount;
    }
}