using UnityEngine;
using UnityEngine.UI;

public class PlayingUI : MonoBehaviour
{
    private GameManager _gameManager;
    
    [SerializeField] private Button backBtn;
    [SerializeField] private Button resetStageBtn;
    [SerializeField] private Button resetManaBtn;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        
        backBtn.onClick.AddListener(OnClickBackBtn);
        resetStageBtn.onClick.AddListener(OnClickResetStageBtn);
        resetManaBtn.onClick.AddListener(OnClickResetManaBtn);
    }

    private void OnClickBackBtn()
    {
        _gameManager.ExitStage();
    }
    
    private void OnClickResetStageBtn()
    {
        _gameManager.CurrentPlayingStage.GetComponentInChildren<StateManager>().ResetDraggable();
    }
    
    private void OnClickResetManaBtn()
    {
        ManaPool.Instance.ResetManaPool();
    }
}
