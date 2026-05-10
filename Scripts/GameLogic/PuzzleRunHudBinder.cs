using TMPro;
using UnityEngine;

/// <summary>
/// Binds <see cref="PuzzleRunSession"/> stats to TMP labels on the puzzle scene.
/// </summary>
public class PuzzleRunHudBinder : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text wordsProgressText;
    [SerializeField] private TMP_Text foundWordsListText;

    private void OnEnable()
    {
        PuzzleRunSession.OnStatsChanged += Refresh;
        PuzzleRunSession.OnRoundEnded += OnRoundEnded;
        Refresh();
    }

    private void OnDisable()
    {
        PuzzleRunSession.OnStatsChanged -= Refresh;
        PuzzleRunSession.OnRoundEnded -= OnRoundEnded;
    }

    private void OnRoundEnded(bool _)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (timerText != null)
            timerText.text = $"Време: {Mathf.CeilToInt(PuzzleRunSession.TimeRemainingSeconds)}s";

        if (scoreText != null)
            scoreText.text = $"Поени: {PuzzleRunSession.Score}";

        if (wordsProgressText != null)
            wordsProgressText.text =  $"Зборови:{PuzzleRunSession.WordsFound}/{PuzzleRunSession.TotalPlacedWords}";

        if (foundWordsListText != null)
        {
            if (PuzzleRunSession.FoundWords.Count == 0)
            {
                foundWordsListText.text = "Пронајдени зборови:\n-";
            }
            else
            {
                foundWordsListText.text = "Пронајдени зборови:\n" + string.Join("\n", PuzzleRunSession.FoundWords);
            }
        }
    }
}
