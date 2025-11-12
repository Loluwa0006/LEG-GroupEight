using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public const float streakInfluence = 0.08f;

    [HideInInspector] public int score = 0;
    [HideInInspector] public int streak = 1;

    [Header("UI Elements")]
    [SerializeField] TMP_Text streakTracker;
    [SerializeField] TMP_Text scoreTracker;

    private void Awake()
    {
        scoreTracker.text = score.ToString("D6");
        EndStreak();
    }

    public void IncrementStreak(bool isPerfect)
    {
        streak += 1;
        if (isPerfect) streak += 1;
        streakTracker.text = "x" + streak.ToString();
    }

    public void IncreaseScore()
    {
        score += GetScoreIncreaseAmount();
        scoreTracker.text = score.ToString("D6");
    }

    public void EndStreak()
    {
        streak = 0;
        streakTracker.text = "";
    }

    public int GetScoreIncreaseAmount()
    {
        return Mathf.CeilToInt(Mathf.Pow((0.1f * Mathf.Max(1,streak)), 2.0f));
    }


}

