using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject restartPanel;
    public Button restartButton;
    public Button toStageButton;
    public Button baseToStageButton;

    public ManaProperties.ManaType filterType = ManaProperties.ManaType.Aqua;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        restartPanel?.SetActive(false);
        baseToStageButton?.gameObject.SetActive(true);

        restartButton?.onClick.AddListener(RestartGame);
        toStageButton?.onClick.AddListener(MoveToStageSelect);
        baseToStageButton?.onClick.AddListener(MoveToStageSelect);
    }

    public void ShowRestartPanel()
    {
        restartPanel?.SetActive(true);
        baseToStageButton?.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
        restartPanel?.SetActive(false);
        baseToStageButton?.gameObject.SetActive(true);
    }

    public void MoveToStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
        restartPanel?.SetActive(false);
        baseToStageButton?.gameObject.SetActive(true);
    }
}
