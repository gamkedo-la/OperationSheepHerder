using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool GamePaused = false;

    public List<Sheep> activeSheep;
    public List<Wolf> activeWolves;
    public delegate void OnUpdateSheep();
    public delegate void OnUpdateWolves();
    public OnUpdateSheep onUpdateSheepCallback;
    public OnUpdateWolves onUpdateWolvesCallback;

    public List<Level> levels;
    public Level currentLevel;

    public InputAction pauseAction;

    /// <summary>
    /// Debug.Log all messages.
    /// </summary>
    public bool debugAll;
    /// <summary>
    /// Debug.Log messages related to state transitions and management for all characters using an FSM.
    /// </summary>
    public bool debugFSM;

    [SerializeField]
    GameObject sheepPrefab;

    [SerializeField]
    GameObject wolfPrefab;

    [SerializeField]
    float spawnRadiusForSheep;

    [SerializeField] 
    GameObject pauseMenu;

    [SerializeField]
    GameObject levelCompletePanel;

    [SerializeField]
    GameObject gameOverPanel;

    [SerializeField]
    TextMeshProUGUI sheepSavedText;

    [SerializeField]
    TextMeshProUGUI pointsText;

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
        activeSheep = FindObjectsOfType<Sheep>().ToList();
        activeWolves = FindObjectsOfType<Wolf>().ToList();
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        //Separate scene for each level
        string sceneName = SceneManager.GetActiveScene().name;

/*        if (sceneName.Contains("Level1"))
        {
            SetupLevel();
        }*/

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
            SetupLevel();
        }
    }

    void SetupLevel()
    {
        for (int i = 0; i < currentLevel.sheepCount;)
        {
            //GameObject tmp = Instantiate(sheepPrefab);
            //TODO: tmp.transform.position = random location within spawnRadiusForSheep around Player's SheepSpawner GameObject
        }
    }

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
        activeSheep = FindObjectsOfType<Sheep>().ToList();
        if (activeSheep.Count <= currentLevel.minSheepToCompleteLevel)
        {
            GameOver();
        }
        onUpdateSheepCallback.Invoke();
    }

    public void UpdateActiveWolves()
    {
        activeWolves.Clear();
        activeWolves = FindObjectsOfType<Wolf>().ToList();
        onUpdateWolvesCallback.Invoke();
    }

    public void PlayAgain()
    {
        Debug.Log("Play again");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LevelComplete()
    {
        Time.timeScale = 0;
        Debug.Log("Level Complete!");
        //show level complete text
        levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
        int sheepSaved = activeSheep.Count;
        sheepSavedText.text = $"You saved {sheepSaved} sheep!";
        int points = sheepSaved * currentLevel.pointsPerSheep;
        pointsText.text = $"{points} points";
    }

    public void GameOver()
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("game over panel not connected to scene");
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
