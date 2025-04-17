using UnityEngine;

public class InputCircuit : MonoBehaviour
{
    public GameObject manaPrefab;
    public float fireRate = 0.2f;
    public float shootEffectDuration = 0.3f;

    public ManaProperties.ManaType attributeType = ManaProperties.ManaType.None;

    private bool isFiring = false;
    private float nextFireTime = 0f;
    private float lastFireTime = 0f;
    private GameObject shootEffectInstance;
    private SpriteRenderer shootEffectRenderer;
    private SpriteRenderer attributeSpriteRenderer; 

    public ShootPatternConfig defaultShootPattern;
    public ShootDirectionConfig defaultShootDirection;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        shootEffectInstance = transform.Find("ShootEffect")?.gameObject;

        if (shootEffectInstance != null)
        {
            shootEffectInstance.SetActive(false);
            shootEffectRenderer = shootEffectInstance.GetComponentInChildren<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning($"InputCircuit({gameObject.name}): ShootEffect를 찾을 수 없음!");
        }

        attributeSpriteRenderer = GetComponentInChildren<SpriteRenderer>(); 
        UpdateAttributeColor(attributeType);

        if (defaultShootPattern == null)
        {
            defaultShootPattern = ScriptableObject.CreateInstance<ShootPattern1>();
        }

        if (defaultShootDirection == null)
        {
            defaultShootDirection = ScriptableObject.CreateInstance<DirectionPattern4Way>();
        }
    }

    void Update()
    {
        if (isFiring && Time.time >= nextFireTime) 
        {
            ShootMana();
            nextFireTime = Time.time + fireRate;
        }

        if (!isFiring && shootEffectInstance != null && shootEffectInstance.activeSelf && Time.time - lastFireTime > shootEffectDuration)
        {
            shootEffectInstance.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.gameState.GamePlaying) return;
        isFiring = true;
        if (Time.time >= nextFireTime) 
        {
            ShootMana();
            nextFireTime = Time.time + fireRate;
        }
        ActivateShootEffect(attributeType);
    }

    void OnMouseUp()
    {
        isFiring = false;
    }

    void OnMouseEnter()
    {
        if (Input.GetMouseButton(0)) 
        {
            isFiring = true;
            if (Time.time >= nextFireTime) 
            {
                ShootMana();
                nextFireTime = Time.time + fireRate;
            }
            ActivateShootEffect(attributeType);
        }
    }

    void OnMouseExit()
    {
        isFiring = false;
    }

    void ShootMana()
    {
        var directions = defaultShootDirection.GetShootDirections();
        foreach (var direction in directions)
        {
            Mana mana = ManaPool.Instance.GetMana(transform.position, direction, attributeType);
            if (mana != null)
            {
                defaultShootPattern.ApplyPattern(mana);
            }
        }
        lastFireTime = Time.time;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Mana incomingMana = other.GetComponent<Mana>();
        if (incomingMana == null || !incomingMana.CanBeDetected()) return;

        ManaPool.Instance.ReturnMana(incomingMana);
    }

    private void ActivateShootEffect(ManaProperties.ManaType manaType)
    {
        if (shootEffectInstance != null)
        {
            shootEffectInstance.SetActive(true);
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

        if (attributeSpriteRenderer != null)
        {
            UpdateAttributeColor(attributeType);
        }

        UpdateShootEffectColor(attributeType);
    }

}
