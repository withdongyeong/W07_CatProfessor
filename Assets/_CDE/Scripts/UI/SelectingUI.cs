using UnityEngine;
using UnityEngine.UI;

public class SelectingUI : MonoBehaviour
{
    [SerializeField] private Button titleBtn;
    [SerializeField] private Button submitBtn;

    public void Start()
    {
        titleBtn.onClick.AddListener(OnClickTitleBtn);
    }

    private void OnClickTitleBtn()
    {
        GameManager.Instance.CurrentGameState = GameManager.gameState.Title;
    }

    public void ActivateSubmitBtn(bool isActive)
    {
        submitBtn.gameObject.SetActive(isActive);
    }
}
