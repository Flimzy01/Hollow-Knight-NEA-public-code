using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    //loads the last scene in unless there is non stored where the scene loaded will be kings pass
    public void Play()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "KingsPass");
        SceneManager.LoadScene(lastScene);
    }


    public void Quit()
    {
        Application.Quit();
    }
}
