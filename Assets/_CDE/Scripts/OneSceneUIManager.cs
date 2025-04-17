using UnityEngine;
using UnityEngine.Serialization;

public class OneSceneUIManager : MonoBehaviour
{
    public PlayingUI PlayingUI { get; private set; }
    private Canvas _playingCanvas;

    private void Start()
    {
        PlayingUI = GetComponentInChildren<PlayingUI>();
        _playingCanvas = PlayingUI.GetComponent<Canvas>();
    }

    public void ActivatePlayingCanvas(bool isActive)
    {
        _playingCanvas.enabled = isActive;
        PlayingUI.ActivateResetBtns(true);
    }
}
