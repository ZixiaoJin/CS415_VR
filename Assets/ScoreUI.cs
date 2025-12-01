using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public PlayerScore playerScore;
    public TMP_Text scoreText;

    void Start()
    {
        if (playerScore)
        {
            playerScore.onScoreChanged.AddListener(UpdateText);
            UpdateText(playerScore.score);
        }
    }

    void OnDestroy()
    {
        if (playerScore)
            playerScore.onScoreChanged.RemoveListener(UpdateText);
    }

    void UpdateText(int s)
    {
        if (scoreText) scoreText.text = s.ToString();
    }
}
