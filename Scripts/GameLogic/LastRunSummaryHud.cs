using TMPro;
using UnityEngine;

/// <summary>
/// Displays last puzzle run summary on the level menu (or any scene). Reads <see cref="PuzzleRunSession.LastRun"/>.
/// </summary>
public class LastRunSummaryHud : MonoBehaviour
{
    [SerializeField] private TMP_Text summaryLine;
    [SerializeField] private string noRunText = "Last game: —";

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (summaryLine == null)
            return;

        PuzzleRunSession.LastRunSnapshot s = PuzzleRunSession.LastRun;
        if (!s.HasRun)
        {
            summaryLine.text = noRunText;
            return;
        }

        string result = s.Won ? "Win" : "Lose";
        summaryLine.text =
            $"Last game: {result} | Score {s.Score} | Words {s.WordsFound}/{s.TotalPlacedWords} | Time left {Mathf.CeilToInt(s.TimeRemainingSeconds)}s";
    }
}
