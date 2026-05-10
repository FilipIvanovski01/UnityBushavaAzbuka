using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TableFactory))]
public class WordDragSelector : MonoBehaviour
{
    [SerializeField] private int minWordLength = 3;
    [SerializeField] private float wrongResetDelaySeconds = 1.5f;

    [Header("Audio — assign clips in Inspector; add AudioSource on this object for 2D UI (Spatial Blend = 0)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip swipeStepClip;
    [SerializeField] private AudioClip wordCorrectClip;
    [SerializeField] private AudioClip wordIncorrectClip;
    [SerializeField] [Range(0f, 1f)] private float swipeStepVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float wordCorrectVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float wordIncorrectVolume = 1f;

    private IInputHandler inputHandler;

    private bool dragging;
    private readonly List<LetterCellView> path = new List<LetterCellView>();
    private readonly List<LetterCellView> lastSuccessPath = new List<LetterCellView>();

    private LetterCellView[,] cells;
    private bool allowDiagonal;
    private HashSet<string> puzzleWords;

    private static readonly List<RaycastResult> RaycastBuffer = new List<RaycastResult>();

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void Initialize(
        LetterCellView[,] cellGrid,
        bool diagonalAllowed,
        IEnumerable<string> puzzleWordList)
    {
        cells = cellGrid;
        allowDiagonal = diagonalAllowed;

        puzzleWords = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
        if (puzzleWordList != null)
        {
            foreach (string w in puzzleWordList)
            {
                if (!string.IsNullOrWhiteSpace(w))
                    puzzleWords.Add(w.Trim().ToUpperInvariant());
            }
        }
    }

    private void Update()
    {
        if (cells == null)
            return;

        if (inputHandler == null)
            inputHandler = InputManager.InputHandler;

        if (inputHandler == null)
            return;

        if (inputHandler.PointerDownThisFrame)
        {
            if (dragging)
            {
                foreach (LetterCellView c in path)
                    c.ResetDefault();
                path.Clear();
                dragging = false;
            }

            LetterCellView hit = RaycastForCell(inputHandler.PointerPosition);
            if (hit == null)
                return;

            ClearPreviousSuccessHighlights();
            dragging = true;
            path.Clear();
            path.Add(hit);
            hit.SetHighlightSelecting();
            PlaySwipeStep();
        }
        else if (dragging && inputHandler.PointerHeld)
        {
            LetterCellView hit = RaycastForCell(inputHandler.PointerPosition);
            if (hit == null)
                return;

            if (path.Count >= 2 && hit == path[path.Count - 2])
            {
                LetterCellView popped = path[path.Count - 1];
                path.RemoveAt(path.Count - 1);
                popped.ResetDefault();
                PlaySwipeStep();
                return;
            }

            if (path.Count > 0 && hit == path[path.Count - 1])
                return;

            if (path.Contains(hit))
                return;

            if (!IsAdjacent(path[path.Count - 1], hit))
                return;

            path.Add(hit);
            hit.SetHighlightSelecting();
            PlaySwipeStep();
        }
        else if (dragging && inputHandler.PointerUpThisFrame)
        {
            dragging = false;
            CommitSelection();
        }
    }

    private void ClearPreviousSuccessHighlights()
    {
        foreach (LetterCellView cell in lastSuccessPath)
        {
            if (cell != null)
                cell.ResetDefault();
        }

        lastSuccessPath.Clear();
    }

    private LetterCellView RaycastForCell(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return null;

        RaycastBuffer.Clear();
        var eventData = new PointerEventData(EventSystem.current) { position = screenPosition };
        EventSystem.current.RaycastAll(eventData, RaycastBuffer);

        for (int i = 0; i < RaycastBuffer.Count; i++)
        {
            GameObject go = RaycastBuffer[i].gameObject;
            LetterCellView cell = go.GetComponent<LetterCellView>();
            if (cell == null)
                cell = go.GetComponentInParent<LetterCellView>();

            if (cell != null)
                return cell;
        }

        return null;
    }

    private bool IsAdjacent(LetterCellView a, LetterCellView b)
    {
        int dr = Mathf.Abs(a.Row - b.Row);
        int dc = Mathf.Abs(a.Col - b.Col);

        if (dr == 0 && dc == 0)
            return false;

        if (allowDiagonal)
            return dr <= 1 && dc <= 1;

        return dr + dc == 1;
    }

    private void CommitSelection()
    {
        if (path.Count == 0)
            return;

        var pathSnapshot = new List<LetterCellView>(path);
        string word = BuildWordFromPath(pathSnapshot);
        bool valid = ValidateWord(word);

        if (valid)
        {
            foreach (LetterCellView cell in pathSnapshot)
                cell.SetResultSuccess();

            lastSuccessPath.Clear();
            lastSuccessPath.AddRange(pathSnapshot);
            PlayWordCorrect();
        }
        else
        {
            foreach (LetterCellView cell in pathSnapshot)
                cell.SetResultFail();

            StartCoroutine(ResetFailPathAfterDelay(pathSnapshot));
            PlayWordIncorrect();
        }

        path.Clear();
    }

    private IEnumerator ResetFailPathAfterDelay(List<LetterCellView> snapshot)
    {
        yield return new WaitForSeconds(wrongResetDelaySeconds);

        foreach (LetterCellView cell in snapshot)
        {
            if (cell != null)
                cell.ResetDefault();
        }
    }

    private static string BuildWordFromPath(List<LetterCellView> pathCells)
    {
        var sb = new StringBuilder();
        foreach (LetterCellView cell in pathCells)
        {
            char ch = cell.GetLetter();
            if (ch != '\0')
                sb.Append(ch);
        }

        return sb.ToString().Trim().ToUpperInvariant();
    }

    private bool ValidateWord(string word)
    {
        if (word.Length < minWordLength)
            return false;

        if (puzzleWords != null && puzzleWords.Contains(word))
            return true;

        return WordLoading.GetAllWords().Contains(word);
    }

    private void PlaySwipeStep()
    {
        if (audioSource == null || swipeStepClip == null)
            return;

        audioSource.PlayOneShot(swipeStepClip, swipeStepVolume);
    }

    private void PlayWordCorrect()
    {
        if (audioSource == null || wordCorrectClip == null)
            return;

        audioSource.PlayOneShot(wordCorrectClip, wordCorrectVolume);
    }

    private void PlayWordIncorrect()
    {
        if (audioSource == null || wordIncorrectClip == null)
            return;

        audioSource.PlayOneShot(wordIncorrectClip, wordIncorrectVolume);
    }
}
