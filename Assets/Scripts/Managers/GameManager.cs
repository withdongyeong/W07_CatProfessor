using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private OutputCircuit[] outputCircuits;
    private bool isGameOver = false;
    private float sayInterval = 60f;
    private float lastSayTime = 0f;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        outputCircuits = FindObjectsByType<OutputCircuit>(FindObjectsSortMode.None);
        lastSayTime = Time.time;

        if (outputCircuits.Length == 0)
        {
            Debug.LogWarning("출력 회로가 없음! 게임 클리어 체크를 중단합니다.");
            return;
        }

        Debug.Log($"총 출력 회로 개수: {outputCircuits.Length}");
        if (StageDataManager.Instance.IsEnding())
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameStart_Ending);
        }
        else
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameStart);
        }
    }

    void Update()
    {
        if (isGameOver || outputCircuits.Length == 0) return;

        if (outputCircuits.All(circuit => circuit.IsComplete()))
        {
            GameClear();
        }

        // ✅ 60초마다 대사 실행
        if (Time.time - lastSayTime >= sayInterval)
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.Compliment);
            lastSayTime = Time.time;
        }
    }

    void GameClear()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("게임 클리어! 모든 출력 회로가 충족됨.");
        Time.timeScale = 0f;
        UIManager.Instance.ShowRestartPanel();
        string currentScene = SceneManager.GetActiveScene().name;
        StageDataManager.Instance.SetStageCleared(currentScene);
        SoundManager.Instance.PlayClearMusic();

        if (StageDataManager.Instance.IsEnding())
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameWin_Ending);
        }
        else
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameWin);
        }
        Professor.Instance.SetAnimation(Professor.AnimationType.Victory);
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame() 실행됨!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isGameOver = false;
        InitializeGame();
    }
}
