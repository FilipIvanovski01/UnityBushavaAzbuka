using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static per-run state for the puzzle scene: timer sync, score, placed-word progress, win/lose, and last-run snapshot for the level menu.
/// </summary>
public static class PuzzleRunSession
{
    public struct LastRunSnapshot
    {
        public bool HasRun;
        public bool Won;
        public int Score;
        public int WordsFound;
        public int TotalPlacedWords;
        public float TimeRemainingSeconds;
    }

    public static event Action OnStatsChanged;
    public static event Action<bool> OnRoundEnded;

    private static readonly HashSet<string> PlacedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> FoundPlacedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static readonly List<string> FoundWordsOrdered = new List<string>();

    private static bool roundEnded;
    private static int score;
    private static int wordsFound;
    private static int totalPlaced;
    private static int pointsPerLetter;
    private static float timeRemainingSeconds;

    private static LastRunSnapshot lastRun;

    public static bool RoundEnded => roundEnded;
    public static int Score => score;
    public static int WordsFound => wordsFound;
    public static int TotalPlacedWords => totalPlaced;
    public static float TimeRemainingSeconds => timeRemainingSeconds;
    public static IReadOnlyList<string> FoundWords => FoundWordsOrdered;

    public static LastRunSnapshot LastRun => lastRun;

    /// <summary>Call when the grid is ready: only successfully placed words should be in <paramref name="placedWords"/>.</summary>
    public static void ResetForNewRun(IReadOnlyList<string> placedWords, int pointsPerLetterScore)
    {
        roundEnded = false;
        score = 0;
        wordsFound = 0;
        pointsPerLetter = Mathf.Max(0, pointsPerLetterScore);

        PlacedWords.Clear();
        FoundPlacedWords.Clear();
        FoundWordsOrdered.Clear();

        if (placedWords != null)
        {
            foreach (string w in placedWords)
            {
                if (string.IsNullOrWhiteSpace(w))
                    continue;

                string key = w.Trim().ToUpperInvariant();
                PlacedWords.Add(key);
            }
        }

        totalPlaced = PlacedWords.Count;
        timeRemainingSeconds = 0f;

        RaiseStatsChanged();

        if (totalPlaced == 0)
            NotifyRoundWon();
    }

    public static bool IsWordOnBoard(string normalizedUpperWord)
    {
        if (string.IsNullOrEmpty(normalizedUpperWord))
            return false;

        return PlacedWords.Contains(normalizedUpperWord);
    }

    /// <summary>Updates HUD timer display; driven by <see cref="PuzzleCountdownTimer"/>.</summary>
    public static void SetTimeRemaining(float seconds)
    {
        timeRemainingSeconds = Mathf.Max(0f, seconds);
        RaiseStatsChanged();
    }

    /// <summary>
    /// Call after a valid submission. Awards score and progress only for words that were placed on the board.
    /// Returns true if this was a newly found placed word.
    /// </summary>
    public static bool TryRegisterCorrectWord(string normalizedUpperWord)
    {
        if (roundEnded || string.IsNullOrEmpty(normalizedUpperWord))
            return false;

        if (!PlacedWords.Contains(normalizedUpperWord))
            return false;

        if (FoundPlacedWords.Contains(normalizedUpperWord))
            return false;

        FoundPlacedWords.Add(normalizedUpperWord);
        FoundWordsOrdered.Add(normalizedUpperWord);
        wordsFound++;

        if (pointsPerLetter > 0)
            score += pointsPerLetter * normalizedUpperWord.Length;
        else
            score += 1;

        RaiseStatsChanged();

        if (wordsFound >= totalPlaced && totalPlaced > 0)
            NotifyRoundWon();

        return true;
    }

    public static void NotifyTimeUp()
    {
        if (roundEnded)
            return;

        roundEnded = true;
        CaptureLastRun(false);
        OnRoundEnded?.Invoke(false);
        RaiseStatsChanged();
    }

    public static void NotifyRoundWon()
    {
        if (roundEnded)
            return;

        roundEnded = true;
        CaptureLastRun(true);
        OnRoundEnded?.Invoke(true);
        RaiseStatsChanged();
    }

    private static void CaptureLastRun(bool won)
    {
        lastRun = new LastRunSnapshot
        {
            HasRun = true,
            Won = won,
            Score = score,
            WordsFound = wordsFound,
            TotalPlacedWords = totalPlaced,
            TimeRemainingSeconds = timeRemainingSeconds
        };
    }

    private static void RaiseStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }
}
