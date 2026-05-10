using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TableFactory : MonoBehaviour
{
    [Header("Table Settings")]
    [SerializeField] private int rows = 6;
    [SerializeField] private int cols = 6;
    [SerializeField] private bool allowDiagonal = true;

    [Header("UI")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject letterCellPrefab;

    [Header("Run / scoring")]
    [SerializeField] private int pointsPerLetter = 10;

    [SerializeField] private string[] words;

    private LetterCellView[,] cellViews;

    private char[] randomLetters =
    {
        'А', 'Б', 'В', 'Г', 'Д', 'Ѓ', 'Е', 'Ж', 'З', 'Ѕ', 'И', 'Ј',
        'К', 'Л', 'Љ', 'М', 'Н', 'Њ', 'О', 'П', 'Р', 'С', 'Т', 'Ќ',
        'У', 'Ф', 'Х', 'Ц', 'Ч', 'Џ', 'Ш'
    };

    private char[,] grid;

    private enum WordDirection
    {
        Horizontal,
        Vertical,
        DiagonalRight,
        DiagonalLeft
    }

    private static readonly WordDirection[] AllDirections =
    {
        WordDirection.Horizontal,
        WordDirection.Vertical,
        WordDirection.DiagonalRight,
        WordDirection.DiagonalLeft
    };

    private static readonly WordDirection[] StraightDirections =
    {
        WordDirection.Horizontal,
        WordDirection.Vertical
    };

    private void Start()
    {
        WordLoading.LoadAllWords();

        string[] candidates = WordSelectionService.SelectWordsForPuzzle();
        var placedWords = new List<string>();
        CreateLetterGridFromCandidates(candidates, placedWords);

        words = placedWords.Count > 0 ? placedWords.ToArray() : System.Array.Empty<string>();

        SpawnGridUI();

        PuzzleRunSession.ResetForNewRun(placedWords, pointsPerLetter);

        PuzzleCountdownTimer timer = FindObjectOfType<PuzzleCountdownTimer>();
        if (timer != null && !PuzzleRunSession.RoundEnded)
            timer.BeginRound();

        WordDragSelector dragSelector = GetComponent<WordDragSelector>();
        if (dragSelector != null && cellViews != null)
            dragSelector.Initialize(cellViews, allowDiagonal, words);
    }

    private void CreateLetterGridFromCandidates(IReadOnlyList<string> candidates, List<string> placedWords)
    {
        grid = new char[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                grid[row, col] = '-';
            }
        }

        if (candidates == null)
        {
            FillEmptySpaces();
            return;
        }

        foreach (string raw in candidates)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            string word = raw.Trim().ToUpperInvariant();
            if (TryPlaceWord(word))
                placedWords.Add(word);
        }

        FillEmptySpaces();
    }

    private bool TryPlaceWord(string word)
    {
        bool placed = false;
        int trials = 0;

        while (!placed && trials < 500)
        {
            trials++;
            int row = Random.Range(0, rows);
            int col = Random.Range(0, cols);
            WordDirection direction = GetRandomDirection();

            if (!CanPlaceWord(word, row, col, direction))
                continue;

            WriteWord(word, row, col, direction);
            placed = true;
        }

        if (!placed)
            Debug.LogWarning($"Unable to place word '{word}' after {trials} attempts.");

        return placed;
    }

    private WordDirection GetRandomDirection()
    {
        if (allowDiagonal)
            return AllDirections[Random.Range(0, AllDirections.Length)];

        return StraightDirections[Random.Range(0, StraightDirections.Length)];
    }

    private bool CanPlaceWord(string word, int row, int col, WordDirection direction)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        int dr = 0;
        int dc = 0;

        switch (direction)
        {
            case WordDirection.Horizontal:
                dc = 1;
                break;
            case WordDirection.Vertical:
                dr = 1;
                break;
            case WordDirection.DiagonalRight:
                dr = 1;
                dc = 1;
                break;
            case WordDirection.DiagonalLeft:
                dr = 1;
                dc = -1;
                break;
        }

        for (int i = 0; i < word.Length; i++)
        {
            int r = row + dr * i;
            int c = col + dc * i;

            if (r < 0 || r >= rows || c < 0 || c >= cols)
                return false;

            if (grid[r, c] != '-' && grid[r, c] != word[i])
                return false;
        }

        return true;
    }

    private void WriteWord(string word, int row, int col, WordDirection direction)
    {
        if (string.IsNullOrEmpty(word))
            return;

        int dr = 0;
        int dc = 0;

        switch (direction)
        {
            case WordDirection.Horizontal:
                dc = 1;
                break;
            case WordDirection.Vertical:
                dr = 1;
                break;
            case WordDirection.DiagonalRight:
                dr = 1;
                dc = 1;
                break;
            case WordDirection.DiagonalLeft:
                dr = 1;
                dc = -1;
                break;
        }

        for (int i = 0; i < word.Length; i++)
        {
            int r = row + dr * i;
            int c = col + dc * i;
            grid[r, c] = word[i];
        }
    }

    private void FillEmptySpaces()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row, col] == '-')
                    grid[row, col] = randomLetters[Random.Range(0, randomLetters.Length)];
            }
        }
    }

    private void SpawnGridUI()
    {
        if (letterCellPrefab == null || gridParent == null)
        {
            Debug.LogError("TableFactory: letterCellPrefab and gridParent must be assigned.");
            return;
        }

        cellViews = new LetterCellView[rows, cols];

        AlphabetAtlasCache.EnsureLoaded();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject cell = Instantiate(letterCellPrefab, gridParent);
                LetterCellView view = cell.GetComponent<LetterCellView>();
                if (view == null)
                    view = cell.AddComponent<LetterCellView>();

                view.Configure(row, col, grid[row, col]);
                cellViews[row, col] = view;
            }
        }
    }
}
