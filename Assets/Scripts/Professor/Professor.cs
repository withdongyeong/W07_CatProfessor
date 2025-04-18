using System;
using UnityEngine;
using System.Collections.Generic;

public class Professor : MonoBehaviour
{
    public static Professor Instance { get; private set; }

    public enum AnimationType { Idle, Talk, Victory, Surprise, Sleep}

    [Header("애니메이션 프레임")]
    public List<Sprite> idleFrames;
    public List<Sprite> talkFrames;
    public List<Sprite> victoryFrames;

    [Header("프레임 속도")]
    public float idleFrameRate = 0.1f;
    public float talkFrameRate = 0.07f;
    public float victoryFrameRate = 0.15f;

    public bool loop = true;

    private SpriteRenderer spriteRenderer;
    private List<Sprite> currentFrames;
    private int currentFrame;
    private float timer;
    private bool isPlaying = true;
    private AnimationType currentAnimation = AnimationType.Idle;
    private float currentFrameRate;
    
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
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentFrames = idleFrames;
        currentFrameRate = idleFrameRate;
        SetAnimation(AnimationType.Idle);

        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("SpriteRenderer.sprite가 설정되지 않았습니다.");
        }
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
    }

    private void LateUpdate()
    {
        // 카메라 줌 & 패닝에 맞춰 위치와 스케일 조정
        if (mainCamera == null || !mainCamera.orthographic) return;

        // 1) 화면(viewport) 좌표 → 월드 좌표
        Vector3 vp = new Vector3(viewportPos.x, viewportPos.y, zOffset);
        Vector3 worldPos = mainCamera.ViewportToWorldPoint(vp);
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        // 2) 카메라 사이즈 비율에 따라 스케일 조정
        float scaleFactor = baseCamSize / mainCamera.orthographicSize; //줌인 되면 scaleFactor 커짐
        transform.localScale = baseProfessorScale / scaleFactor; // 화면 스케일이 커지니까 캐릭터는 작아지게
    }

    public void SetAnimation(AnimationType animationType)
    {
        // if (currentAnimation == animationType) return;
        
        currentAnimation = animationType;
        isPlaying = true;
        currentFrame = 0;
        timer = 0f;

        switch (animationType)
        {
            case AnimationType.Idle:
                // currentFrames = idleFrames;
                // currentFrameRate = idleFrameRate;
                animator.Play("Idle");
                break;
            case AnimationType.Talk:
                // currentFrames = talkFrames;
                // currentFrameRate = talkFrameRate;
                animator.Play("Talk");
                break;
            case AnimationType.Victory:
                // currentFrames = victoryFrames;
                // currentFrameRate = victoryFrameRate;
                animator.Play("Happy");
                break;
            case AnimationType.Surprise:
                animator.Play("Surprised");
                break;
            case AnimationType.Sleep:
                animator.Play("Sleep");
                break;
        }

        if (currentFrames == null || currentFrames.Count == 0)
        {
            Debug.LogError($"{animationType} 애니메이션 프레임이 없습니다.");
            return;
        }

        spriteRenderer.sprite = currentFrames[0];
    }

    public void Play()
    {
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }
    public void Say(string message)
    {
        TalkBox.Instance.Talk(message);
    }

    public void SayRandom(ScriptManager.ScriptCategory category)
    {
        string message = ScriptManager.Instance.Get(category);
        TalkBox.Instance.Talk(message);
    }

    private void OnMouseDown()
    {
        SetAnimation(AnimationType.Surprise);
    }
}
