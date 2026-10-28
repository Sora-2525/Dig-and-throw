using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalDoor : MonoBehaviour
{
    public string nextScene = "Stage2"; // Ÿ‚ÌƒV[ƒ“–¼

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}