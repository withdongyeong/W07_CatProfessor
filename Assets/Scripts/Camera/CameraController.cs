using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 5f;

    private Vector3 targetPosition;
    private float targetSize;

    private Camera cam;

    private CinemachineCameraController cine;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetPosition = transform.position;
        targetSize = cam.orthographicSize;

        cine = FindAnyObjectByType<CinemachineCameraController>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }

        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }
    }

    public void MoveToStage(Vector3 stageCenter, int zoomedInSize)
    {
        targetPosition = new Vector3(stageCenter.x, stageCenter.y, transform.position.z);
        targetSize = zoomedInSize;
        cine.MoveToStage(stageCenter, zoomedInSize);
    }

    public void MoveToWorld(Vector3 worldViewCenter, int zoomedOutSize)
    {
        targetPosition = new Vector3(worldViewCenter.x, worldViewCenter.y, transform.position.z);
        targetSize = zoomedOutSize;
        cine.MoveToWorld(worldViewCenter, zoomedOutSize);
    }
}