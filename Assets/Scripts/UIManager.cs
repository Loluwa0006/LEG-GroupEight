using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] TMP_Text statusDisplay;

    [SerializeField] ScoreManager scoreManager;
    public void SetDisplayString(string message)
    {
        statusDisplay.text = message;
    }

    public void OnNoteMissed(NoteEntity note)
    {
        scoreManager.EndStreak();
        animator.Play("NoteMisplayed", 0, 0.0f);
    }

    public void OnNotePlayed(NoteEntity note, bool isPerfect)
    {
        if (isPerfect)
        {
            animator.Play("NotePerfect", 0, 0.0f);
        }
        else
        {
            animator.Play("NoteGood", 0, 0.0f);

        }
        scoreManager.IncrementStreak(isPerfect);
        scoreManager.IncreaseScore();
    }

    public void OnStreakLost(NoteClearer clearer)
    {
        scoreManager.EndStreak();
    }
}
