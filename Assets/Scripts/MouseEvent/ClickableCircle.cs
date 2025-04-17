using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ClickableCircle : MonoBehaviour
{
    GameObject FindStageRoot()
    {
        var marker = GetComponentInParent<StageRootMarker>();
        return marker != null ? marker.gameObject : null;
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
        if (GameManager.Instance.CurrentGameState == GameManager.gameState.StageSelecting)
        {
            var stageRoot = FindStageRoot();
            if (stageRoot != null)
            {
                Debug.Log($"{stageRoot.name} 마우스 클릭됨");
                GameManager.Instance.EnterStage(stageRoot);
            }
            else
            {
                Debug.LogWarning("StageRootMarker를 찾지 못함");
            }

            Debug.Log("게임 플레이로 상태 변환");
        }
    }
}