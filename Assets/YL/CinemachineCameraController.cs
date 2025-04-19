using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CinemachineCameraController : MonoBehaviour
{
    [System.Serializable]
    public struct CameraTarget
    {
        public Vector3 position; // 목표 위치
        public float orthographicSize; // 목표 렌즈 크기 (Orthographic Size)
    }

    [SerializeField]
    CinemachineCamera virtualCamera;
    float moveSpeed = 5f; // 카메라 이동 속도
    float zoomSpeed = 5f; // 줌 속도

    float zoomStep = 2f;
    float minZoom = 5f;
    float maxZoom = 120f;

    private Vector3 targetPosition;
    private float targetSize;

    private Transform virtualCamTransform;

    CameraTarget[] Targets;

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private bool isDraggable = true;

    float maxCameraSize;

    // StageUIController 참조
    [SerializeField]
    private StageUIController stageUIController;

    void Awake()
    {
        if (virtualCamera == null)
            virtualCamera = FindAnyObjectByType<CinemachineCamera>();

        virtualCamTransform = virtualCamera.transform;
        targetPosition = virtualCamTransform.position;
        targetSize = virtualCamera.Lens.OrthographicSize;

        maxCameraSize = virtualCamera.Lens.OrthographicSize;

        // StageUIController 자동으로 찾기
        if (stageUIController == null)
            stageUIController = FindAnyObjectByType<StageUIController>();
    }

    void Update()
    {
        HandleZoom();
        HandleDrag();

        // 위치 부드럽게 이동
        if (Vector3.Distance(virtualCamTransform.position, targetPosition) > 0.01f)
        {
            virtualCamTransform.position = Vector3.Lerp(virtualCamTransform.position, targetPosition, Time.deltaTime * moveSpeed);
        }

        // 줌 부드럽게 조정
        if (Mathf.Abs(virtualCamera.Lens.OrthographicSize - targetSize) > 0.01f)
        {
            virtualCamera.Lens.OrthographicSize = Mathf.Lerp(virtualCamera.Lens.OrthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }
    }

    void HandleDrag()
    {
        if (!isDraggable) return; // 줌인 상태면 드래그 금지

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray)) // 빈 공간일 때만 드래그 시작
            {
                dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = dragOrigin - currentPos;
            targetPosition += delta;
            targetPosition = ClampPosition(targetPosition);
            dragOrigin = currentPos;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetSize -= scroll * zoomStep * zoomSpeed;
            targetSize = Mathf.Clamp(targetSize, minZoom, maxZoom);
        }
    }

    public void MoveToStage(Vector3 stageCenter, float zoomedInSize)
    {
        isDraggable = false; // 드래그 잠금
        maxZoom = zoomedInSize;
        targetPosition = new Vector3(stageCenter.x, stageCenter.y, virtualCamTransform.position.z);
        targetSize = zoomedInSize;
        zoomSpeed = 5f;
    }

    public void MoveToWorld(Vector3 worldViewCenter, float zoomedOutSize)
    {
        maxZoom = maxCameraSize;
        isDraggable = true; // 드래그 허용
        targetPosition = new Vector3(worldViewCenter.x, worldViewCenter.y, virtualCamTransform.position.z);
        targetSize = zoomedOutSize;
    }

    // 활성화된 구간으로 카메라 이동
    public void MoveToActiveSection(float zoomedOutSize)
    {
        if (stageUIController == null)
        {
            Debug.LogError("StageUIController가 할당되지 않았습니다!");
            return;
        }

        // 구간별 available stages 가져오기
        var groupedStages = stageUIController.GroupAvailableStagesBySection();

        // 활성화된 구간 찾기 (available stages가 있는 가장 낮은 구간)
        int activeSection = -1;
        foreach (var section in groupedStages)
        {
            if (section.Value.Count > 0)
            {
                activeSection = section.Key;
                break; // 첫 번째 유효한 구간 사용
            }
        }

        if (activeSection == -1)
        {
            Debug.LogWarning("활성화된 구간이 없습니다!");
            return;
        }

        // 구간의 평균 위치 가져오기
        var averagePositions = stageUIController.GetAveragePositionBySection();
        if (averagePositions.ContainsKey(activeSection))
        {
            Vector3 sectionCenter = averagePositions[activeSection];
            MoveToWorld(sectionCenter, zoomedOutSize);
            Debug.Log($"카메라를 구간 {activeSection}의 중심 {sectionCenter}으로 이동");
        }
        else
        {
            Debug.LogWarning($"구간 {activeSection}의 유효한 위치를 찾을 수 없습니다.");
        }
    }

    Vector3 ClampPosition(Vector3 pos)
    {
        float clampedX = Mathf.Clamp(pos.x, -maxCameraSize / 2, maxCameraSize / 2);
        float clampedY = Mathf.Clamp(pos.y, -maxCameraSize / 2, maxCameraSize / 2);
        return new Vector3(clampedX, clampedY, pos.z);
    }
}