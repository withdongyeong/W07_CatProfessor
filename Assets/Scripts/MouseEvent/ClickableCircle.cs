using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ClickableCircle : MonoBehaviour
{
    StageRootMarker FindStageRoot()
    {
        var marker = GetComponentInParent<StageRootMarker>();
        return marker != null ? marker : null;
    }

    private void Awake()
    {
        var collider = GetComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }

    public void SetRadius(float radius)
    {
        var collider = GetComponent<CircleCollider2D>();
        collider.radius = radius;
    }

    void OnMouseEnter()
    {
        Debug.Log($"[{name}] 마우스 진입");
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.gameState.StageSelecting) return;

        var stageRoot = FindStageRoot();
        if (stageRoot == null)
        {
            Debug.LogWarning("StageRootMarker를 찾지 못함");
            return;
        }

        string stageName = stageRoot.name;
        var status = StageDataManager.Instance.GetStageStatus(stageName);

        if (status == StageStatus.Locked)
        {
            Debug.Log($"[{stageName}] 스테이지는 아직 잠겨있습니다.");
            return;
        }

        GameManager.Instance.EnterStage(stageRoot);
    }

}