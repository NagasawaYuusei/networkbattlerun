using UnityEngine;

public class SceneController : MonoBehaviour
{
    void Start()
    {
        FadeController.StartFadeIn();
    }
    public static void ChangeScene(string target)
    {
        SceneChange.LoadScene(target);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
