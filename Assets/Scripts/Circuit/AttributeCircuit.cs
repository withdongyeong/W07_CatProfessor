using UnityEngine;
using System.Collections.Generic;

public class AttributeCircuit : MonoBehaviour
{
    public ManaProperties.ManaType attributeType;
    public bool isDragable = false;

    public List<ShootDirectionConfig> directionConfigs;
    public List<ShootPatternConfig> patternConfigs;

    private int currentDirectionIndex = 0;
    private int currentPatternIndex = 0;

    public GameObject manaPrefab;
    private GameObject ringEffectInstance;
    private GameObject shootEffectInstance;
    private SpriteRenderer effectSpriteRenderer;
    private SpriteRenderer shootEffectRenderer;
    private Animator shootEffectAnimator;
    private SpriteRenderer attributeSpriteRenderer;

    private float lastFireTime = 0f;
    private float shootEffectDuration = 0.55f;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        if (isDragable && GetComponent<DraggableObject>() == null)
        {
            gameObject.AddComponent<DraggableObject>().isDragable = true;
        }

        ringEffectInstance = transform.Find("RingEffect")?.gameObject;
        shootEffectInstance = transform.Find("ShootEffect")?.gameObject;

        if (ringEffectInstance != null)
        {
            effectSpriteRenderer = ringEffectInstance.GetComponentInChildren<SpriteRenderer>();
            ringEffectInstance.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"AttributeCircuit({gameObject.name}): RingEffect를 찾을 수 없음!");
        }

        if (shootEffectInstance != null)
        {
            shootEffectInstance.SetActive(false);
            shootEffectRenderer = shootEffectInstance.GetComponentInChildren<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning($"AttributeCircuit({gameObject.name}): ShootEffect를 찾을 수 없음!");
        }

        shootEffectAnimator = shootEffectInstance?.GetComponent<Animator>();

        attributeSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateAttributeColor(attributeType);
        UpdateRingEffect();
    }

    void Update()
    {
        if (shootEffectInstance != null && shootEffectInstance.activeSelf && Time.time - lastFireTime > shootEffectDuration)
        {
            shootEffectInstance.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Mana incomingMana = other.GetComponent<Mana>();
        if (incomingMana == null || !incomingMana.CanBeDetected()) return;
        int newOutput = incomingMana.remainingOutput - 1;

        if (incomingMana.remainingOutput <= 0)
        {
            ManaPool.Instance.ReturnMana(incomingMana);
            return;
        }
        
        ManaPool.Instance.ReturnMana(incomingMana);
        ConvertAndShoot(incomingMana, newOutput);
    }

    void ConvertAndShoot(Mana incomingMana, int newOutput)
    {
        if (directionConfigs.Count == 0 || patternConfigs.Count == 0)
        {
            Debug.LogWarning("Direction or pattern configuration missing.");
            return;
        }

        ShootDirectionConfig directionConfig = directionConfigs[currentDirectionIndex];
        ShootPatternConfig patternConfig = patternConfigs[currentPatternIndex];

        foreach (Vector2 direction in directionConfig.GetShootDirections())
        {
            if (newOutput <= 0) continue;

            Mana newMana = ManaPool.Instance.GetMana(transform.position, direction, attributeType, newOutput);
            if (newMana != null)
            {
                newMana.SetColorByType();
                newMana.currentPattern = patternConfig;
                patternConfig.ApplyPattern(newMana);

                ActivateShootEffect(newMana.currentType);
            }
        }
    }

    public void CycleDirection()
    {
        currentDirectionIndex = (currentDirectionIndex + 1) % directionConfigs.Count;
    }

    public void CyclePattern()
    {
        currentPatternIndex = (currentPatternIndex + 1) % patternConfigs.Count;
    }

    public string GetCurrentDirectionType()
    {
        return directionConfigs[currentDirectionIndex].name;
    }

    public string GetCurrentShootPattern()
    {
        return patternConfigs[currentPatternIndex].name;
    }

    public void SetDragable(bool canDrag)
    {
        isDragable = canDrag;
        UpdateRingEffect();
    }

    private void UpdateRingEffect()
    {
        if (ringEffectInstance == null) return;

        ringEffectInstance.SetActive(isDragable);
        
        if (isDragable)
        {
            UpdateRingEffectColor(attributeType);
        }
    }

    private void UpdateRingEffectColor(ManaProperties.ManaType manaType)
    {
        if (effectSpriteRenderer == null) return;
        effectSpriteRenderer.color = ManaProperties.GetColor(manaType);
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

    private void UpdateAttributeColor(ManaProperties.ManaType manaType)
    {
        if (attributeSpriteRenderer == null) return;
        attributeSpriteRenderer.color = ManaProperties.GetColor(manaType);
    }

    private void OnValidate()
    {
        if (attributeSpriteRenderer == null)
        {
            attributeSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        UpdateAttributeColor(attributeType);
        UpdateRingEffectColor(attributeType);
        UpdateShootEffectColor(attributeType);

    }
}
