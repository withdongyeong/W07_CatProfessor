using UnityEngine;
using System.Collections.Generic;

public class Professor : MonoBehaviour
{
    public static Professor Instance { get; private set; }

    public enum AnimationType { Idle, Talk, Victory }

    public List<Sprite> idleFrames;
    public List<Sprite> talkFrames;
    public List<Sprite> victoryFrames;

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

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentFrames = idleFrames;
        currentFrameRate = idleFrameRate;
        SetAnimation(AnimationType.Idle);

        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("SpriteRenderer.sprite가 설정되지 않았습니다.");
        }
    }

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

        SetAnimation(AnimationType.Idle);
    }

    void Update()
    {
        if (!isPlaying || currentFrames == null || currentFrames.Count == 0)
        {
            return;
        }

        timer += Time.unscaledDeltaTime;

        if (timer >= currentFrameRate)
        {
            timer = 0f;
            currentFrame++;

            if (currentFrame >= currentFrames.Count)
            {
                if (loop)
                    currentFrame = 0;
                else
                {
                    isPlaying = false;
                    return;
                }
            }

            spriteRenderer.sprite = currentFrames[currentFrame];
        }
    }

    public void SetAnimation(AnimationType animationType)
    {
        if (currentAnimation == animationType) return;

        currentAnimation = animationType;
        isPlaying = true;
        currentFrame = 0;
        timer = 0f;

        switch (animationType)
        {
            case AnimationType.Idle:
                currentFrames = idleFrames;
                currentFrameRate = idleFrameRate;
                break;
            case AnimationType.Talk:
                currentFrames = talkFrames;
                currentFrameRate = talkFrameRate;
                break;
            case AnimationType.Victory:
                currentFrames = victoryFrames;
                currentFrameRate = victoryFrameRate;
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

}
