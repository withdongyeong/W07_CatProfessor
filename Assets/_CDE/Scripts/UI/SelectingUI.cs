using UnityEngine;
using UnityEngine.UI;

public class SelectingUI : MonoBehaviour
{
    [SerializeField] private Button titleBtn;

    public void Start()
    {
        titleBtn.onClick.AddListener(OnClickTitleBtn);
    }

    private void OnClickTitleBtn()
    {
        GameManager.Instance.CurrentGameState = GameManager.gameState.Title;
    }
}
