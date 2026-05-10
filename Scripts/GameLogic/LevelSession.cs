/// <summary>
/// Holds the puzzle difficulty chosen on the level menu until the gameplay scene runs.
/// </summary>
public enum PuzzleDifficulty
{
    Easy,
    Medium,
    Hard,
    Bushava
}

public static class LevelSession
{
    public static PuzzleDifficulty SelectedDifficulty { get; set; } = PuzzleDifficulty.Medium;
}
