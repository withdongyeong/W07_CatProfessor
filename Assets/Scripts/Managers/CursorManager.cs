using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public GameObject normalCursorPrefab;
    public GameObject clickCursorPrefab;

    private Transform normalCursorTransform;
    private Transform clickCursorTransform;

    private SpriteRenderer normalCursorRenderer;
    private SpriteRenderer clickCursorRenderer;

    public Vector2 offset = Vector2.zero;
    private bool isCursorInsideGame = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        if (normalCursorPrefab != null && clickCursorPrefab != null)
        {
            GameObject normalInstance = Instantiate(normalCursorPrefab);
            normalCursorTransform = normalInstance.transform;
            normalCursorRenderer = normalInstance.GetComponentInChildren<SpriteRenderer>();

            GameObject clickInstance = Instantiate(clickCursorPrefab);
            clickCursorTransform = clickInstance.transform;
            clickCursorRenderer = clickInstance.GetComponentInChildren<SpriteRenderer>();
            clickCursorTransform.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("❌ Cursor Prefab이 설정되지 않았습니다!");
        }
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        bool isInsideCamera = IsMouseInsideCameraView();
        if (isInsideCamera != isCursorInsideGame)
        {
            isCursorInsideGame = isInsideCamera;
            Cursor.visible = !isCursorInsideGame;
        }

        if (isCursorInsideGame && normalCursorTransform != null && clickCursorTransform != null)
        {
            normalCursorTransform.position = mousePosition + offset;
            clickCursorTransform.position = mousePosition + offset;
        }

        // ✅ 클릭 시 1회만 효과음 재생
        if (Input.GetMouseButtonDown(0)) // 버튼을 처음 눌렀을 때
        {
            SoundManager.Instance.PlayClickSound();
            normalCursorTransform.gameObject.SetActive(false);
            clickCursorTransform.gameObject.SetActive(true);
            SetCursorTransparency(0.5f);
        }
        else if (Input.GetMouseButtonUp(0)) // 버튼을 뗄 때 원래대로
        {
            normalCursorTransform.gameObject.SetActive(true);
            clickCursorTransform.gameObject.SetActive(false);
            SetCursorTransparency(1.0f);
        }
    }

    private bool IsMouseInsideCameraView()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float camSize = Camera.main.orthographicSize;
        float camRatio = (float)Screen.width / Screen.height;
        float camWidth = camSize * camRatio;

        return mouseWorldPos.x > -camWidth && mouseWorldPos.x < camWidth &&
               mouseWorldPos.y > -camSize && mouseWorldPos.y < camSize;
    }

    private void SetCursorTransparency(float alpha)
    {
        if (normalCursorRenderer != null)
        {
            Color color = normalCursorRenderer.color;
            color.a = alpha;
            normalCursorRenderer.color = color;
        }

        if (clickCursorRenderer != null)
        {
            Color color = clickCursorRenderer.color;
            color.a = alpha;
            clickCursorRenderer.color = color;
        }
    }
}
