using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows win/lose overlay. Assign UI Buttons or wire UnityEvents to <see cref="GoToLevelMenu"/> / <see cref="RegenerateSameLevel"/>.
/// </summary>
public class PuzzleRoundEndPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button goToLevelMenuButton;
    [SerializeField] private Button regenerateSameLevelButton;
    [SerializeField] private string winTitle = "You won!";
    [SerializeField] private string loseTitle = "Time's up!";
    [SerializeField] private string levelMenuSceneName = "LevelChoosing";

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (panelRoot == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        PuzzleRunSession.OnRoundEnded += HandleRoundEnded;
        if (goToLevelMenuButton != null)
            goToLevelMenuButton.onClick.AddListener(GoToLevelMenu);
        if (regenerateSameLevelButton != null)
            regenerateSameLevelButton.onClick.AddListener(RegenerateSameLevel);
        HideOverlay();
    }

    private void OnDisable()
    {
        PuzzleRunSession.OnRoundEnded -= HandleRoundEnded;
        if (goToLevelMenuButton != null)
            goToLevelMenuButton.onClick.RemoveListener(GoToLevelMenu);
        if (regenerateSameLevelButton != null)
            regenerateSameLevelButton.onClick.RemoveListener(RegenerateSameLevel);
    }

    private void HideOverlay()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void ShowOverlay()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void HandleRoundEnded(bool won)
    {
        ShowOverlay();

        if (titleText != null)
            titleText.text = won ? winTitle : loseTitle;
    }

    /// <summary>Loads the level-selection scene (same name as in Build Settings).</summary>
    public void GoToLevelMenu()
    {
        if (string.IsNullOrWhiteSpace(levelMenuSceneName))
        {
            Debug.LogError("PuzzleRoundEndPanel: levelMenuSceneName is empty.");
            return;
        }

        SceneManager.LoadScene(levelMenuSceneName);
    }

    /// <summary>Reloads the current puzzle scene (new grid, same difficulty).</summary>
    public void RegenerateSameLevel()
    {
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex);
    }

    public void RestartCurrentPuzzle() => RegenerateSameLevel();

    public void LoadLevelMenu() => GoToLevelMenu();
}
