using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button exitBtn;

    public void Start()
    {
        startBtn.onClick.AddListener(OnClickStartBtn);
        exitBtn.onClick.AddListener(OnClickExitBtn);
    }
    private void OnClickStartBtn()
    {
        GameManager.Instance.CurrentGameState = GameManager.gameState.StageSelecting;
    }

    private void OnClickExitBtn()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
