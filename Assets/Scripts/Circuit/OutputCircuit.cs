using UnityEngine;

public class OutputCircuit : MonoBehaviour
{
    public ManaProperties.ManaType targetType;
    public int targetCount = 5;
    public float resetTime = 3f;

    private int currentCount = 0;
    private bool isComplete = false;
    private float lastManaTime;

    private SpriteRenderer spriteRenderer;
    private GameObject ringEffectInstance;
    private SpriteRenderer effectSpriteRenderer;

    private GameObject shootEffectInstance;
    private SpriteRenderer shootEffectRenderer;
    private Animator shootEffectAnimator;
    private float shootEffectDuration = 0.55f;
    private float lastFireTime = 0f;

    private bool isRotating = false;
    private float rotateSpeed = 30f;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) return;

        lastManaTime = Time.time;
        SetColorByType();

        ringEffectInstance = transform.Find("RingEffect")?.gameObject;
        if (ringEffectInstance != null)
        {
            effectSpriteRenderer = ringEffectInstance.GetComponentInChildren<SpriteRenderer>();
            ringEffectInstance.SetActive(false);
        }

        shootEffectInstance = transform.Find("ShootEffect")?.gameObject;
        if (shootEffectInstance != null)
        {
            shootEffectInstance.SetActive(false);
            shootEffectRenderer = shootEffectInstance.GetComponentInChildren<SpriteRenderer>();
            shootEffectAnimator = shootEffectInstance.GetComponent<Animator>();
        }

        UpdateRingEffect();
    }

    void Update()
    {
        if (ShouldReset())
        {
            ResetCounter();
        }

        if (shootEffectInstance != null && shootEffectInstance.activeSelf && Time.time - lastFireTime > shootEffectDuration)
        {
            shootEffectInstance.SetActive(false);
        }

        if (isRotating)
        {
            spriteRenderer.transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Mana incomingMana = other.GetComponent<Mana>();
        if (incomingMana == null) return;

        if (incomingMana.currentType == targetType)
        {
            currentCount++;
            lastManaTime = Time.time;
            ActivateShootEffect(targetType);

            isRotating = true;

            if (currentCount >= targetCount)
            {
                isComplete = true;
                UpdateRingEffect();
            }
        }
        ManaPool.Instance.ReturnMana(incomingMana);
    }

    bool ShouldReset()
    {
        return currentCount > 0 && Time.time - lastManaTime > resetTime;
    }

    void ResetCounter()
    {
        currentCount = 0;
        isComplete = false;
        isRotating = false;
        UpdateRingEffect();
    }

    public bool IsComplete()
    {
        return isComplete;
    }

    void SetColorByType()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = ManaProperties.GetColor(targetType);
    }

    private void UpdateRingEffect()
    {
        if (ringEffectInstance == null) return;

        ringEffectInstance.SetActive(isComplete);

        if (isComplete)
        {
            UpdateRingEffectColor();
        }
    }

    private void UpdateRingEffectColor()
    {
        if (effectSpriteRenderer == null) return;
        effectSpriteRenderer.color = ManaProperties.GetColor(targetType);
    }

    private void ActivateShootEffect(ManaProperties.ManaType manaType)
    {
        if (shootEffectInstance != null)
        {
            if (!shootEffectInstance.activeSelf)
            {
                shootEffectInstance.SetActive(true);
            }

            if (shootEffectAnimator != null)
            {
                shootEffectAnimator.Play("ShootEffect", 0, shootEffectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

            lastFireTime = Time.time;
            UpdateShootEffectColor(manaType);
        }
    }

    private void UpdateShootEffectColor(ManaProperties.ManaType manaType)
    {
        if (shootEffectRenderer == null) return;
        shootEffectRenderer.color = ManaProperties.GetColor(manaType);
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        SetColorByType();
        UpdateRingEffectColor();

        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
        #endif
    }
}
