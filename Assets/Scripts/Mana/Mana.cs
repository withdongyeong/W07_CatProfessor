using UnityEngine;

public class Mana : MonoBehaviour
{
    public ManaProperties.ManaType currentType = ManaProperties.ManaType.None;
    public Vector2 direction = Vector2.right;
    public float speed = 5f;
    public float maxLifetime = 2f;

    private TrailRenderer trailRenderer;
    private bool canBeDetected = false;
    public float detectDelay = 0.2f;
    public int defaultOutput = 5;
    public int remainingOutput = 5;
    private Vector3 initialScale;
    
    private ManaPool pool;

    public ShootPatternConfig currentPattern;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        
        initialScale = spriteRenderer.transform.localScale;

        SetColorByType();
        UpdateScaleByOutput();

        Invoke(nameof(EnableDetection), detectDelay);
    }

    void FixedUpdate()
    {
        if (currentPattern != null)
        {
            currentPattern.ApplyPattern(this);
        }

        transform.position += (Vector3)direction.normalized * speed * Time.deltaTime;

        if (remainingOutput <= 0) ReturnToPool();
    }

    public void EnableDetection() => canBeDetected = true;

    public bool CanBeDetected() => canBeDetected;

    public void SetColorByType()
    {
        if (spriteRenderer == null) return;
        Color manaColor = ManaProperties.GetColor(currentType);
        spriteRenderer.color = manaColor;

        if (trailRenderer != null)
        {
            trailRenderer.startColor = manaColor;
            trailRenderer.endColor = new Color(manaColor.r, manaColor.g, manaColor.b, 0);
        }
    }

    public int getDefaultOutput()
    {
        return defaultOutput;
    }

    public void SetOutput(int output)
    {
        remainingOutput = output;
        UpdateScaleByOutput();
    }

    private void UpdateScaleByOutput()
    {
        float minScaleFactor = 0.6f;
        float maxScaleFactor = 2f;
        float normalizedOutput = Mathf.Clamp01((float)remainingOutput / 5f);
        float scaleFactor = Mathf.Lerp(minScaleFactor, maxScaleFactor, normalizedOutput);

        spriteRenderer.transform.localScale = Vector3.one * scaleFactor;

        if (trailRenderer != null)
        {
            trailRenderer.startWidth = scaleFactor * 0.15f;
            trailRenderer.endWidth = 0f;
            trailRenderer.time = 0.5f;
        }
    }

    public void ReturnToPool()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        CancelInvoke(nameof(ReturnToPool));
        ResetMana();

        if (ManaPool.Instance != null)
        {
            ManaPool.Instance.ReturnMana(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetMana()
    {
        remainingOutput = 5;
        canBeDetected = false;
        spriteRenderer.transform.localScale = initialScale;
        currentPattern = null;
        gameObject.SetActive(false);

        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
    }

    public void SetPool(ManaPool manaPool)
    {
        pool = manaPool;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Mana otherMana = other.GetComponent<Mana>();
        if (otherMana == null) return;

        if (currentType == otherMana.currentType) return;

        ManaProperties.ManaType newType = ManaProperties.CombineManaTypes(currentType, otherMana.currentType);

        if (newType != ManaProperties.ManaType.None)
        {
            currentType = newType;
            otherMana.currentType = newType;

            SetColorByType();
            otherMana.SetColorByType();
        }
    }
}
