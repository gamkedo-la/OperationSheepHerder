using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Sheep> activeSheep;
    public List<Wolf> activeWolves;
    public delegate void OnUpdateSheep();
    public delegate void OnUpdateWolves();
    public OnUpdateSheep onUpdateSheepCallback;
    public OnUpdateWolves onUpdateWolvesCallback;

    public InputAction pauseAction;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        activeSheep = FindObjectsOfType<Sheep>().ToList<Sheep>();
        activeWolves = FindObjectsOfType<Wolf>().ToList<Wolf>();
        
    }
 
    public static bool GamePaused = false;

    public List<Level> levels;
    public Level currentLevel;

    [SerializeField] GameObject pauseMenu;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnPauseGame();
        }
    }
    private void OnEnable()
    {
        pauseAction = new InputAction("GameManager/PauseGame");
        pauseAction.performed += ctx => OnPauseGame();
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        if (sceneName.Contains("Level"))
        {
            currentLevel = levels.Find(x => x.name == sceneName);
        }
    }

/*    private void OnLevelWasLoaded(int level)
    {
        DynamicGI.UpdateEnvironment();
    }*/

    public void OnPauseGame()
    {
        Debug.Log("Game paused");
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        GamePaused = !GamePaused;
        if (GamePaused == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void UpdateActiveSheep()
    {
        activeSheep.Clear();
        activeSheep = FindObjectsOfType<Sheep>().ToList<Sheep>();
        onUpdateSheepCallback.Invoke();
    }

    public void UpdateActiveWolves()
    {
        activeWolves.Clear();
        activeWolves = FindObjectsOfType<Wolf>().ToList<Wolf>();
        onUpdateWolvesCallback.Invoke();
    }
    public void LevelComplete()
    {
        Time.timeScale = 0;
        Debug.Log("Level Complete!");
        //show level complete text
    }

    public void LoadSettingsMenu()
    {
        //open settings menu
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
