using UnityEngine;

public class OneSceneUIManager : MonoBehaviour
{
    public TitleUI TitleUI { get; private set; }
    private Canvas _titleCanvas;
    
    public SelectingUI SelectingUI { get; private set; }
    private Canvas _selectingCanvas;
    
    public PlayingUI PlayingUI { get; private set; }
    private Canvas _playingCanvas;

    public void Init()
    {
        TitleUI = GetComponentInChildren<TitleUI>();
        _titleCanvas = TitleUI.GetComponent<Canvas>();

        SelectingUI = GetComponentInChildren<SelectingUI>();
        _selectingCanvas = SelectingUI.GetComponent<Canvas>();

        PlayingUI = GetComponentInChildren<PlayingUI>();
        _playingCanvas = PlayingUI.GetComponent<Canvas>();
    }

    public void ChangeUI(GameManager.gameState state)
    {
        ActivateAllCanvas(false);

        switch (state)
        {
            case GameManager.gameState.Title:
                ActivateTitleUI();
                break;
            case GameManager.gameState.StageSelecting:
                ActivateSelectingUI();
                break;
            case GameManager.gameState.GamePlaying:
                ActivatePlayingUI();
                break;
        }
    }

    private void ActivateTitleUI()
    {
        _titleCanvas.enabled = true;
    }

    private void ActivateSelectingUI()
    {
        _selectingCanvas.enabled = true;
    }

    private void ActivatePlayingUI()
    {
        _playingCanvas.enabled = true;
        PlayingUI.ActivateResetBtns(true);
    }

    private void ActivateAllCanvas(bool isActive)
    {
        _titleCanvas.enabled = isActive;
        _selectingCanvas.enabled = isActive;
        _playingCanvas.enabled = isActive;
    }
}
