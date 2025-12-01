using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : MonoBehaviour
{
    public int score { get; private set; }
    public UnityEvent<int> onScoreChanged;   // broadcasts new score

    public void Add(int amount = 1)
    {
        score += amount;
        onScoreChanged?.Invoke(score);
    }

    public void ResetScore(int to = 0)
    {
        score = to;
        onScoreChanged?.Invoke(score);
    }
}
