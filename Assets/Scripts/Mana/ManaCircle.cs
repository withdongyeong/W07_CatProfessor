using UnityEngine;
using System.Collections.Generic;

public class ManaCircle : MonoBehaviour
{
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

    void Start()
    {
        SetupCircle();
        SetupCollider();
        SetupOrbiters();
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

    void SetupOrbiters()
    {
        int beforeCount = transform.childCount;
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }

        float radius = diameter / 2f;
        float angleStep = 360f / activeOrbiters;

        for (int i = 0; i < activeOrbiters; i++)
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
                float angle = (Time.time * orbitSpeed + i * angleStep) * Mathf.Deg2Rad;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f) + transform.position;
                child.position = newPos;

                child.Rotate(Vector3.forward * selfRotationSpeed * Time.deltaTime);

                i++;
            }
        }
    }

    void Update()
    {
        UpdateOrbitMotion();

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (IsPointOnCircleEdge(mousePosition))
            {
                if (IsManaPresent()) 
                {
                    Professor.Instance.SayRandom(ScriptManager.ScriptCategory.WaitMana); // ✅ 마나가 있을 때만 대사 출력
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

    void FindAndModifyAttributeCircuits()
    {
        AttributeCircuit[] circuits = FindObjectsByType<AttributeCircuit>(FindObjectsSortMode.InstanceID);

        foreach (var circuit in circuits)
        {
            if (circuit.attributeType == manaType)
            {
                circuit.CycleDirection();
            }
        }

        activeOrbiters += 1;
        if (activeOrbiters > 4) activeOrbiters = 2;
        SetupOrbiters();
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

    void OnTriggerEnter2D(Collider2D other)
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
}
