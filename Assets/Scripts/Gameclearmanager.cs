using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    public void RetryGame()
    {
        PlayerMove.carriedTreasure = 0; // お宝所持数をリセット
        PlayerMove.hasBeamWeapon = false; // 武器をリセット
        SceneManager.LoadScene("Stage1");
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}