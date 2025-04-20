using UnityEngine;
using static GameManager;

public class GuideText : MonoBehaviour


{
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject currentStage;
    [SerializeField]
    string parentName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        parentName = gameObject.transform.parent.gameObject.name;
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.parent == transform) // 직속 자식인지 확인
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if(gameManager.CurrentGameState == gameState.StageSelecting)
        {
            foreach (Transform child in transform)
            {
                if (child.parent == transform) // 직속 자식인지 확인
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ActiveTrue()
    {
        currentStage = gameManager.CurrentPlayingStage;

        if (currentStage.name == parentName)
        {
            foreach (Transform child in transform)
            {
                if (child.parent == transform) // 직속 자식인지 확인
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }


}
