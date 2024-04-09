using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool GamePaused = false;

    public List<Sheep> activeSheep;
    public List<Wolf> activeEnemies;
    public delegate void OnUpdateSheep();
    public delegate void OnUpdateEnemies();
    public OnUpdateSheep onUpdateSheepCallback;
    public OnUpdateEnemies onUpdateEnemiesCallback;

    public List<Level> levelData;
    public Level currentLevelData;

    public GameObject player;

    public InputAction pauseAction;

    /// <summary>
    /// Debug.Log all messages.
    /// </summary>
    public bool debugAll;
    /// <summary>
    /// Debug.Log messages related to state transitions and management for all characters using an FSM.
    /// </summary>
    public bool debugFSM;

    public int farthestSheep = -1;

    [SerializeField]
    Scene[] levelScenes;

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

    float maxSheepDistance = 30f;

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

        //fun tidbit! in this case, you can do instance = this and it will work without complaining. However, this is BAD! 
        //This is a Unity specific problem. Unity objects have an implicit conversion to bool, meaning if (instance = this) 
        //will overwrite whatever instance was equal to, set it equal to this, and that will always evaluate to true so the code
        //below will always run
        if (instance == this)
        {
            activeSheep = FindObjectsOfType<Sheep>().ToList();
            activeEnemies = FindObjectsOfType<Wolf>().ToList();
        }
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(CheckSheepDistanceFromPlayer());
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
        if (activeSheep.Count <= currentLevelData.minSheepToCompleteLevel)
        {
            GameOver();
        }
        onUpdateSheepCallback.Invoke();
    }

    IEnumerator CheckSheepDistanceFromPlayer()
    {
        while (true)
        {
            UpdateFarthestSheep();
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateFarthestSheep()
    {
        farthestSheep = -1;
        float farthestSheepDistance = 0f;
        for (int i = 0; i < activeSheep.Count; i++)
        {
            float distance = Vector3.Distance(activeSheep[i].transform.position, player.transform.position);
            if (distance > farthestSheepDistance)
            {
                farthestSheep = i;
                farthestSheepDistance = distance;
            }
        }
        if (farthestSheepDistance < maxSheepDistance)
        {
            farthestSheep = -1;
        }
    }

    public void UpdateActiveEnemies()
    {
        activeEnemies.Clear();
        activeEnemies = FindObjectsOfType<Wolf>().ToList();
        onUpdateEnemiesCallback.Invoke();
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
        int points = sheepSaved * currentLevelData.pointsPerSheep;
        pointsText.text = $"{points} points";
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(levelData[1].sceneName);
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
