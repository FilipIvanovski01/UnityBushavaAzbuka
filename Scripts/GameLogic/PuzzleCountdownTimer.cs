using UnityEngine;

/// <summary>
/// Counts down each frame while the round is active; notifies <see cref="PuzzleRunSession"/> on time up.
/// Call <see cref="BeginRound"/> after the grid is built (e.g. from <see cref="TableFactory"/>).
/// </summary>
public class PuzzleCountdownTimer : MonoBehaviour
{
    [SerializeField] private float durationSeconds = 120f;

    private float remaining;
    private bool started;

    public float DurationSeconds => durationSeconds;

    private void Update()
    {
        if (!started || PuzzleRunSession.RoundEnded)
        {
            PuzzleRunSession.SetTimeRemaining(remaining);
            return;
        }

        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            remaining = 0f;
            started = false;
            PuzzleRunSession.SetTimeRemaining(0f);
            PuzzleRunSession.NotifyTimeUp();
            return;
        }

        PuzzleRunSession.SetTimeRemaining(remaining);
    }

    /// <summary>Starts or restarts the countdown from <see cref="durationSeconds"/>.</summary>
    public void BeginRound()
    {
        if (PuzzleRunSession.RoundEnded)
            return;

        remaining = Mathf.Max(0.1f, durationSeconds);
        started = true;
        PuzzleRunSession.SetTimeRemaining(remaining);
    }
}
