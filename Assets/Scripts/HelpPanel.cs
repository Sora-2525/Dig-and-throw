using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    [Header("操作説明パネル本体")]
    public GameObject panel;   // 表示・非表示を切り替えるパネル

    void Start()
    {
        // 最初は閉じておく
        if (panel != null)
            panel.SetActive(false);
    }

    // 開く（「操作方法」ボタンに割り当て）
    public void OpenPanel()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    // 閉じる（「閉じる」ボタンに割り当て）
    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    // 開閉を切り替える（1つのボタンでトグルしたい場合）
    public void TogglePanel()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }
}
