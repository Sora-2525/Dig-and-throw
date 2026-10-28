using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RetryGame()
    {
        PlayerMove.carriedTreasure = 0; // お宝所持数をリセット
        PlayerMove.hasBeamWeapon = false; // 武器をリセット
        SceneManager.LoadScene("Stage1");
    }
}