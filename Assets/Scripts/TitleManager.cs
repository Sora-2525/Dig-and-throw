using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [Header("キャラクター演出")]
    public SpriteRenderer characterSprite;
    public Sprite[] idleFrames;        // idle アニメーションフレーム
    public float animInterval = 0.15f;

    [Header("UI")]
    public GameObject pressAnyKeyText; // 点滅させるテキストオブジェクト

    private int frameIndex = 0;
    private bool inputEnabled = true;

    void Start()
    {
        StartCoroutine(AnimateCharacter());
        if (pressAnyKeyText != null)
            StartCoroutine(BlinkText());
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.anyKeyDown)
        {
            inputEnabled = false;
            StartCoroutine(StartGame());
        }
    }

    IEnumerator AnimateCharacter()
    {
        if (characterSprite == null || idleFrames == null || idleFrames.Length == 0)
            yield break;

        while (true)
        {
            characterSprite.sprite = idleFrames[frameIndex % idleFrames.Length];
            frameIndex++;
            yield return new WaitForSeconds(animInterval);
        }
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            pressAnyKeyText.SetActive(!pressAnyKeyText.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator StartGame()
    {
        // フェードアウト後にシーン遷移
        yield return new WaitForSeconds(0.3f);
        PlayerMove.carriedTreasure = 0; // 新規ゲーム開始時にお宝をリセット
        PlayerMove.hasBeamWeapon = false; // 武器をリセット
        PlayerMove.savedHP = -1;          // HPもリセット（満タンから）
        SceneManager.LoadScene("Stage1");
    }
}
