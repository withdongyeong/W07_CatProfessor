using UnityEngine;
using System.Linq;using UnityEditor.SceneManagement;
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
    /*
     * 마우스 클릭을 각 스테이지의 큰 원에 달아놓고, 다른 문제에서도 옆에 스테이지가 보이기 때문에,
     * 1. 퍼즐 풀이 상태, 스테이지 선택 상태를 구분하고,  Done
     * 2. 스테이지 선택 상태일때, dONE
     * 3. 각 스테이지의 원안을 클릭하면, Done 
     * 4. 퍼즐 풀이 상태로 바꾸고 dONE
     * 5. 그 스테이지 원에서 게임 매니저를 호출해서 자신을 현재 스테이지에 등록함 Done
     * 6. 그러면 게임 매니저입장에서는 각종 초기화를 현재 스테이지의 오브젝트들을 가져와서 함
     * 7. 카메라를 현재 스테이지의 중심으로 이동시키고
     * 8. 일정 시간 이동시간을 넣어서, 그 이동시간후에 UI를 on 하고
     * 9. 음 Draggable도 따로 구분해야할듯? 
     */

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
        // if (StageDataManager.Instance.IsEnding())
        // {
        //     Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameStart_Ending);
        // }
        // else
        // {
        //     Professor.Instance.SayRandom(ScriptManager.ScriptCategory.GameStart);
        // }
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
