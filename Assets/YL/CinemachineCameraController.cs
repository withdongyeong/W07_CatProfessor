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
    float moveSpeed = 5f;
    float zoomSpeed = 5f;

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

    void Awake()
    {
        if (virtualCamera == null)
            virtualCamera = FindAnyObjectByType<CinemachineCamera>();

        virtualCamTransform = virtualCamera.transform;
        targetPosition = virtualCamTransform.position;
        targetSize = virtualCamera.Lens.OrthographicSize;

        maxCameraSize = virtualCamera.Lens.OrthographicSize;


    }

    void Update()
    {
        HandleZoom();
        HandleDrag();

        // Position lerp
        if (Vector3.Distance(virtualCamTransform.position, targetPosition) > 0.01f)
        {
            virtualCamTransform.position = Vector3.Lerp(virtualCamTransform.position, targetPosition, Time.deltaTime * moveSpeed);
        }

        // Zoom lerp
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
        maxZoom = zoomedOutSize;
        isDraggable = true; // 드래그 다시 허용
        targetPosition = new Vector3(worldViewCenter.x, worldViewCenter.y, virtualCamTransform.position.z);
        targetSize = zoomedOutSize;
    }

    Vector3 ClampPosition(Vector3 pos)
    {
        float clampedX = Mathf.Clamp(pos.x, -maxCameraSize / 2, maxCameraSize / 2);
        float clampedY = Mathf.Clamp(pos.y, -maxCameraSize / 2, maxCameraSize / 2);
        return new Vector3(clampedX, clampedY, pos.z);
    }
}
