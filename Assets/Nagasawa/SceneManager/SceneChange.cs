using UnityEngine.SceneManagement;

public class SceneChange
{
    private static bool roadNow = false;
    /// <summary>
    /// �w��V�[���Ɉڍs����
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        if (roadNow)
        {
            return;
        }
        roadNow = true;
        FadeController.StartFadeOut(() => Load(sceneName));
    }  
    private static void Load(string sceneName)
    {
        roadNow = false;
        SceneManager.LoadScene(sceneName);
    }
}
