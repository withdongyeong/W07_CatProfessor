using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ClickableCircle : MonoBehaviour
{
    private bool wasInside = false;

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

    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.gameState.GamePlaying)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float radius = GetComponent<CircleCollider2D>().radius * transform.lossyScale.x;
        float dist = Vector2.Distance(transform.position, mousePos);

        bool isInside = dist <= radius;

        if (wasInside && !isInside)
        {
            Debug.Log("마우스가 원 바깥으로 벗어남 → 스테이지 나가기");
            GameManager.Instance.ExitStage();
        }

        wasInside = isInside;
    }
}