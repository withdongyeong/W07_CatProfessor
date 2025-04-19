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
        _gameManager.CurrentPlayingStage.GetComponentInChildren<StateManager>().ResetManaCircle();
        ManaPool.Instance.ResetManaPool();
    }
    
    private void OnClickResetManaBtn()
    {
        ManaPool.Instance.ResetManaPool();
    }

    public void ActivateResetBtns(bool isActive)
    {
        resetStageBtn.gameObject.SetActive(isActive);
        resetManaBtn.gameObject.SetActive(isActive);
    }
}
