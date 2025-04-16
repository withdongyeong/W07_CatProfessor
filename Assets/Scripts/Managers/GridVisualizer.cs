using UnityEngine;
using System.Collections.Generic;

public class GridVisualizer : MonoBehaviour
{
    public Grid grid;
    public bool isGridVisible = false;

    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public Vector2 offset = new Vector2(-20f, -20f);
    public float lineThickness = 0.02f;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    void Start()
    {
        if (grid == null)
            grid = FindAnyObjectByType<Grid>();

        DrawGrid();
        SetGridVisibility(isGridVisible);
    }

    public void ToggleGrid()
    {
        isGridVisible = !isGridVisible;
        SetGridVisibility(isGridVisible);
    }

    void SetGridVisibility(bool visible)
    {
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.enabled = visible;
        }
    }

    void DrawGrid()
    {
        float cellSize = grid.cellSize.x; // 정확한 셀 크기 유지
        Vector3 gridOrigin = grid.transform.position; // Grid의 실제 위치 고려

        for (int x = 0; x <= gridSizeX; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, 0, 0)) + (Vector3)offset;
            Vector3 end = grid.CellToWorld(new Vector3Int(x, gridSizeY, 0)) + (Vector3)offset;
            CreateLine(start, end);
        }

        for (int y = 0; y <= gridSizeY; y++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(0, y, 0)) + (Vector3)offset;
            Vector3 end = grid.CellToWorld(new Vector3Int(gridSizeX, y, 0)) + (Vector3)offset;
            CreateLine(start, end);
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderers.Add(lineRenderer);
    }
}
