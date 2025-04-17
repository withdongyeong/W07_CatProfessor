using System;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    public bool isDragable = true;
    private Vector3 offset;
    private Grid grid;
    public static bool isDragging = false;
    private Vector3 originalPosition;
    private ManaProperties.ManaType _type;
    private HintManager _hintManager;

    public void Init(ManaProperties.ManaType type)
    {
        grid = FindFirstObjectByType<Grid>();
        _hintManager = GetComponentInParent<StageRootMarker>().GetComponentInChildren<HintManager>();

        isDragable = true;
        _type = type;
    }

    void OnMouseDown()
    {
        if (!isDragable)
            return;

        if (!NoManaObjectsExist())
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.WaitMana); // ✅ 마나가 있으면 교수님 대사 출력
            return;
        }

        offset = transform.position - GetMouseWorldPosition();
        originalPosition = transform.position;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            SnapToGrid();
        }
    }

    private void SnapToGrid()
    {
        if (grid != null)
        {
            Vector3 objectWorldPosition = transform.position;

            float snappedX = (float)Math.Round(objectWorldPosition.x);
            float snappedY = (float)Math.Round(objectWorldPosition.y);
            
            _hintManager.OnCheckAnswerAction?.Invoke(_type, new Vector2(snappedX, snappedY));

            if (IsPositionOccupied(snappedX, snappedY))
            {
                StartCoroutine(SmoothMove(originalPosition, 0.1f));
                return;
            }

            StartCoroutine(SmoothMove(new Vector3(snappedX, snappedY, transform.position.z), 0.1f));
        }
    }

    private System.Collections.IEnumerator SmoothMove(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private bool NoManaObjectsExist()
    {
        return FindAnyObjectByType<Mana>() == null;
    }

    private bool IsPositionOccupied(float x, float y)
    {
        foreach (AttributeCircuit circuit in FindObjectsByType<AttributeCircuit>(FindObjectsSortMode.InstanceID))
        {
            if (circuit != this && Mathf.Approximately(circuit.transform.position.x, x)
                && Mathf.Approximately(circuit.transform.position.y, y))
            {
                return true;
            }
        }

        foreach (NeutralCircuit circuit in FindObjectsByType<NeutralCircuit>(FindObjectsSortMode.InstanceID))
        {
            if (circuit != this && Mathf.Approximately(circuit.transform.position.x, x)
                && Mathf.Approximately(circuit.transform.position.y, y))
            {
                return true;
            }
        }

        foreach (InputCircuit input in FindObjectsByType<InputCircuit>(FindObjectsSortMode.InstanceID))
        {
            if (Mathf.Approximately(input.transform.position.x, x)
                && Mathf.Approximately(input.transform.position.y, y))
            {
                return true;
            }
        }

        foreach (OutputCircuit output in FindObjectsByType<OutputCircuit>(FindObjectsSortMode.InstanceID))
        {
            if (Mathf.Approximately(output.transform.position.x, x)
                && Mathf.Approximately(output.transform.position.y, y))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsDragging()
    {
        return isDragging;
    }
}
