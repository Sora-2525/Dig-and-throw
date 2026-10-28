using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("参照")]
    public GameObject panel;        // 操作説明パネル（最初は非表示）
    public GameObject pauseButton;  // ポーズ（操作方法）ボタン

    void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
        Time.timeScale = 1f; // 念のため再開状態で開始
    }

    // ポーズして操作説明を表示（ボタンに割り当て）
    public void Pause()
    {
        if (panel != null) panel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
        Time.timeScale = 0f;
    }

    // 再開して閉じる（「とじる」ボタンに割り当て）
    public void Resume()
    {
        if (panel != null) panel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    // シーン遷移などで破棄される際、必ず再開状態に戻す（次シーンが止まらないように）
    void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
