using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private MenuSettings _menuSettings;
    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex==0)
        Time.timeScale = 1;
        _menuSettings = GameObject.FindObjectOfType<MenuSettings>().GetComponent<MenuSettings>();
    }

    public void Exit()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        if (_menuSettings.isAllGodsChoosen())
        {
            _menuSettings.Save();
            SceneManager.LoadScene("MapSelectionTest", LoadSceneMode.Single);
        }
        
    }
    public void BackToTheMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }
}
