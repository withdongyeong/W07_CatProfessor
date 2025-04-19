using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Random = Unity.Mathematics.Random;

public class Professor : MonoBehaviour
{
    public static Professor Instance { get; private set; }

    public enum AnimationType { Idle, Talk, Victory, Surprise, InSleep, Sleep}

    // [Header("애니메이션 프레임")]
    // public List<Sprite> idleFrames;
    // public List<Sprite> talkFrames;
    // public List<Sprite> victoryFrames;

    // [Header("프레임 속도")]
    // public float idleFrameRate = 0.1f;
    // public float talkFrameRate = 0.07f;
    // public float victoryFrameRate = 0.15f;

    // public bool loop = true;

    private SpriteRenderer spriteRenderer;
    // private List<Sprite> currentFrames;
    // private int currentFrame;
    // private float timer;
    // private bool isPlaying = true;
    // private AnimationType currentAnimation = AnimationType.Idle;
    // private float currentFrameRate;
    
    [Header("스테이지 설정")]
    private StateManager curStateManager;
    private HintManager curhintManager;
    
    [Header("카메라 설정")]
    [Tooltip("Viewport 좌표")]
    public Vector2 viewportPos = new Vector2(0.9f, 0.1f);
    [Tooltip("Z offset")]
    public float zOffset = 10f;
    private Camera mainCamera;
    private float baseCamSize;
    private Vector3 baseProfessorScale;
    
    [Header("애니메이션 설정")]
    private Animator animator;
    
    [Header("힌트 설정")]
    [Tooltip("깜빡임 총 시간 (초)")]
    public float hintBlinkDuration = 3f;
    [Tooltip("깜빡이는 속도 (사이클/초)")]
    public float blinkFrequency    = 2f;
    [Tooltip("테스트용 : 직접힌트 0 간접힌트 1")]
    public bool hintType = false;
    [Tooltip("간접힌트 : 진행도 체크 주기 (초)")]
    [SerializeField] private float progressCheckInterval = 30f; // 30초마다 대사 출력
    private Coroutine progressCoroutine; // 진행도 체크 코루틴
    // 현재 실행 중인 힌트 코루틴
    private Coroutine hintCoroutine;
    
    //마지막 입력 시간
    private float lastInputTime;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer가 없습니다.");
            return;
        }
        
        //카메라 참조
        mainCamera = Camera.main;
        if (mainCamera == null || !mainCamera.orthographic)
        {
            Debug.LogError("Main camera not found or not orthographic");
        }
        else
        {
            baseCamSize = mainCamera.orthographicSize;
            baseProfessorScale = transform.localScale;
        }
        //시네머신 업데이트 이벤트 등록
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        // currentFrames = idleFrames;
        // currentFrameRate = idleFrameRate;
        SetAnimation(AnimationType.Idle);

        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("SpriteRenderer.sprite가 설정되지 않았습니다.");
        }
        if (progressCoroutine == null && hintType == true)
            progressCoroutine = StartCoroutine(CheckProgressRoutine());
    }

    void Update()
    {
        // if (!isPlaying || currentFrames == null || currentFrames.Count == 0)
        // {
        //     return;
        // }
        //
        // timer += Time.unscaledDeltaTime;
        //
        // if (timer >= currentFrameRate)
        // {
        //     timer = 0f;
        //     currentFrame++;
        //
        //     if (currentFrame >= currentFrames.Count)
        //     {
        //         if (loop)
        //             currentFrame = 0;
        //         else
        //         {
        //             isPlaying = false;
        //             return;
        //         }
        //     }
        //
        //     spriteRenderer.sprite = currentFrames[currentFrame];
        // }
        if (Input.GetMouseButtonDown(0))
        {
            lastInputTime = Time.time;
        }
        // 10초간 입력이 없으면 애니메이션 변경
        if (Time.time - lastInputTime > 10f && GameManager.Instance.resetCount >= 10)
        {
            IsCurrentAnimation(AnimationType.Idle);
            {
                SetAnimation(AnimationType.InSleep);
            }
        }
    }

    // Cinemachine이 카메라를 다 이동/블렌딩한 직후 호출됩니다.
    void OnCameraUpdated(CinemachineBrain brain)
    {
        if (brain.OutputCamera != mainCamera || !mainCamera.orthographic)
            return;

        // 1) 화면(viewport) → 월드 좌표 변환
        Vector3 vp = new Vector3(viewportPos.x, viewportPos.y, zOffset);
        Vector3 worldPos = mainCamera.ViewportToWorldPoint(vp);
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        // 2) 카메라 확대율에 맞춘 스케일 조정
        float scaleFactor = baseCamSize / mainCamera.orthographicSize;
        transform.localScale = baseProfessorScale / scaleFactor;
    }

    public void SetAnimation(AnimationType animationType)
    {
        // if (currentAnimation == animationType) return;
        
        // currentAnimation = animationType;
        // isPlaying = true;
        // currentFrame = 0;
        // timer = 0f;

        switch (animationType)
        {
            case AnimationType.Idle:
                // currentFrames = idleFrames;
                // currentFrameRate = idleFrameRate;
                animator.SetInteger("State", 0);
                break;
            case AnimationType.Talk:
                // currentFrames = talkFrames;
                // currentFrameRate = talkFrameRate;
                animator.SetInteger("State", 2);
                break;
            case AnimationType.Victory:
                // currentFrames = victoryFrames;
                // currentFrameRate = victoryFrameRate;
                animator.Play("Happy");
                break;
            case AnimationType.Surprise:
                animator.Play("Surprised");
                break;
            case AnimationType.InSleep:
                animator.SetInteger("State", 1);
                break;
            case AnimationType.Sleep:
                animator.Play("Sleep");
                break;
        }

        // if (currentFrames == null || currentFrames.Count == 0)
        // {
        //     Debug.LogError($"{animationType} 애니메이션 프레임이 없습니다.");
        //     return;
        // }
        //
        // spriteRenderer.sprite = currentFrames[0];
    }

    // public void Play()
    // {
        // isPlaying = true;
    // }

    // public void Stop()
    // {
        // isPlaying = false;
    // }
    public void Say(string message)
    {
        TalkBox.Instance.Talk(message);
    }

    public void SayRandom(ScriptManager.ScriptCategory category)
    {
        string message = ScriptManager.Instance.Get(category);
        TalkBox.Instance.Talk(message);
    }

    
    // 교수님 상호작용
    private void OnMouseDown()
    {
        if (hintType == false) // 직접힌트
        {
            
            if (IsCurrentAnimation(AnimationType.Sleep))
            {
                SetAnimation(AnimationType.Idle);
                lastInputTime = Time.time;
                Debug.Log("힌트 호출");
            }
            Debug.Log("교수님 클릭");
            //스테이지 내부 아니면 취소
            if (curStateManager == null || curhintManager == null) return;

            var draggables = curStateManager.Draggables;
            var answers    = curhintManager.AnswerCircuits;
            const float threshold = 0.1f;

            // 1) 놓여 있지 않은(= 힌트가 필요한) AnswerCircuit만 뽑아서 리스트로 만듭니다.
            var missing = answers
                .Where(ac => !draggables
                    .Any(d => Vector3.Distance(d.transform.position, ac.AnswerPos) < threshold))
                .ToList();

            // 2) 먼저 모든 힌트 오브젝트를 끕니다.
            foreach (var ac in answers)
            {
                var sr = ac.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.enabled = false;
            }

            // 3) missing이 비어있지 않다면, 그 중 하나만 골라 켭니다.
            if (missing.Count > 0)
            {
                var pick = missing[0];

                var sr = pick.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                    sr.enabled = true;

                // 이미 표시 중이면 멈추고 초기화
                if (hintCoroutine != null)
                    StopCoroutine(hintCoroutine);
            
                hintCoroutine = StartCoroutine(BlinkHint(sr));
            }
        }
        else //간접 힌트
        {
            if (curStateManager == null || curhintManager == null) return;

            var draggables = curStateManager.Draggables;
            var answers    = curhintManager.AnswerCircuits;
            
            const float threshold = 0.1f;

            int correctCount = 0;
            int totalAnswers = answers.Count;

            foreach (var answer in answers)
            {
                bool matched = draggables
                    .Any(d => Vector3.Distance(d.transform.position, answer.AnswerPos) < threshold);

                if (matched)
                    correctCount++;
            }

            // 퍼센트 (예: 60%)
            float percent = (totalAnswers > 0) ? (correctCount / (float)totalAnswers) * 100f : 0f;

            // 예시 출력
            Debug.Log($"진행도: {correctCount}/{totalAnswers} ({percent:0.0}%)");
        }
    }

    private IEnumerator BlinkHint(SpriteRenderer sr)
    {
        float elapsed  = 0f;
        Color baseCol  = sr.color;             // 원래 색(컬러+알파)
        while (elapsed < hintBlinkDuration)
        {
            // 0~1 사이를 왔다갔다
            float a = Mathf.PingPong(elapsed * blinkFrequency, 1f);
            sr.color = new Color(baseCol.r, baseCol.g, baseCol.b, a);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 끝나면 숨기고, 원래 알파 복원
        sr.enabled   = false;
        sr.color     = baseCol;
        hintCoroutine = null;
    }
    
    //간접힌트
    private IEnumerator CheckProgressRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(progressCheckInterval);

            //스테이지 내부 아니면 취소
            if (curStateManager == null || curhintManager == null) continue;

            var draggables = curStateManager.Draggables;
            var answers = curhintManager.AnswerCircuits;
            const float threshold = 0.1f;

            int correctCount = answers.Count(ac =>
                draggables.Any(d => Vector3.Distance(d.transform.position, ac.AnswerPos) < threshold));

            float percent = (answers.Count > 0) ? (correctCount / (float)answers.Count) * 100f : 0f;

            // 간접 대사 출력
            ScriptManager.ScriptCategory category = GetProgressCategory(percent);
            string message = ScriptManager.Instance.Get(category);
            TalkBox.Instance.Talk(message);
        }
    }

    private ScriptManager.ScriptCategory GetProgressCategory(float percent)
    {
        if (percent < 20f)
            return ScriptManager.ScriptCategory.ProgressStage0;
        else if (percent < 40f)
            return ScriptManager.ScriptCategory.ProgressStage1; 
        else if (percent < 60f)
            return ScriptManager.ScriptCategory.ProgressStage2;
        else if (percent < 80f)
            return ScriptManager.ScriptCategory.ProgressStage3;
        else
            return ScriptManager.ScriptCategory.ProgressStage4;
    }

    
    // 현재 스테이지 설정
    public void SetCurrentStage(StateManager stateManager, HintManager hintManager)
    {
        curStateManager = stateManager;
        curhintManager = hintManager;
    }
    
    // 스테이지 초기화
    public void ResetStage()
    {
        curStateManager = null;
        curhintManager = null;
    }
    
    //현재 애니메이션 체크
    public bool IsCurrentAnimation(AnimationType animationType)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(animationType.ToString()))
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if(animTime == 0)
            {
                // 플레이 중이 아님
            }
            if(animTime > 0 && animTime < 1.0f)
            {
                // 애니메이션 플레이 중
            }
            else if(animTime >= 1.0f)
            {
                // 애니메이션 종료
            }

            return true;
        }

        return false;
    }

    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }
}
