using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneProvider : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void LoadNormalMode()
    {
        SceneManager.LoadScene(1);
    }
    
    public void LoadSpecialMode()
    {
        SceneManager.LoadScene(2);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
