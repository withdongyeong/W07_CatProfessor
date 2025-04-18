using UnityEngine;
using System.Collections.Generic;

public class NeutralCircuit : MonoBehaviour, IColorable
{
    public bool isDragable = false;

    public List<ShootDirectionConfig> directionConfigs;
    public List<ShootPatternConfig> patternConfigs;

    private int currentDirectionIndex = 0;
    private int currentPatternIndex = 0;

    public GameObject manaPrefab;
    private GameObject ringEffectInstance;
    private GameObject shootEffectInstance;
    private SpriteRenderer shootEffectRenderer;
    private Animator shootEffectAnimator;

    private float lastFireTime = 0f;
    public float shootEffectDuration = 0.55f;

    private bool isRotating = false;
    private float rotateSpeed = 90f;

    void Start()
    {
        InitializeComponents();

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = ManaProperties.GetColor(ManaProperties.ManaType.Neutral);
        }

        if (isDragable) {
            if (ringEffectInstance != null)
            {
                SpriteRenderer ringRenderer = ringEffectInstance.GetComponentInChildren<SpriteRenderer>();
                if (ringRenderer != null)
                {
                    ringRenderer.color = ManaProperties.GetColor(ManaProperties.ManaType.Neutral);
                }
            }
        }
    }
    public void SetColor(Color color)
    {
        var mainRenderer = GetComponentInChildren<SpriteRenderer>();
        if (mainRenderer != null)
            mainRenderer.color = color;

        if (ringEffectInstance != null)
        {
            var ringRenderer = ringEffectInstance.GetComponentInChildren<SpriteRenderer>();
            if (ringRenderer != null)
                ringRenderer.color = color;
        }

        if (shootEffectRenderer != null)
            shootEffectRenderer.color = color;
    }

    public void SetDefaultColor()
    {
        SetColor(ManaProperties.GetColor(ManaProperties.ManaType.Neutral));
    }
    void InitializeComponents()
    {
        if (isDragable && GetComponent<DraggableObject>() == null)
        {
            var draggable = gameObject.AddComponent<DraggableObject>();
            draggable.Init(ManaProperties.ManaType.Neutral);
        }

        ringEffectInstance = transform.Find("RingEffect")?.gameObject;
        shootEffectInstance = transform.Find("ShootEffect")?.gameObject;

        if (shootEffectInstance != null)
        {
            shootEffectInstance.SetActive(false);
            shootEffectRenderer = shootEffectInstance.GetComponentInChildren<SpriteRenderer>();
            if (shootEffectRenderer != null) {
                shootEffectRenderer.color = ManaProperties.GetColor(ManaProperties.ManaType.Neutral);
            }
        }

        shootEffectAnimator = shootEffectInstance?.GetComponent<Animator>();
    }

    void Update()
    {
        if (shootEffectInstance != null && shootEffectInstance.activeSelf && Time.time - lastFireTime > shootEffectDuration)
        {
            shootEffectInstance.SetActive(false);
            isRotating = false;
        }

        if (isRotating)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
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
        ShootNeutralMana(incomingMana, newOutput);
    }

    void ShootNeutralMana(Mana incomingMana, int newOutput)
    {
        if (directionConfigs.Count == 0 || patternConfigs.Count == 0) return;

        ShootDirectionConfig directionConfig = directionConfigs[currentDirectionIndex];
        ShootPatternConfig patternConfig = patternConfigs[currentPatternIndex];

        isRotating = true;

        foreach (Vector2 direction in directionConfig.GetShootDirections())
        {
            if (newOutput <= 0) continue;

            Mana newMana = ManaPool.Instance.GetMana(transform.position, direction, incomingMana.currentType, newOutput);
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
        }
    }

    private void OnValidate()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = ManaProperties.GetColor(ManaProperties.ManaType.Neutral);
        }

        if (ringEffectInstance != null)
        {
            SpriteRenderer ringRenderer = ringEffectInstance.GetComponentInChildren<SpriteRenderer>();
            if (ringRenderer != null)
            {
                ringRenderer.color = ManaProperties.GetColor(ManaProperties.ManaType.Neutral);
            }
        }

        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
        #endif
    }

}
