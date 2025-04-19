using UnityEngine;
using System.Collections.Generic;

public class ManaCircle : MonoBehaviour
{
    public bool isClickable = false;
    public float diameter = 5f;
    public float lineWidth = 0.1f;
    public float clickThreshold = 0.5f; // 클릭 판정 영역 확장
    public ManaProperties.ManaType manaType = ManaProperties.ManaType.None;
    public GameObject orbitingPrefab;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    private GameObject[] orbiters = new GameObject[5];
    private int activeOrbiters = 2;
    private float orbitSpeed = 30f;
    private bool isRotating = true;
    private bool isHighlighted = false;
    private float highlightPulseSpeed = 2f;
    private float highlightIntensity = 0.4f;
    private Color baseColor;

    private HintManager _hintManager;
    
    void Start()
    {
        SetupCircle();
        SetupCollider();
        SetupOrbiters(activeOrbiters);
        SetupStageClickable();
        
        _hintManager = GetComponentInParent<StageRootMarker>().GetComponentInChildren<HintManager>();
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        baseColor = ManaProperties.GetColor(manaType); // 기존 색 보존
    }

    // 회전 제어
    public void StopRotation()
    {
        isRotating = false;
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    // 색상 제어
    public void SetColor(Color c)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
        }

        foreach (Transform child in transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = c;
        }
    }

    public void SetDefaultColor()
    {
        SetColor(ManaProperties.GetColor(manaType));
    }
    void SetupStageClickable()
    {
        Transform clickableTransform = transform.Find("ClickableCircle");
        if (clickableTransform == null) return;

        GameObject clickableObject = clickableTransform.gameObject;

        if (isClickable)
        {
            clickableObject.SetActive(true);

            ClickableCircle circle = clickableObject.GetComponent<ClickableCircle>();
            if (circle == null)
            {
                circle = clickableObject.AddComponent<ClickableCircle>();
            }

            circle.SetRadius(diameter / 2f);
        }
        else
        {
            clickableObject.SetActive(false);
        }
    }

    void SetupCircle()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = 100;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.startColor = ManaProperties.GetColor(manaType);
        lineRenderer.endColor = ManaProperties.GetColor(manaType);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        DrawCircle();
    }

    void DrawCircle()
    {
        float radius = diameter / 2f;
        Vector3[] points = new Vector3[lineRenderer.positionCount];

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = i * 2 * Mathf.PI / (lineRenderer.positionCount - 1);
            points[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
        }

        lineRenderer.SetPositions(points);
    }

    void SetupCollider()
    {
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
        if (edgeCollider == null)
        {
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        }

        edgeCollider.isTrigger = true;
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (edgeCollider == null) return;

        float radius = diameter / 2f;
        Vector2[] colliderPoints = new Vector2[lineRenderer.positionCount + 1];

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = i * 2 * Mathf.PI / (lineRenderer.positionCount - 1);
            colliderPoints[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }

        colliderPoints[lineRenderer.positionCount] = colliderPoints[0];
        edgeCollider.points = colliderPoints;
    }

    public void SetupOrbiters(int activeOrbiterCount)
    {
        int beforeCount = transform.childCount;
        activeOrbiters = activeOrbiterCount;
        
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
        {
            // ClickableCircle이 있는 자식은 삭제하지 않음
            if (child.GetComponent<ClickableCircle>() == null)
            {
                children.Add(child);
            }
        }

        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }


        float radius = diameter / 2f;
        float angleStep = 360f / activeOrbiterCount;

        for (int i = 0; i < activeOrbiterCount; i++)
        {
            GameObject orbiter = Instantiate(orbitingPrefab, transform);
            orbiter.hideFlags = HideFlags.DontSave;

            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f) + transform.position;
            orbiter.transform.position = position;

            SpriteRenderer sr = orbiter.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = ManaProperties.GetColor(manaType);
            }
        }
    }

    void UpdateOrbitMotion()
    {
        float radius = diameter / 2f;
        float angleStep = 360f / activeOrbiters;
        float selfRotationSpeed = 90f;

        int i = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                if (!child.gameObject.activeSelf) continue;
                // ClickableCircle를 회전시키지 않음
                if (child.GetComponent<ClickableCircle>() != null) continue;
                
                float angle = (Time.time * orbitSpeed + i * angleStep) * Mathf.Deg2Rad;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f) + transform.position;
                child.position = newPos;

                child.Rotate(Vector3.forward * selfRotationSpeed * Time.deltaTime);

                i++;
            }
        }
    }
    
    private void Update()
    {
        // 1. Highlight 관련
        if (isHighlighted)
        {
            float pulse = Mathf.PingPong(Time.time * highlightPulseSpeed, 1f);
            float lerpFactor = Mathf.Lerp(0f, highlightIntensity, pulse);

            Color pulseColor = Color.Lerp(baseColor, Color.cyan, lerpFactor);
            Color glowColor = Color.Lerp(pulseColor, Color.white, lerpFactor * 0.7f);

            lineRenderer.startColor = glowColor;
            lineRenderer.endColor = glowColor;

            float baseWidth = 0.12f;
            float highlightMaxWidth = 0.35f;
            float widthPulse = Mathf.Lerp(baseWidth, highlightMaxWidth, pulse);
            lineRenderer.startWidth = widthPulse;
            lineRenderer.endWidth = widthPulse;
        }
        
        // 2. 궤도 회전 관련
        if (isRotating)
        {
            UpdateOrbitMotion();
        }

        // 3. 클릭 관련
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance.CurrentGameState != GameManager.gameState.GamePlaying)
            {
                return;
            }

            // 다른 스테이지 클릭 방지
            var isCurrentStageCircle = false;
            var circles = GameManager.Instance.CurrentPlayingStage.GetComponentInChildren<StateManager>().ManaCircles;
            foreach (var circle in circles)
            {
                
                if (this == circle)
                {
                    isCurrentStageCircle = true;
                    break;
                }
            }
            if (!isCurrentStageCircle) return;
            
            // 클릭 범위 체크 후 궤도 업데이트
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (IsPointOnCircleEdge(mousePosition))
            {
                if (IsManaPresent()) 
                {
                    Professor.Instance.SayRandom(ScriptManager.ScriptCategory.WaitMana);
                    return;
                }

                FindAndModifyAttributeCircuits();
            }
        }
    }


    bool IsManaPresent()
    {
        return FindObjectsByType<Mana>(FindObjectsSortMode.InstanceID).Length > 0;
    }
    
    private void ModifyCircuits(List<AttributeCircuit> circuits, bool isReset)
    {
        // Circle과 동일한 type일 때 CycleDirection 실행
        foreach (var circuit in circuits)
        {
            if (circuit.attributeType != manaType) continue;

            if (isReset)
                // 리셋 시
                circuit.CycleDirection(activeOrbiters + 1);
            else
                // 클릭 시 
                circuit.CycleDirection();
        }
    }
    
    private void FindAndModifyAttributeCircuits(bool isReset = false)
    {
        if (GameManager.Instance.CurrentGameState != GameManager.gameState.GamePlaying) return;

        var stateManager = GameManager.Instance.CurrentPlayingStage.GetComponentInChildren<StateManager>();
        
        ModifyCircuits(stateManager.AttributeCircuits, isReset);
        ModifyCircuits(stateManager.Draggables, isReset);
        
        if (!isReset)
        {
            // 클릭 시 activeOrbiters 증가
            activeOrbiters += 1;
        }
        
        if (activeOrbiters > 4) activeOrbiters = 2;

        // 같은 타입의 ManaCircle 동기화
        foreach (var circle in stateManager.ManaCircles)
        {
            if (circle.manaType == manaType)
            {
                circle.SetupOrbiters(activeOrbiters);
            }
        }

        // 정답 Circle 개수 체크 
        _hintManager.OnCheckCircleAction?.Invoke(manaType, activeOrbiters);
    }


    bool IsPointOnCircleEdge(Vector2 point)
    {
        float radius = diameter / 2f;
        float distance = Vector2.Distance(transform.position, point);

        // 원래 중심선 기준, 안팎으로 클릭 판정 적용
        return distance >= (radius - clickThreshold / 2f) && distance <= (radius + clickThreshold / 2f);
    }

    void OnValidate()
    {
        if (lineRenderer != null)
        {
            SetupCircle();
            UpdateCollider();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Mana mana = other.GetComponent<Mana>();
        if (mana != null)
        {
            ManaPool.Instance.ReturnMana(mana);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = ManaProperties.GetColor(manaType);

        float radius = diameter / 2f;
        float outerRadius = radius + (clickThreshold / 2f); // 바깥 클릭 범위
        float innerRadius = radius - (clickThreshold / 2f); // 안쪽 클릭 범위

        Vector3 prevOuterPoint = transform.position + new Vector3(outerRadius, 0, 0);
        Vector3 prevInnerPoint = transform.position + new Vector3(innerRadius, 0, 0);

        int segments = 100;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;

            Vector3 newOuterPoint = transform.position + new Vector3(Mathf.Cos(angle) * outerRadius, Mathf.Sin(angle) * outerRadius, 0);
            Vector3 newInnerPoint = transform.position + new Vector3(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius, 0);

            Gizmos.DrawLine(prevOuterPoint, newOuterPoint);
            Gizmos.DrawLine(prevInnerPoint, newInnerPoint);

            prevOuterPoint = newOuterPoint;
            prevInnerPoint = newInnerPoint;
        }
    }

    public void ResetManaCircle(int orbitCount)
    {
        activeOrbiters = orbitCount;
        FindAndModifyAttributeCircuits(true);
    }
}
