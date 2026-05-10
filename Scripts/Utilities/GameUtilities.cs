using UnityEngine;
using UnityEngine.SceneManagement;
public class GameUtilities : MonoBehaviour
{
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
