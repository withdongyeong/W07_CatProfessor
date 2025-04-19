using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum gameState
    {
        GamePlaying,
        StageSelecting,
        Title,
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
    
    // 리셋 카운트
    [Tooltip("리셋 횟수")]
    public int resetCount { get; private set; }

    public event Action OnReset;

    [Header("기본 월드 카메라 설정")]
    public Vector3 worldViewPosition = Vector3.zero;
    private int worldViewSize = 110;
    
    public static GameManager Instance { get; private set; }
    
    // Getter & Setter
    public gameState CurrentGameState
    {
        get => currentGameState;
        set
        {
            currentGameState = value;
            _uiManager.ChangeUI(currentGameState);
            _grid.ActivateGrid(currentGameState == gameState.GamePlaying);
        }
    }

    public GameObject CurrentPlayingStage
    {
        get => currentPlayingStage;
        set => currentPlayingStage = value;
    }

    public bool IsGameOver
    {
        get => isGameOver;
        set => isGameOver = value;
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
        
        if (mainCamera == null)
            mainCamera = Camera.main.GetComponent<CameraController>();
        _grid = mainCamera.GetComponentInChildren<GridManager>();
        
        _uiManager = FindAnyObjectByType<OneSceneUIManager>();
        _uiManager.Init();
        
        CurrentGameState = gameState.Title;
    }
    
    void Start()
    {
        IsGameOver = false;
        InitializeGame();
        ApplyStageVisualStates();
        
        
    }

    void InitializeGame()
    {
        if (CurrentPlayingStage == null) return; 
        // 현재 출력회로들 다 초기화(스테이지갔다가 다시 오면 바로 클리어되기 때문)
        List<OutputCircuit> currentOutputCircuits = currentPlayingStage.GetComponentInChildren<StateManager>().OutputCircuits;
        if (currentOutputCircuits.Count == 0) return;
        foreach (OutputCircuit circuit in currentOutputCircuits)
        {
            circuit.ResetCounter();
        }
        
        IsGameOver = false;
        _uiManager.PlayingUI.ActivateResetBtns(!isGameOver);
        
        Time.timeScale = 1f;
        lastSayTime = Time.time;
        ManaPool.Instance.ResetManaPool();

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
            if (IsGameOver || currentOutputCircuits.Count == 0) return;
            
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

        // 하이라이트 끔
        foreach (var circle in stateManager.ManaCircles)
        {
            circle.SetHighlight(false); 
        }
        // 회전 및 색상 복구
        foreach (var circle in stateManager.ManaCircles)
        {
            circle.StartRotation();
            circle.SetDefaultColor();
        }

        void ApplyDefaultToCircuits<T>(List<T> list) where T : MonoBehaviour
        {
            foreach (var item in list)
            {
                if (item is IColorable colorable)
                {
                    colorable.SetDefaultColor();
                }
            }
        }

        ApplyDefaultToCircuits(stateManager.InputCircuits);
        ApplyDefaultToCircuits(stateManager.OutputCircuits);
        ApplyDefaultToCircuits(stateManager.AttributeCircuits);
        ApplyDefaultToCircuits(stateManager.Draggables);
        ApplyDefaultToCircuits(stateManager.NeutralCircuits);

        // 카메라 이동
        mainCamera.MoveToStage(stageRoot.transform.position, currentViewSize);

        // 클릭 collider 해제
        stateManager.MainCircle.GetComponentInChildren<ClickableCircle>().gameObject.SetActive(false);

        // 스테이지 초기화
        stateManager.ResetManaCircle();
        stateManager.ResetDraggable();
        
        //교수님한테 현재 StateManager 전달
        Professor.Instance.SetCurrentStage(stateManager, hintManager: stageRoot.GetComponentInChildren<HintManager>());
        // 스테이지 리셋 횟수 초기화
        resetCount = 0;
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

        ApplyStageVisualStates();
        mainCamera.MoveToWorld(worldViewPosition, worldViewSize);
        
        // 교수님 스테이지 초기화
        Professor.Instance.ResetStage();
        
        // 스테이지 리셋 횟수 초기화
        resetCount = 0;
    }

    private void ApplyStageVisualStates()
    {
        var allStages = FindObjectsOfType<StageRootMarker>();

        foreach (var stageMarker in allStages)
        {
            string stageName = stageMarker.gameObject.name;
            StageStatus status = StageDataManager.Instance.GetStageStatus(stageName);

            var stateManager = stageMarker.GetComponentInChildren<StateManager>();
            if (stateManager == null) continue;

            // 비주얼 반영
            Debug.Log(stageName + " 시각화, 현재 상태 : " + status);
            stateManager.ApplyStageVisual(status);

            // 모범답안 초기화 처리
            var hintManager = stageMarker.GetComponentInChildren<HintManager>();
            if (hintManager == null) continue;

            if (status == StageStatus.Cleared)
            {
                ApplyAnswerDraggables(stateManager, hintManager);
                ApplyAnswerCircles(stateManager, hintManager);
            }
            else
            {
                // 배치 초기화
                stateManager.ResetDraggable();
                stateManager.ResetManaCircle();
            }
        }
    }
    
    // 드래그 가능한 속성 회로들을 HintManager의 정답 위치에 맞춰 재배치
    private void ApplyAnswerDraggables(StateManager stateManager, HintManager hintManager)
    {
        // 1. 현재 스테이지의 드래그 가능한 속성 회로들을 속성 타입별로 그룹화
        var draggablesByType = stateManager.Draggables
            .GroupBy(d => d.attributeType)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 2. HintManager 내부의 정답 회로들(AnswerCircuit)도 속성 타입별로 그룹화
        var answerByType = hintManager.AnswerCircuits
            .GroupBy(a => a.AnswerType)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 3. 타입별로 순회하며 일치하는 속성 회로 위치를 정답 포지션으로 배치
        foreach (var kvp in answerByType)
        {
            var type = kvp.Key;
            var answerList = kvp.Value;

            // None 타입은 무효이므로 스킵
            if (type == ManaProperties.ManaType.None) continue;

            // 해당 타입의 드래그 회로가 존재하지 않을 경우 스킵 (정답과 무관)
            if (!draggablesByType.ContainsKey(type))
            {
                string stageName = stateManager.GetComponentInParent<StageRootMarker>()?.gameObject.name ?? "(알 수 없음)";
                Debug.LogWarning($"[HintSync] {stageName}에서 속성 {type}에 대한 드래그 가능한 회로가 존재하지 않습니다.");
                continue;
            }

            var dragList = draggablesByType[type];

            // 드래그 회로 수 ≠ 정답 회로 수 → 데이터가 불일치함을 경고
            if (dragList.Count != answerList.Count)
            {
                string stageName = stateManager.GetComponentInParent<StageRootMarker>()?.gameObject.name ?? "(알 수 없음)";
                Debug.LogWarning($"[HintSync] {stageName}에서 속성 {type} 회로 개수 불일치: 정답 {answerList.Count}개 / 현재 {dragList.Count}개");
                continue;
            }

            // 개수 일치 시 순서대로 정답 위치에 배치 (정렬은 없음)
            for (int i = 0; i < dragList.Count; i++)
            {
                dragList[i].transform.position = answerList[i].AnswerPos;
            }
        }
    }



    // 마나 서클의 활성 개수를 HintManager의 정답 개수에 맞춰 재설정 (동일 타입 서클 모두 적용)
    private void ApplyAnswerCircles(StateManager stateManager, HintManager hintManager)
    {
        // 1. HintManager에 정의된 정답 마나 서클 목록 순회
        foreach (var answer in hintManager.ManaCircleAnswsers)
        {
            // None 타입은 무효이므로 스킵
            if (answer.type == ManaProperties.ManaType.None) continue;

        // 2. 해당 타입의 마나 서클을 전부 가져옴 (복수 가능)
            var targets = stateManager.ManaCircles
                .Where(c => c.manaType == answer.type)
                .ToList();
            
            if (targets.Count == 0)
            {
                string stageName = stateManager.GetComponentInParent<StageRootMarker>()?.gameObject.name ?? "(알 수 없음)";
    
                var allTypes = stateManager.ManaCircles
                    .Select(c => c.manaType.ToString())
                    .Distinct()
                    .ToList();

                Debug.LogWarning($"[HintSync] {stageName}에서 타입 {answer.type}에 해당하는 마나 서클이 존재하지 않습니다.");
                Debug.LogWarning($"→ 해당 스테이지의 모든 마나타입: {string.Join(", ", allTypes)}");
            }

            // 3. 해당 타입의 모든 마나 서클에 동일한 정답 개수 적용
            foreach (var circle in targets)
            {
                circle.ResetManaCircle(answer.answerCount);
            }
        }
    }





    void GameClear()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        _uiManager.PlayingUI.ActivateResetBtns(!isGameOver);

        Debug.Log("게임 클리어! 모든 출력 회로가 충족됨.");
        Time.timeScale = 0f;
        // UIManager.Instance.ShowRestartPanel();
        string stageName = GameManager.Instance.CurrentPlayingStage.name;
        StageDataManager.Instance.SetStageCleared(stageName);
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
        IsGameOver = false;
        InitializeGame();
        ApplyStageVisualStates();
    }    
    
    // 내부에서만 호출하도록
    public void RegisterReset()
    {
        resetCount++;
        OnReset?.Invoke();
        Debug.Log("리셋 카운트: " + resetCount);
    }
}
