using UnityEngine;

public class OneSceneManager : MonoBehaviour
{
    private PlayingUI _playingUI;
    private Canvas _playingCanvas;

    private void Start()
    {
        _playingUI = GetComponentInChildren<PlayingUI>();
        _playingCanvas = _playingUI.GetComponent<Canvas>();
    }

    public void ActivatePlayingCanvas(bool isActive)
    {
        _playingCanvas.enabled = isActive;
    }
}
