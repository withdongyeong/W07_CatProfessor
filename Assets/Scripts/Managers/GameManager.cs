using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum gameState
    {
        GamePlaying,
        StageSelecting
    }

    private gameState currentGameState;
    private GameObject currentPlayingStage;
    private OutputCircuit[] outputCircuits;
    public CameraController mainCamera;
    private GridManager _grid;
    private OneSceneUIManager _uiManager;

    private bool isGameOver = false;
    private float sayInterval = 60f;
    private float lastSayTime = 0f;

    [Header("기본 월드 카메라 설정")]
    public Vector3 worldViewPosition = Vector3.zero;
    private int worldViewSize = 40;
    
    public static GameManager Instance { get; private set; }
    
    // Getter & Setter
    public gameState CurrentGameState
    {
        get => currentGameState;
        set
        {
            currentGameState = value;
            _grid.ActivateGrid(currentGameState == gameState.GamePlaying);
            _uiManager.ActivatePlayingCanvas(currentGameState == gameState.GamePlaying);
        }
    }
    
    public GameObject CurrentPlayingStage
    {
        get => currentPlayingStage;
        set => currentPlayingStage = value;
    }
    
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
        currentGameState = gameState.StageSelecting;
    }
    
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main.GetComponent<CameraController>();
        
        _grid = mainCamera.GetComponentInChildren<GridManager>();
        _uiManager = FindAnyObjectByType<OneSceneUIManager>();
        
        InitializeGame();
    }

    void InitializeGame()
    {
        // 현재 출력회로들 다 초기화(스테이지갔다가 다시 오면 바로 클리어되기 때문)
        List<OutputCircuit> currentOutputCircuits = currentPlayingStage.GetComponentInChildren<StateManager>().OutputCircuits;
        if (currentOutputCircuits.Count == 0) return;
        foreach (OutputCircuit circuit in currentOutputCircuits)
        {
            circuit.ResetCounter();
        }
        
        isGameOver = false;
        Time.timeScale = 1f;
        lastSayTime = Time.time;
        // ManaPool.Instance.ResetManaPool();

        // if (StageDataManager.Instance.IsEnding())
        // {
        //     Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameStart_Ending);
        // }
        // else
        // {
        //     Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameStart);
        // }
    }

    private void checkGameClear()
    {
        if (currentPlayingStage != null)
        {
            List<OutputCircuit> currentOutputCircuits = currentPlayingStage.GetComponentInChildren<StateManager>().OutputCircuits;
            if (isGameOver || currentOutputCircuits.Count == 0) return;
            
            if (currentOutputCircuits.All(circuit => circuit.IsComplete()))
            {
                GameClear();
            }    
        }
        
        
    }
    void Update()
    {
        if (CurrentGameState == gameState.GamePlaying) checkGameClear();
        
        // 60초마다 교수님 대사 실행
        if (Time.time - lastSayTime >= sayInterval)
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.Compliment);
            lastSayTime = Time.time;
        }
    }

    public void EnterStage(GameObject stageRoot)
    {
        CurrentPlayingStage = stageRoot;
        CurrentGameState = gameState.GamePlaying;

        StateManager stateManager = stageRoot.GetComponentInChildren<StateManager>();
        int currentViewSize = Mathf.RoundToInt(stateManager.MainCircle.diameter / 2f);

        // 카메라 이동
        mainCamera.MoveToStage(stageRoot.transform.position, currentViewSize);
        
        // 클릭 collider 해제
        stateManager.MainCircle.GetComponentInChildren<ClickableCircle>().gameObject.SetActive(false);
    }


    public void ExitStage()
    {
        InitializeGame();
        if (CurrentPlayingStage != null)
        {
            var stateManager = CurrentPlayingStage.GetComponentInChildren<StateManager>();
            // 클릭 collider 활성화
            stateManager.MainCircle.GetComponentInChildren<ClickableCircle>(true).gameObject.SetActive(true);
        }

        CurrentPlayingStage = null;
        CurrentGameState = gameState.StageSelecting;

        mainCamera.MoveToWorld(worldViewPosition, worldViewSize);
        
    }

    
    void GameClear()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("게임 클리어! 모든 출력 회로가 충족됨.");
        Time.timeScale = 0f;
        // UIManager.Instance.ShowRestartPanel();
        string currentScene = SceneManager.GetActiveScene().name;
        // TODO 스테이지 클리어 저장
        // StageDataManager.Instance.SetStageCleared(currentScene);
        // TODO 확인하고 다시 켜자
        // SoundManager.Instance.PlayClearMusic();

        // if (StageDataManager.Instance.IsEnding())
        // {
        //     Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameWin_Ending);
        // }
        // else
        // {
        //     Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameWin);
        // }
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
