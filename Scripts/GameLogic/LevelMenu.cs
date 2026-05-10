using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Wire each public method to a Button OnClick in the Inspector (e.g. LoadEasy).
/// Set <see cref="gameplaySceneName"/> to the name of your puzzle/table scene as it appears in Build Settings.
/// </summary>
public class LevelMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "PuzzleTable";

    public void LoadEasy()
    {
        StartGame(PuzzleDifficulty.Easy);
    }

    public void LoadMedium()
    {
        StartGame(PuzzleDifficulty.Medium);
    }

    public void LoadHard()
    {
        StartGame(PuzzleDifficulty.Hard);
    }

    public void LoadBushava()
    {
        StartGame(PuzzleDifficulty.Bushava);
    }

    private void StartGame(PuzzleDifficulty difficulty)
    {
        LevelSession.SelectedDifficulty = difficulty;

        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogError("LevelMenu: gameplaySceneName is empty. Assign your puzzle scene name in the Inspector.");
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }
}
